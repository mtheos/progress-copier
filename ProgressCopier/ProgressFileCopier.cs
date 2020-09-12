using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace ProgressCopier {
    public class ProgressFileCopier : IProgressCopier {
        private const int MEGABYTE = 1024 * 1024;
        private readonly ConsoleProgressBar _consoleProgressBar;
        private FileStream _source;
        private FileStream _dest;
        public bool Cancel { private get; set; }


        public ProgressFileCopier (int barLength = 10, double interval = 100) => _consoleProgressBar = new ConsoleProgressBar(barLength, interval);

        public ProgressFileCopier (ConsoleProgressBar cpb) => _consoleProgressBar = cpb;

        public void Copy (string sourceFile, string destinationFile, bool overwrite = false) {
            Cancel = false;
            if (!OKToContinue(sourceFile, destinationFile, overwrite)) return;
            byte[] buffer = new byte[MEGABYTE]; // 1MB buffer
            using (_source = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            using (_dest = new FileStream(destinationFile, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write)) {
                double totalBytes = 0;
                int currentBlockSize;
                _consoleProgressBar.SafeStartStopTimer();
                while ((currentBlockSize = _source.Read(buffer, 0, buffer.Length)) > 0) {
                    if (Cancel) {
                        CancelLogic(destinationFile);
                        return;
                    }
                    totalBytes += currentBlockSize;
                    _consoleProgressBar.Percentage = totalBytes / _source.Length;
                    _dest.Write(buffer, 0, currentBlockSize);
                }
                _consoleProgressBar.SafeStartStopTimer();
                _consoleProgressBar.ForceUpdate();
            }
            _consoleProgressBar.OnComplete();
        }

        private void CancelLogic (string destinationFile) {
            _source.Dispose();
            _dest.Dispose();
            _consoleProgressBar.SafeStartStopTimer();
            if (File.Exists(destinationFile))
                File.Delete(destinationFile);
        }

        [Pure]
        private static long GetFreeSpace (char driveLetter) => (from drive in DriveInfo.GetDrives() where drive.IsReady && drive.Name == $@"{driveLetter}:\" select drive.AvailableFreeSpace).Single();

        private static bool OKToContinue (string sourceFile, string destinationFile, bool overWrite) {
            bool ok = true;
            if (sourceFile == null) {
                Console.WriteLine("Source path is null.");
                ok = false;
            }
            if (destinationFile == null) {
                Console.WriteLine("Destination path is null.");
                ok = false;
            }
            if (ok) {
                long fileLength = new FileInfo(sourceFile).Length;
                long freeSpace = GetFreeSpace(destinationFile.First());
                if (fileLength > freeSpace)
                    Console.WriteLine($"There is not enough space on {destinationFile.First()}. {((float)fileLength - freeSpace) / 1_000_000} more MB are needed");
            }
            if (!File.Exists(sourceFile)) {
                Console.WriteLine($"The path {sourceFile} does not exist.");
                ok = false;
            }
            if (File.Exists(destinationFile) && !overWrite) {
                Console.WriteLine($"The path {destinationFile} already exists.");
                ok = false;
            }
            return ok;
        }
    }
}

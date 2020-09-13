using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace ProgressCopier {
    public class FileCopier : IFileCopier {
        private const int MEGABYTE = 1024 * 1024;
        protected byte[] Buffer { get; set; }
        public bool Cancel { private get; set; }

        public virtual void Copy(string sourceFile, string destinationFile, bool overwrite=false) {
            Cancel = false;
            if (!OKToContinue(sourceFile, destinationFile, overwrite)) return;
            Buffer = new byte[MEGABYTE]; // 1MB buffer
            using Stream source = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
            using Stream dest = new FileStream(destinationFile, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write);
            long totalBytes = 0;
            int block;
            while ((block = source.Read(Buffer, 0, Buffer.Length)) > 0) {
                if (Cancel) {
                    DeleteIncomplete(destinationFile);
                    break;
                }
                dest.Write(Buffer, 0, block);
                totalBytes += block;
                ReportProgress(totalBytes, source.Length);
            }
            Buffer = null;
        }

        protected virtual void ReportProgress(long copied, long size) { }

        private static void DeleteIncomplete(string destinationFile) {
            if (File.Exists(destinationFile))
                File.Delete(destinationFile);
        }

        [Pure]
        private static long GetFreeSpace(char driveLetter) => (from drive in DriveInfo.GetDrives() where drive.IsReady && drive.Name == $@"{driveLetter}:\" select drive.AvailableFreeSpace).Single();

        private static bool OKToContinue(string sourceFile, string destinationFile, bool overWrite) {
            bool ok = true;
            if (sourceFile is null) {
                Console.WriteLine("Source path is null.");
                ok = false;
            }
            if (destinationFile is null) {
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

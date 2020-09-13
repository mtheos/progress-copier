using System;
using System.IO;

namespace UnitTestProgressCopier {
    class FileWriter {
        private readonly FileStream _fs;
        private readonly Random _rand;

        public FileWriter(string dst) {
            _rand = new Random();
            _fs = new FileStream(dst, FileMode.Create, FileAccess.ReadWrite);
        }

        public void Write(long size) {
            const int BUFF_SIZE = 1024 * 1024;
            byte[] buffer = new byte[BUFF_SIZE];
            _rand.NextBytes(buffer);
            long written = 0;
            using BinaryWriter bw = new BinaryWriter(_fs);
            while (written < size) {
                if (written + buffer.Length > size) {
                    bw.Write(buffer, 0, (int)(size - written));
                    written += size - written;
                } else {
                    bw.Write(buffer);
                    written += buffer.Length;
                }
                bw.Flush();
            }
        }
    }
}

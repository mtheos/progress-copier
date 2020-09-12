using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProgressCopier;

namespace UnitTestProgressCopier {
    [TestClass]
    public class ProgressCopierTests {
        private const string SRC_PATH = @"F:\src.dat";
        private const string DST_PATH = @"F:\dst.dat";
        
        [TestMethod]
        public void TestMethod1 () {
            ProgressBar cpb = new ConsoleProgressBar(20);
            ProgressFileCopier pfc = new ProgressFileCopier(cpb);
            cpb.ProgressChanged += delegate { Console.WriteLine(cpb.GetProgressBar()); };
            cpb.Completed += delegate { Console.WriteLine("Yay done"); };
            pfc.Copy(SRC_PATH, DST_PATH, true);
            Assert.AreEqual(1,1);
        }

        [TestInitialize]
        public void Init () {
            new BigFileWriter(SRC_PATH).Run();
        }

        [TestCleanup]
        public void Cleanup () {
            File.Delete(SRC_PATH);
            File.Delete(DST_PATH);
        }
    }

    class BigFileWriter {
        private readonly FileStream _fs;
        private readonly Random _rand;
        private const double SIZE = 1_000_000_000;

        public BigFileWriter(string path) {
            _rand = new Random();
            _fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
        }

        public void Run() {
            int buffSize = 16 * 1024 * 1024;
            if (buffSize <= 0) throw new Exception();
            byte[] buffer = new byte[buffSize];
            _rand.NextBytes(buffer);
            double written = 0;
            using BinaryWriter bw = new BinaryWriter(_fs);
            while (written < SIZE) {
                if (written + buffer.Length > SIZE) {
                    bw.Write(buffer, 0, (int)(SIZE - written));
                    written += SIZE - written;
                } else {
                    bw.Write(buffer);
                    written += buffer.Length;
                }
                bw.Flush();
            }
        }
    }
}

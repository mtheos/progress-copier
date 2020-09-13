using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProgressCopier;

namespace UnitTestProgressCopier {
    [TestClass]
    public class CopierTests {
        private const string SRC_PATH = @"F:\src.dat";
        private const string DST_PATH = @"F:\dst.dat";

        [TestInitialize]
        public void Init() => new FileWriter(SRC_PATH).Write(17 * 1234 * 1234); // ~25mb

        [TestCleanup]
        public void Cleanup() {
            File.Delete(SRC_PATH);
            File.Delete(DST_PATH);
        }

        [TestMethod]
        public void TestCopier() {
            IFileCopier pfc = new FileCopier();
            pfc.Copy(SRC_PATH, DST_PATH, true);
            Assert.IsTrue(File.Exists(DST_PATH));
            Assert.IsTrue(FileComparer.Compare(SRC_PATH, DST_PATH));
        }

        [TestMethod]
        public void TestProgressCopier() {
            IFileCopier pfc = new ProgressFileCopier(new ConsoleProgressBar());
            pfc.Copy(SRC_PATH, DST_PATH, true);
            Assert.IsTrue(File.Exists(DST_PATH));
            Assert.IsTrue(FileComparer.Compare(SRC_PATH, DST_PATH));
        }

        [TestMethod]
        public void TestProgressCopierBar() {
            ProgressBar cpb = new ConsoleProgressBar();
            ProgressFileCopier pfc = new ProgressFileCopier(cpb);
            double percent = cpb.Percentage;
            string progressBar = cpb.GetProgressBar();
            bool complete = default;
            cpb.ProgressChanged += (percentage, bar) => {
                                       percent = percentage;
                                       progressBar = bar;
                                   };
            cpb.Completed += delegate { complete = true; };
            Assert.AreEqual(percent, 0);
            Assert.AreEqual(progressBar, "[>---------]");
            Assert.AreEqual(complete, false);
            pfc.Copy(SRC_PATH, DST_PATH, true);
            Assert.AreEqual(percent, 1);
            Assert.AreEqual(progressBar, "[==========]");
            Assert.AreEqual(complete, true);
        }
    }

    public static class FileComparer {
        public static bool Compare(string file1, string file2) {
            if (file1 == file2)
                return true;
            using Stream fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
            using Stream fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);
            return fs1.Length == fs2.Length && GetFileHash(fs1).SequenceEqual(GetFileHash(fs2));
        }

        private static IEnumerable<byte> GetFileHash(Stream fs) {
            using SHA1Managed hash = new SHA1Managed();
            return hash.ComputeHash(fs);
        }
    }
}

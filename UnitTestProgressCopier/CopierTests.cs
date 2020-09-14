using System.IO;
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
            IFileCopier pfc = new ProgressFileCopier(cpb);
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

        [TestMethod]
        public void TestIProgressBarInterface() {
            IProgressBar cpb = new ConsoleProgressBar();
            IFileCopier pfc = new ProgressFileCopier(cpb);
            double percent = default;
            string progressBar = default;
            bool complete = default;
            cpb.ProgressChanged += (percentage, bar) => {
                                       percent = percentage;
                                       progressBar = bar;
                                   };
            cpb.Completed += delegate { complete = true; };
            pfc.Copy(SRC_PATH, DST_PATH, true);
            Assert.AreEqual(percent, 1);
            Assert.AreEqual(progressBar, "[==========]");
            Assert.AreEqual(complete, true);
        }
    }
}

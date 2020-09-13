using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProgressCopier;

namespace UnitTestProgressCopier {
    [TestClass]
    public class ConsoleProgressBarTests {
        private ProgressBar _cpb;
        [TestInitialize]
        public void Init() {
            _cpb = new ConsoleProgressBar(interval: 0);
        }
        [TestMethod]
        public void SimpleBarTest() {
            bool completed = default;
            _cpb.Completed += delegate { completed = true; };
            _cpb.Percentage = 0d / 100;
            Assert.AreEqual(_cpb.Percentage, 0, Double.Epsilon);
            Assert.AreEqual(_cpb.GetProgressBar(), "[>---------]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 1d / 100;
            Assert.AreEqual(_cpb.Percentage, 0.01, Double.Epsilon);
            Assert.AreEqual(_cpb.GetProgressBar(), "[>---------]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 10d / 100;
            Assert.AreEqual(_cpb.Percentage, 0.1, Double.Epsilon);
            Assert.AreEqual(_cpb.GetProgressBar(), "[=>--------]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 19d / 100;
            Assert.AreEqual(_cpb.Percentage, 0.19, Double.Epsilon);
            Assert.AreEqual(_cpb.GetProgressBar(), "[=>--------]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 50d / 100;
            Assert.AreEqual(_cpb.Percentage, 0.5, Double.Epsilon);
            Assert.AreEqual(_cpb.GetProgressBar(), "[=====>----]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 90d / 100;
            Assert.AreEqual(_cpb.Percentage, 0.9, Double.Epsilon);
            Assert.AreEqual(_cpb.GetProgressBar(), "[=========>]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 99d / 100;
            Assert.AreEqual(_cpb.Percentage, 0.99, Double.Epsilon);
            Assert.AreEqual(_cpb.GetProgressBar(), "[=========>]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 100d / 100;
            Assert.AreEqual(_cpb.Percentage, 1, Double.Epsilon);
            Assert.AreEqual(_cpb.GetProgressBar(), "[==========]");
            Assert.AreEqual(completed, true);
        }

        [TestMethod]
        public void HarderBarTest() {
            bool completed = default;
            _cpb.Completed += delegate { completed = true; };
            _cpb.Percentage = 12_345d / 1_000_000; // 1.2345%
            Assert.AreEqual(_cpb.Percentage, 1.2345 / 100, 0.000_000_001);
            Assert.AreEqual(_cpb.GetProgressBar(), "[>---------]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 99_999d / 1_000_000; // 9.9999%
            Assert.AreEqual(_cpb.Percentage, 9.9999 / 100, 0.000_000_001);
            Assert.AreEqual(_cpb.GetProgressBar(), "[>---------]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 499_999d / 1_000_000; // 49.9999%
            Assert.AreEqual(_cpb.Percentage, 49.9999 / 100, 0.000_000_001);
            Assert.AreEqual(_cpb.GetProgressBar(), "[====>-----]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 500_001d / 1_000_000; // 50.0001%
            Assert.AreEqual(_cpb.Percentage, 50.0001 / 100, 0.000_000_001);
            Assert.AreEqual(_cpb.GetProgressBar(), "[=====>----]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 536_876d / 1_000_000; // 53.6876%
            Assert.AreEqual(_cpb.Percentage, 53.6876 / 100, 0.000_000_001);
            Assert.AreEqual(_cpb.GetProgressBar(), "[=====>----]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 899_999d / 1_000_000; // 89.9999%
            Assert.AreEqual(_cpb.Percentage, 89.9999 / 100, 0.000_000_001);
            Assert.AreEqual(_cpb.GetProgressBar(), "[========>-]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 900_001d / 1_000_000; // 90.0001%
            Assert.AreEqual(_cpb.Percentage, 90.0001 / 100, 0.000_000_001);
            Assert.AreEqual(_cpb.GetProgressBar(), "[=========>]");
            Assert.AreEqual(completed, false);

            _cpb.Percentage = 999_999d / 1_000_000; // 99.9999%
            Assert.AreEqual(_cpb.Percentage, 99.9999 / 100, 0.000_000_001);
            Assert.AreEqual(_cpb.GetProgressBar(), "[=========>]");
            Assert.AreEqual(completed, false);
        }

        [TestMethod]
        public void PercentOverflowTest() {
            try {
                _cpb.Percentage = 1_000_001d / 1_000_000; // 100.0001%
                Assert.Fail("Overflow Percentage did not die");
            } catch (ArgumentOutOfRangeException) {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void PercentUnderflowTest() {
            try {
                _cpb.Percentage = -1d / 1_000_000; // -0.0001%
                Assert.Fail("Underflow Percentage did not die");
            } catch (ArgumentOutOfRangeException) {
                Assert.IsTrue(true);
            }
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace UnitTestProgressCopier {
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

namespace ProgressCopier {
    public interface IFileCopier {
        void Copy(string sourceFile, string destinationFile, bool overwrite = false);
    }
}

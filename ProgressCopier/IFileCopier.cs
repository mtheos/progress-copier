namespace ProgressCopier {
    public interface IFileCopier {
        bool Move(string sourceFile, string destinationFile, bool overwrite = false);
        bool Copy(string sourceFile, string destinationFile, bool overwrite = false);
    }
}

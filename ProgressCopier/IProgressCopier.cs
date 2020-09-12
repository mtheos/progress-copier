namespace ProgressCopier {
    interface IProgressCopier {
        void Copy (string sourceFile, string destinationFile, bool overwrite = false);
    }
}

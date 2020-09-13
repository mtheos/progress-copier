namespace ProgressCopier {
    public class ProgressFileCopier : FileCopier {
        private readonly IProgressBar _progressBar;
        public ProgressFileCopier(IProgressBar cpb) => _progressBar = cpb;
        protected override void ReportProgress(long copied, long size) => _progressBar.Percentage = (double)copied / size;
    }
}

namespace ProgressCopier {
    public interface IProgressBar {
        public delegate void ProgressChangedEventHandler(double percentage, string progressBar);
        public delegate void CompletedEventHandler();
        public event ProgressChangedEventHandler ProgressChanged;
        public event CompletedEventHandler Completed;
        double Percentage { get; set; }
        // string GetProgressBar();
        // void OnComplete();
    }
}

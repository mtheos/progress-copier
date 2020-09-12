namespace ProgressCopier {
    public interface IProgressBar {
        double Percentage { get; set; }
        string GetProgressBar ();
        void OnComplete ();
    }
}

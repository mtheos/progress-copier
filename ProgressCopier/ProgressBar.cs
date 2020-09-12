using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ProgressCopier.Annotations;

namespace ProgressCopier {
    public abstract class ProgressBar : IProgressBar {
        private readonly Stopwatch _intervalTimer;
        private readonly double _interval;

        protected ProgressBar (double interval = 100) {
            _interval = interval;
            _intervalTimer = Stopwatch.StartNew();
            PropertyChanged = PropertyChanged is null ? OnPropertyChanged : (PropertyChangedEventHandler)null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event ProgressChangedEventHandler ProgressChanged;

        public delegate void ProgressChangedEventHandler ();

        public event CompletedEventHandler Completed;

        public delegate void CompletedEventHandler ();

        private double _percentage;
        public double Percentage {
            get => _percentage;
            set {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"0 <= value <= 1 must hold. Given: {value}");
                _percentage = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged ([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public virtual void OnComplete () => Completed?.Invoke();

        private void OnPropertyChanged (object sender, PropertyChangedEventArgs propertyChangedEventArgs) => UpdateProgress();

        private void UpdateProgress () {
            if (_intervalTimer.ElapsedMilliseconds > _interval) {
                _intervalTimer.Restart();
                ProgressChanged?.Invoke();
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Percentage == 1) { // Must hold as what we copy is exactly the whole file
                ProgressChanged?.Invoke();
                OnComplete();
            }
        }

        public abstract string GetProgressBar ();

    }
}

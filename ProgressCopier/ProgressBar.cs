using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ProgressCopier.Annotations;

namespace ProgressCopier {
    public abstract class ProgressBar : IProgressBar {
        private readonly Stopwatch _intervalTimer;
        private readonly long _interval;

        protected ProgressBar(long interval = 100) {
            _interval = interval;
            _intervalTimer = Stopwatch.StartNew();
        }
        public event IProgressBar.ProgressChangedEventHandler ProgressChanged;
        public event IProgressBar.CompletedEventHandler Completed;
        private double _percentage;
        public double Percentage {
            get => _percentage;
            set {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"0 <= value <= 1 must hold. Given: {value}");
                _percentage = value;
                NotifyPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null) => OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        public virtual void OnComplete() => Completed?.Invoke();

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
            if (_intervalTimer.ElapsedMilliseconds > _interval) {
                _intervalTimer.Restart();
                ProgressChanged?.Invoke(Percentage, GetProgressBar());
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Percentage == 1) { // float can exactly store 1
                ProgressChanged?.Invoke(Percentage, GetProgressBar());
                OnComplete();
            }
        }

        public abstract string GetProgressBar();

    }
}

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

        private IProgressBar.ProgressChangedEventHandler _progressChanged;
        public event IProgressBar.ProgressChangedEventHandler ProgressChanged {
            add => _progressChanged += value;
            remove => _progressChanged -= value;
        }

        private IProgressBar.CompletedEventHandler _completed;
        public event IProgressBar.CompletedEventHandler Completed {
            add => _completed += value;
            remove => _completed -= value;
        }

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

        public virtual void OnComplete() => _completed?.Invoke();

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
            if (_intervalTimer.ElapsedMilliseconds > _interval) {
                _intervalTimer.Restart();
                _progressChanged?.Invoke(Percentage, GetProgressBar());
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Percentage == 1) { // float can exactly store 1
                _progressChanged?.Invoke(Percentage, GetProgressBar());
                OnComplete();
            }
        }

        public abstract string GetProgressBar();

    }
}

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using ProgressCopier.Annotations;
using Timer = System.Timers.Timer;

namespace ProgressCopier {
    public class ConsoleProgressBar : ProgressBar, INotifyPropertyChanged {
        private readonly Timer _timer;
        public bool UseTimer = true;
        private int _locks;
        private bool _hasCompleted;

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

        private void OnInterval (object sender, ElapsedEventArgs e) => UpdateProgress();

        private static readonly object ConsoleLock = new object();

        private void UpdateProgress () {
            _locks++;
            lock (ConsoleLock) {
                _hasCompleted = false;
                ProgressChanged?.Invoke();
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (Percentage == 1) { // Set as copied / file size, if this doesn't hold bad things have probably happened
                    Completed?.Invoke();
                    _hasCompleted = true;
                }
                _locks--;
            }
        }

        public virtual void ForceUpdate () => ProgressChanged?.Invoke();

        public ConsoleProgressBar (int barLength = 10, double interval = 100) : base(barLength) {
            _timer = new Timer { Interval = interval };
            _timer.Elapsed += OnInterval;
        }

        public void SafeStartStopTimer () {
            if (UseTimer) {
                PropertyChanged = null;
                if (_timer.Enabled) {
                    while (!_hasCompleted) Thread.Sleep(TimeSpan.FromMilliseconds(100));
                    _timer.Stop();
                    while (_locks > 0) Thread.Sleep(TimeSpan.FromMilliseconds(1));
                } else
                    _timer.Start();
            } else
                PropertyChanged = PropertyChanged is null ? OnPropertyChanged : (PropertyChangedEventHandler)null;
        }
    }
}

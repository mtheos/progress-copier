using System;
using System.Text;

namespace ProgressCopier {
    public abstract class ProgressBar : IProgressBar {
        /// <summary>
        /// If used, must be a 5 char array with characters for
        /// 'start of bar', 'completed segment', 'current segment', 'uncompleted segment' and 'end of bar' in that order. i.e. { '[', '=', '>', '-', ']' }
        /// </summary>
        private readonly char[] _segChars = { '[', '=', '>', '-', ']' };

        private int _barLength;
        private int BarLength {
            get => _barLength;
            set {
                if (_barLength < 0)
                    throw new ArgumentOutOfRangeException(nameof(BarLength), "Bar length must be non-negative");
                _barLength = value;
            }
        }

        protected ProgressBar (int barLength, char[] segChars = null) {
            BarLength = barLength;
            if (segChars == null)
                return; // use default
            if (segChars.Length != 5)
                throw new Exception(@"Array must contain 5 characters.");
            _segChars = segChars;
        }

        public string GetProgressBar (double percentage) {
            int usedSegments = (int)(BarLength * percentage);
            StringBuilder sb = new StringBuilder($"{_segChars[0]}{new string(_segChars[1], usedSegments)}{new string(_segChars[3], BarLength - usedSegments)}{_segChars[4]}");
            if (usedSegments != BarLength)
                sb[usedSegments + 1] = _segChars[2];
            return sb.ToString();
        }
    }
}

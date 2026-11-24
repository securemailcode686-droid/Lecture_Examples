using System;
using System.IO;
using System.Collections.Generic;

namespace LogHub.Importing
{
    public sealed class LogInput
    {
        private readonly Func<TextReader> _open;
        private readonly Func<TextReader> _openMemory;

        private LogInput(
            Func<TextReader> open,
            Func<TextReader> openMemory)
        {
            _open = open;
            _openMemory = openMemory;
        }

        public static LogInput FromFile(string path)
        {
            return new LogInput(
                () => File.OpenText(path),
                () => new LineReader(File.ReadAllLines(path)));
        }

        public static LogInput FromLines(
            IEnumerable<string> lines)
        {
            return new LogInput(
                () => new LineReader(lines),
                () => new LineReader(lines));
        }

        public TextReader Open() => _open();

        public TextReader OpenMemory() => _openMemory();

        private sealed class LineReader : TextReader
        {
            private readonly IEnumerator<string> _lines;

            public LineReader(IEnumerable<string> lines)
            {
                _lines = lines.GetEnumerator();
            }

            public override string? ReadLine()
            {
                return _lines.MoveNext()
                    ? _lines.Current
                    : null;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    _lines.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}

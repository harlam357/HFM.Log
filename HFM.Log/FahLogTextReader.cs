
using System;
using System.IO;
using System.Threading.Tasks;

namespace HFM.Log
{
    /// <summary>
    /// Represents a reader that provides fast, forward-only access to Folding@Home log line text data.
    /// </summary>
    public abstract class FahLogTextReader : FahLogReader
    {
        private readonly TextReader _textReader;
        /// <summary>
        /// Initializes a new instance of the <see cref="FahLogTextReader"/> class.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> that provides line data as a string.</param>
        protected FahLogTextReader(TextReader textReader)
        {
            _textReader = textReader ?? throw new ArgumentNullException(nameof(textReader));
        }

        private int _lineIndex;

        /// <summary>
        /// Reads a line of characters from the log and returns the data as a LogLine.
        /// </summary>
        /// <returns>The next line from the reader, or null if all lines have been read.</returns>
        public override LogLine ReadLine()
        {
            string line = _textReader.ReadLine();
            if (line == null) return null;
            return OnReadLine(line, _lineIndex++);
        }

        /// <summary>
        /// Reads a line of characters asynchronously and returns the data as a LogLine.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation. The value of the TResult parameter contains the next line from the reader, or is null if all of the lines have been read.</returns>
        public override async Task<LogLine> ReadLineAsync()
        {
            string line = await _textReader.ReadLineAsync().ConfigureAwait(false);
            if (line == null) return null;
            return OnReadLine(line, _lineIndex++);
        }

        /// <summary>
        /// Occurs after a line was read from the <see cref="TextReader"/> and returns a new <see cref="LogLine"/> object.
        /// </summary>
        /// <param name="line">The line read from the <see cref="TextReader"/>.</param>
        /// <param name="index">The index of the line read from the <see cref="TextReader"/>.</param>
        /// <returns>A new <see cref="LogLine"/> object from the string line and line index.</returns>
        protected abstract LogLine OnReadLine(string line, int index);

        /// <summary>
        /// Closes the <see cref="FahLogTextReader"/> and releases any system resources associated with the it.
        /// </summary>
        public override void Close()
        {
            _textReader.Close();
        }
    }
}
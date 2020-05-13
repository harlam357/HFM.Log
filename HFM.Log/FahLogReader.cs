
using System;
using System.Threading.Tasks;

namespace HFM.Log
{
    /// <summary>
    /// Represents a reader that provides fast, forward-only access to Folding@Home log line data.
    /// </summary>
    public abstract class FahLogReader : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FahLogReader"/> class.
        /// </summary>
        protected FahLogReader()
        {

        }

        /// <summary>
        /// Reads a line of characters from the log and returns the data as a LogLine.
        /// </summary>
        public abstract LogLine ReadLine();

        /// <summary>
        /// Reads a line of characters asynchronously and returns the data as a LogLine.
        /// </summary>
        public abstract Task<LogLine> ReadLineAsync();

        /// <summary>
        /// Closes the <see cref="FahLogReader"/> and releases any system resources associated with the it.
        /// </summary>
        public virtual void Close()
        {

        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FahLogReader"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="FahLogReader"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

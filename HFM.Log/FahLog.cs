
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HFM.Log
{
    /// <summary>
    /// Represents a Folding@Home client log.
    /// </summary>
    public abstract class FahLog
    {
        /// <summary>
        /// Gets the <see cref="RunDataAggregator"/> instance.
        /// </summary>
        protected internal RunDataAggregator RunDataAggregator { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FahLog"/> class.
        /// </summary>
        /// <param name="runDataAggregator">The <see cref="RunDataAggregator"/> that will be used to generate <see cref="ClientRunData"/>, <see cref="SlotRunData"/>, and <see cref="UnitRunData"/> objects.</param>
        protected FahLog(RunDataAggregator runDataAggregator)
        {
            RunDataAggregator = runDataAggregator ?? throw new ArgumentNullException(nameof(runDataAggregator));
        }

        private List<ClientRun> _clientRuns;
        /// <summary>
        /// Gets the collection of <see cref="ClientRun"/> objects.
        /// </summary>
        public IList<ClientRun> ClientRuns
        {
            get { return _clientRuns ?? (_clientRuns = new List<ClientRun>()); }
        }

        /// <summary>
        /// Reads Folding@Home log line data from the <see cref="FahLogReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="FahLogReader"/> that reads the Folding@Home log line data.</param>
        public void Read(FahLogReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            LogLine logLine;
            while ((logLine = reader.ReadLine()) != null)
            {
                OnLogLineRead(logLine);
            }
            OnClientRunFinished();
        }

        /// <summary>
        /// Reads Folding@Home log line data asynchronously from the <see cref="FahLogReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="FahLogReader"/> that reads the Folding@Home log line data.</param>
        public async Task ReadAsync(FahLogReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            LogLine logLine;
            while ((logLine = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                OnLogLineRead(logLine);
            }
            OnClientRunFinished();
        }

        /// <summary>
        /// Clears the log data.
        /// </summary>
        public void Clear()
        {
            _clientRuns?.Clear();
        }

        /// <summary>
        /// Occurs after a <see cref="LogLine"/> was read from the <see cref="FahLogReader"/>.
        /// </summary>
        /// <param name="logLine">The <see cref="LogLine"/> that was read from the <see cref="FahLogReader"/>.</param>
        protected abstract void OnLogLineRead(LogLine logLine);

        /// <summary>
        /// Occurs after log information indicates that a <see cref="ClientRun"/> has been finished.
        /// </summary>
        protected abstract void OnClientRunFinished();
    }
}

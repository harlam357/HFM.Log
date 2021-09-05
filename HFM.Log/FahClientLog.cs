using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HFM.Log.Internal;

namespace HFM.Log
{
    /// <summary>
    /// Represents a Folding@Home client log from a v7 or newer client.
    /// </summary>
    public class FahClientLog : FahLog
    {
        private int _lineIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientLog"/> class.
        /// </summary>
        public FahClientLog()
            : this(null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientLog"/> class.
        /// </summary>
        /// <param name="runDataAggregator">The <see cref="RunDataAggregator"/> that will be used to generate <see cref="ClientRunData"/>, <see cref="SlotRunData"/>, and <see cref="UnitRunData"/> objects.</param>
        protected FahClientLog(RunDataAggregator runDataAggregator)
            : base(runDataAggregator ?? FahClientRunDataAggregator.Instance)
        {

        }

        /// <summary>
        /// Reads the log file from the given path and returns a new <see cref="FahClientLog"/> object.
        /// </summary>
        /// <param name="path">The log file path.</param>
        /// <returns>A new <see cref="FahClientLog"/> object.</returns>
        public static FahClientLog Read(string path)
        {
            using (var stream = OpenFileStream(path))
            {
                return Read(stream);
            }
        }

        /// <summary>
        /// Reads the log file from the given path and returns a new <see cref="FahClientLog"/> object.
        /// </summary>
        /// <param name="stream">A stream containing the log file text.</param>
        /// <returns>A new <see cref="FahClientLog"/> object.</returns>
        public static FahClientLog Read(Stream stream)
        {
            using (var textReader = new StreamReader(stream))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                var log = new FahClientLog();
                log.Read(reader);
                return log;
            }
        }

        /// <summary>
        /// Reads the log file asynchronously from the given path and returns a new <see cref="FahClientLog"/> object.
        /// </summary>
        /// <param name="path">The log file path.</param>
        /// <returns>A new <see cref="FahClientLog"/> object.</returns>
        public static async Task<FahClientLog> ReadAsync(string path)
        {
            using (var stream = OpenFileStream(path))
            {
                return await ReadAsync(stream).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Reads the log file asynchronously from the given path and returns a new <see cref="FahClientLog"/> object.
        /// </summary>
        /// <param name="stream">A stream containing the log file text.</param>
        /// <returns>A new <see cref="FahClientLog"/> object.</returns>
        public static async Task<FahClientLog> ReadAsync(Stream stream)
        {
            using (var textReader = new StreamReader(stream))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                var log = new FahClientLog();
                await log.ReadAsync(reader).ConfigureAwait(false);
                return log;
            }
        }

        private static Stream OpenFileStream(string path)
        {
            // equivalent to StreamReader opening a file for reading
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
        }

        /// <summary>
        /// Occurs after a <see cref="LogLine"/> was read from the <see cref="FahLogReader"/>.
        /// </summary>
        /// <param name="logLine">The <see cref="LogLine"/> that was read from the <see cref="FahLogReader"/>.</param>
        protected override void OnLogLineRead(LogLine logLine)
        {
            logLine.Index = _lineIndex++;

            var properties = GetLogLineProperties(logLine);
            if (!properties.IsWorkUnitLogLine)
            {
                var clientRun = EnsureClientRunExists(logLine.Index);
                clientRun.LogLines.Add(logLine);
            }
            else
            {
                var unitRun = EnsureUnitRunExists(logLine.Index, properties.FoldingSlot, properties.QueueIndex);
                if (logLine.LineType == LogLineType.WorkUnitCleaningUp)
                {
                    unitRun.EndIndex = logLine.Index;
                    unitRun.IsComplete = true;
                }

                unitRun.LogLines.Add(logLine);
            }
        }

        /// <summary>
        /// Occurs after log information indicates that a <see cref="ClientRun"/> has been finished.
        /// </summary>
        protected override void OnClientRunFinished()
        {
            var clientRun = ClientRuns.LastOrDefault();
            if (clientRun == null)
            {
                return;
            }

            foreach (var unitRun in clientRun.SlotRuns.Values.SelectMany(x => x.UnitRuns).Cast<FahClientUnitRun>())
            {
                if (!unitRun.IsComplete)
                {
                    unitRun.EndIndex = unitRun.LogLines[unitRun.LogLines.Count - 1].Index;
                }
            }
        }

        private static LogLineProperties GetLogLineProperties(LogLine logLine)
        {
            Match workUnitRunningMatch;
            if ((workUnitRunningMatch = Internal.FahLogRegex.FahClient.WorkUnitRunningRegex.Match(logLine.Raw)).Success)
            {
                return new LogLineProperties
                {
                    IsWorkUnitLogLine = true,
                    QueueIndex = Int32.Parse(workUnitRunningMatch.Groups["UnitIndex"].Value, CultureInfo.InvariantCulture),
                    FoldingSlot = Int32.Parse(workUnitRunningMatch.Groups["FoldingSlot"].Value, CultureInfo.InvariantCulture)
                };
            }

            return new LogLineProperties();
        }

        private struct LogLineProperties
        {
            public bool IsWorkUnitLogLine { get; set; }
            public int QueueIndex { get; set; }
            public int FoldingSlot { get; set; }
        }

        private ClientRun EnsureClientRunExists(int lineIndex)
        {
            if (ClientRuns.Count == 0)
            {
                ClientRuns.Add(new ClientRun(this, lineIndex));
            }

            return ClientRuns.Last();
        }

        private SlotRun EnsureSlotRunExists(int lineIndex, int foldingSlot)
        {
            var clientRun = EnsureClientRunExists(lineIndex);
            if (!clientRun.SlotRuns.ContainsKey(foldingSlot))
            {
                clientRun.SlotRuns[foldingSlot] = new SlotRun(clientRun, foldingSlot);
            }

            return clientRun.SlotRuns[foldingSlot];
        }

        private FahClientUnitRun EnsureUnitRunExists(int lineIndex, int foldingSlot, int queueIndex)
        {
            var slotRun = EnsureSlotRunExists(lineIndex, foldingSlot);
            var unitRun = GetMostRecentUnitRun(slotRun, queueIndex);
            if (unitRun == null)
            {
                unitRun = new FahClientUnitRun(slotRun, queueIndex, lineIndex);
                slotRun.UnitRuns.Add(unitRun);
            }

            return unitRun;
        }

        private static FahClientUnitRun GetMostRecentUnitRun(SlotRun slotRun, int queueIndex)
        {
            return slotRun.UnitRuns
                .LastOrDefault<UnitRun, FahClientUnitRun>(x => x.QueueIndex == queueIndex && !x.IsComplete);
        }

        /// <summary>
        /// A <see cref="FahClientUnitRun"/> encapsulates all the Folding@Home client log information for a single work unit execution (run).
        /// </summary>
        private class FahClientUnitRun : UnitRun
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FahClientUnitRun"/> class.
            /// </summary>
            /// <param name="parent">The parent <see cref="SlotRun"/> object.</param>
            /// <param name="queueIndex">The queue index.</param>
            /// <param name="startIndex">The log line index for the starting line of this unit run.</param>
            internal FahClientUnitRun(SlotRun parent, int queueIndex, int startIndex)
                : base(parent, queueIndex, startIndex)
            {

            }

            /// <summary>
            /// Gets or sets a value indicating if the <see cref="UnitRun"/> is complete.
            /// </summary>
            internal bool IsComplete { get; set; }
        }
    }
}
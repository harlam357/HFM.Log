
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HFM.Log
{
    /// <summary>
    /// A <see cref="UnitRun"/> encapsulates all the Folding@Home client log information for a single work unit execution (run).
    /// </summary>
    public class UnitRun
    {
        /// <summary>
        /// Gets the parent <see cref="SlotRun"/> object.
        /// </summary>
        public SlotRun Parent { get; }

        private readonly ObservableCollection<LogLine> _logLines;
        /// <summary>
        /// Gets a collection of <see cref="LogLine"/> assigned to this unit run.
        /// </summary>
        public IList<LogLine> LogLines
        {
            get { return _logLines; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitRun"/> class.
        /// </summary>
        /// <param name="parent">The parent <see cref="SlotRun"/> object.</param>
        /// <param name="queueIndex">The queue index.</param>
        /// <param name="startIndex">The log line index for the starting line of this unit run.</param>
        public UnitRun(SlotRun parent, int queueIndex, int startIndex)
        {
            Parent = parent;
            QueueIndex = queueIndex;
            StartIndex = startIndex;

            _logLines = new ObservableCollection<LogLine>();
            _logLines.CollectionChanged += (s, e) => IsDirty = true;
        }

        // for unit testing only
        internal UnitRun(SlotRun parent, int queueIndex, int startIndex, int endIndex)
        {
            Parent = parent;
            QueueIndex = queueIndex;
            StartIndex = startIndex;
            EndIndex = endIndex;

            _logLines = new ObservableCollection<LogLine>();
            _logLines.CollectionChanged += (s, e) => IsDirty = true;
        }

        /// <summary>
        /// Gets the queue index.
        /// </summary>
        public int QueueIndex { get; }

        /// <summary>
        /// Gets the log line index for the starting line of this unit run.
        /// </summary>
        public int StartIndex { get; }

        /// <summary>
        /// Gets or sets the log line index for the ending line of this unit run.
        /// </summary>
        public int? EndIndex { get; set; }

        private UnitRunData _data;
        /// <summary>
        /// Gets the data object containing information aggregated from the <see cref="LogLine"/> objects assigned to this unit run.
        /// </summary>
        public UnitRunData Data
        {
            get
            {
                if (_data == null || IsDirty)
                {
                    IsDirty = false;
                    _data = Parent.Parent.Parent.RunDataAggregator.GetUnitRunData(this);
                }
                return _data;
            }
            set
            {
                IsDirty = false;
                _data = value;
            }
        }

        private bool _isDirty = true;
        /// <summary>
        /// Gets or sets a value indicating if the <see cref="Data"/> property value is not current.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                if (Parent != null && _isDirty)
                {
                    Parent.IsDirty = true;
                }
            }
        }
    }

    /// <summary>
    /// Represents data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="UnitRun"/> object.
    /// </summary>
    public abstract class UnitRunData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitRunData"/> class.
        /// </summary>
        protected UnitRunData()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitRunData"/> class.
        /// </summary>
        /// <param name="other">The other instance from which data will be copied.</param>
        protected UnitRunData(UnitRunData other)
        {
            if (other == null) return;

            UnitStartTimeStamp = other.UnitStartTimeStamp;
            FramesObserved = other.FramesObserved;
            CoreVersion = other.CoreVersion;
            ProjectID = other.ProjectID;
            ProjectRun = other.ProjectRun;
            ProjectClone = other.ProjectClone;
            ProjectGen = other.ProjectGen;
            WorkUnitResult = other.WorkUnitResult;
            Frames = CopyFrames(other.Frames);
        }

        private static Dictionary<int, LogLineFrameData> CopyFrames(IDictionary<int, LogLineFrameData> source)
        {
            if (source == null) return null;

            var copy = new Dictionary<int, LogLineFrameData>();
            foreach (var kvp in source)
            {
                var valueCopy = new LogLineFrameData(kvp.Value);
                copy.Add(kvp.Key, valueCopy);
            }
            return copy;
        }

        /// <summary>
        /// Gets or sets the time stamp of the start of the work unit.
        /// </summary>
        public TimeSpan? UnitStartTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the number of frames observed (completed) since last unit start or resume from pause.
        /// </summary>
        public int FramesObserved { get; set; }

        /// <summary>
        /// Gets or sets the core version number.
        /// </summary>
        public string CoreVersion { get; set; }

        /// <summary>
        /// Gets or sets the project ID (Number).
        /// </summary>
        public int ProjectID { get; set; }

        /// <summary>
        /// Gets or sets the project ID (Run).
        /// </summary>
        public int ProjectRun { get; set; }

        /// <summary>
        /// Gets or sets the project ID (Clone).
        /// </summary>
        public int ProjectClone { get; set; }

        /// <summary>
        /// Gets or sets the project ID (Gen).
        /// </summary>
        public int ProjectGen { get; set; }

        /// <summary>
        /// Gets or sets the work unit result.
        /// </summary>
        public string WorkUnitResult { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of frame data parsed from log lines.
        /// </summary>
        public IDictionary<int, LogLineFrameData> Frames { get; set; }
    }
}
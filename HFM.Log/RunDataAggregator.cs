
using System;
using System.Collections.Generic;
using System.Linq;

using static HFM.Log.Internal.CollectionExtensions;

namespace HFM.Log
{
    /// <summary>
    /// Creates run data objects by aggregating information owned by <see cref="ClientRun"/>, <see cref="SlotRun"/>, and <see cref="UnitRun"/> objects.
    /// </summary>
    public abstract class RunDataAggregator
    {
        /// <summary>
        /// Creates a new <see cref="ClientRunData"/> object from the information contained in the <see cref="ClientRun"/> object.
        /// </summary>
        public ClientRunData GetClientRunData(ClientRun clientRun)
        {
            return OnGetClientRunData(clientRun);
        }

        /// <summary>
        /// Creates a new <see cref="ClientRunData"/> object from the information contained in the <see cref="ClientRun"/> object.
        /// </summary>
        protected abstract ClientRunData OnGetClientRunData(ClientRun clientRun);

        /// <summary>
        /// Creates a new <see cref="SlotRunData"/> object from the information contained in the <see cref="SlotRun"/> object.
        /// </summary>
        public SlotRunData GetSlotRunData(SlotRun slotRun)
        {
            return OnGetSlotRunData(slotRun);
        }

        /// <summary>
        /// Creates a new <see cref="SlotRunData"/> object from the information contained in the <see cref="SlotRun"/> object.
        /// </summary>
        protected abstract SlotRunData OnGetSlotRunData(SlotRun slotRun);

        /// <summary>
        /// Creates a new <see cref="UnitRunData"/> object from the information contained in the <see cref="UnitRun"/> object.
        /// </summary>
        public UnitRunData GetUnitRunData(UnitRun unitRun)
        {
            return OnGetUnitRunData(unitRun);
        }

        /// <summary>
        /// Creates a new <see cref="UnitRunData"/> object from the information contained in the <see cref="UnitRun"/> object.
        /// </summary>
        protected abstract UnitRunData OnGetUnitRunData(UnitRun unitRun);
    }

    namespace Internal
    {
        internal static class CommonRunDataAggregator
        {
            internal static void IncrementCompletedOrFailedUnitCount(SlotRunData slotRunData, UnitRunData unitRunData)
            {
                if (unitRunData.WorkUnitResult == WorkUnitResult.FINISHED_UNIT)
                {
                    slotRunData.CompletedUnits++;
                }
                else if (IsFailedWorkUnit(unitRunData.WorkUnitResult))
                {
                    slotRunData.FailedUnits++;
                }
            }

            private static bool IsFailedWorkUnit(string result)
            {
                switch (result)
                {
                    case WorkUnitResult.EARLY_UNIT_END:
                    case WorkUnitResult.UNSTABLE_MACHINE:
                    case WorkUnitResult.BAD_WORK_UNIT:
                        return true;
                    default:
                        return false;
                }
            }

            internal static void CalculateFrameDataDurations(IDictionary<int, WorkUnitFrameData> frameDataDictionary)
            {
                foreach (var frameData in frameDataDictionary.Values)
                {
                    int previousFrameID = frameData.ID - 1;
                    if (frameDataDictionary.ContainsKey(previousFrameID))
                    {
                        var previousFrameData = frameDataDictionary[previousFrameID];
                        frameData.Duration = GetTimeSpanDelta(frameData.TimeStamp, previousFrameData.TimeStamp);
                    }
                }
            }

            /// <summary>
            /// Get time delta between the TimeSpan values.
            /// </summary>
            /// <param name="first">The first TimeSpan value.</param>
            /// <param name="second">The second TimeSpan value.</param>
            private static TimeSpan GetTimeSpanDelta(TimeSpan first, TimeSpan second)
            {
                TimeSpan delta;

                // check for rollover back to 00:00:00 time1 will be less than previous time2 reading
                if (first < second)
                {
                    // get time before rollover
                    delta = TimeSpan.FromDays(1).Subtract(second);
                    // add time from latest reading
                    delta = delta.Add(first);
                }
                else
                {
                    delta = first.Subtract(second);
                }

                return delta;
            }
        }
    }

    namespace FahClient
    {
        /// <summary>
        /// Creates run data objects for a v7 or newer client.
        /// </summary>
        public class FahClientRunDataAggregator : RunDataAggregator
        {
            /// <summary>
            /// Gets a singleton instance of the <see cref="FahClientRunDataAggregator"/> class.
            /// </summary>
            public static FahClientRunDataAggregator Instance { get; } = new FahClientRunDataAggregator();

            /// <summary>
            /// Creates a new <see cref="ClientRunData"/> object from the information contained in the <see cref="ClientRun"/> object.
            /// </summary>
            protected override ClientRunData OnGetClientRunData(ClientRun clientRun)
            {
                var clientRunData = new FahClientClientRunData();

                foreach (var line in clientRun.LogLines)
                {
                    switch (line.LineType)
                    {
                        case LogLineType.LogOpen:
                            clientRunData.StartTime = (DateTime)line.Data;
                            break;
                    }
                }

                return clientRunData;
            }

            /// <summary>
            /// Creates a new <see cref="SlotRunData"/> object from the information contained in the <see cref="SlotRun"/> object.
            /// </summary>
            protected override SlotRunData OnGetSlotRunData(SlotRun slotRun)
            {
                var slotRunData = new FahClientSlotRunData();

                foreach (var unitRun in slotRun.UnitRuns)
                {
                    Internal.CommonRunDataAggregator.IncrementCompletedOrFailedUnitCount(slotRunData, unitRun.Data);
                }

                return slotRunData;
            }

            /// <summary>
            /// Creates a new <see cref="UnitRunData"/> object from the information contained in the <see cref="UnitRun"/> object.
            /// </summary>
            protected override UnitRunData OnGetUnitRunData(UnitRun unitRun)
            {
                var unitRunData = new FahClientUnitRunData();
                var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
                foreach (var line in unitRun.LogLines)
                {
                    switch (line.LineType)
                    {
                        case LogLineType.WorkUnitWorking:
                            unitRunData.UnitStartTimeStamp = line.TimeStamp;
                            unitRunData.WorkUnitResult = Internal.WorkUnitResult.None;
                            break;
                        case LogLineType.WorkUnitFrame:
                            if (unitRunData.UnitStartTimeStamp == null)
                            {
                                unitRunData.UnitStartTimeStamp = line.TimeStamp;
                            }
                            if (line.Data != null && !(line.Data is LogLineDataParserError))
                            {
                                unitRunData.FramesObserved++;
                                if (line.Data is WorkUnitFrameData frameData)
                                {
                                    // Make a copy so UnitRunData is not holding a reference to the same 
                                    // WorkUnitFrameData instance as the LogLine it was sourced from.
                                    frameDataDictionary.TryAdd(frameData.ID, new WorkUnitFrameData(frameData));
                                }
                            }
                            break;
                        case LogLineType.WorkUnitCoreVersion:
                            if (line.Data != null && !(line.Data is LogLineDataParserError))
                            {
                                unitRunData.CoreVersion = (string)line.Data;
                            }
                            break;
                        case LogLineType.WorkUnitProject:
                            var projectData = (WorkUnitProjectData)line.Data;
                            unitRunData.ProjectID = projectData.ProjectID;
                            unitRunData.ProjectRun = projectData.ProjectRun;
                            unitRunData.ProjectClone = projectData.ProjectClone;
                            unitRunData.ProjectGen = projectData.ProjectGen;
                            break;
                        case LogLineType.WorkUnitCoreReturn:
                            unitRunData.WorkUnitResult = (string)line.Data;
                            if (unitRunData.WorkUnitResult == Internal.WorkUnitResult.INTERRUPTED ||
                                unitRunData.WorkUnitResult == Internal.WorkUnitResult.UNKNOWN_ENUM)
                            {
                                unitRunData.FramesObserved = 0;
                            }
                            break;
                    }
                }
                Internal.CommonRunDataAggregator.CalculateFrameDataDurations(frameDataDictionary);
                unitRunData.FrameDataDictionary = frameDataDictionary;
                return unitRunData;
            }
        }
    }
}

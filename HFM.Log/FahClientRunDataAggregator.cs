
using System;
using System.Collections.Generic;

using HFM.Log.Internal;

namespace HFM.Log
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
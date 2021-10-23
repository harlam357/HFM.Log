using System;
using System.Collections.Generic;

using static HFM.Log.Internal.CollectionExtensions;

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

            int count = clientRun.LogLines.Count;
            for (int i = 0; i < count; i++)
            {
                var line = clientRun.LogLines[i];
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

            int count = slotRun.UnitRuns.Count;
            for (int i = 0; i < count; i++)
            {
                var unitRun = slotRun.UnitRuns[i];
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
            var frames = new Dictionary<int, LogLineFrameData>();

            int count = unitRun.LogLines.Count;
            for (int i = 0; i < count; i++)
            {
                var line = unitRun.LogLines[i];
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
                            if (line.Data is LogLineFrameData frameData)
                            {
                                // Make a copy so UnitRunData is not holding a reference to the same 
                                // WorkUnitFrameData instance as the LogLine it was sourced from.
                                var frameDataCopy = new LogLineFrameData(frameData);
                                frames.TryAdd(frameData.ID, frameDataCopy);

                                if (unitRunData.FramesObserved > 0)
                                {
                                    Internal.CommonRunDataAggregator.CalculateFrameDataDuration(frameDataCopy, frames);
                                }
                            }
                            unitRunData.FramesObserved++;
                        }
                        break;
                    case LogLineType.WorkUnitCoreVersion:
                        if (line.Data != null && !(line.Data is LogLineDataParserError))
                        {
                            unitRunData.CoreVersion = (string)line.Data;
                        }
                        break;
                    case LogLineType.WorkUnitProject:
                        var projectData = (LogLineProjectData)line.Data;
                        unitRunData.ProjectID = projectData.ProjectID;
                        unitRunData.ProjectRun = projectData.ProjectRun;
                        unitRunData.ProjectClone = projectData.ProjectClone;
                        unitRunData.ProjectGen = projectData.ProjectGen;
                        break;
                    case LogLineType.WorkUnitCoreReturn:
                        unitRunData.WorkUnitResult = (string)line.Data;
                        switch (unitRunData.WorkUnitResult)
                        {
                            case Internal.WorkUnitResult.INTERRUPTED:
                            case Internal.WorkUnitResult.UNKNOWN_ENUM:
                                unitRunData.FramesObserved = 0;
                                break;
                            case Internal.WorkUnitResult.CORE_RESTART:
                                unitRunData.FramesObserved = 0;
                                frames.Clear();
                                break;
                        }
                        break;
                }
            }

            unitRunData.Frames = frames;
            return unitRunData;
        }
    }
}

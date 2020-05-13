
using System;
using System.Collections.Generic;

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
}

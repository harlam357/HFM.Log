
using System.Linq;

using NUnit.Framework;

namespace HFM.Log
{
    internal static class AssertClientRun
    {
        internal static void AreEqual(ClientRun expectedRun, ClientRun actualRun, bool assertUnitRunData = false)
        {
            Assert.AreEqual(expectedRun.ClientStartIndex, actualRun.ClientStartIndex);
            Assert.AreEqual(expectedRun.Data.StartTime, actualRun.Data.StartTime);

            Assert.AreEqual(expectedRun.SlotRuns.Count, actualRun.SlotRuns.Count);
            foreach (int slotRunKey in expectedRun.SlotRuns.Keys)
            {
                var expectedSlotRun = expectedRun.SlotRuns[slotRunKey];
                var actualSlotRun = actualRun.SlotRuns[slotRunKey];
                Assert.AreEqual(expectedSlotRun.Data.CompletedUnits, actualSlotRun.Data.CompletedUnits);
                Assert.AreEqual(expectedSlotRun.Data.FailedUnits, actualSlotRun.Data.FailedUnits);

                Assert.AreEqual(expectedSlotRun.UnitRuns.Count, actualSlotRun.UnitRuns.Count);
                for (int i = 0; i < expectedSlotRun.UnitRuns.Count; i++)
                {
                    var expectedUnitRun = expectedSlotRun.UnitRuns.ElementAt(i);
                    var actualUnitRun = actualSlotRun.UnitRuns.ElementAt(i);
                    Assert.AreEqual(expectedUnitRun.QueueIndex, actualUnitRun.QueueIndex);
                    Assert.AreEqual(expectedUnitRun.StartIndex, actualUnitRun.StartIndex);
                    Assert.AreEqual(expectedUnitRun.EndIndex, actualUnitRun.EndIndex);
                    if (assertUnitRunData)
                    {
                        Assert.AreEqual(expectedUnitRun.Data.UnitStartTimeStamp, actualUnitRun.Data.UnitStartTimeStamp);
                        Assert.AreEqual(expectedUnitRun.Data.CoreVersion, actualUnitRun.Data.CoreVersion);
                        Assert.AreEqual(expectedUnitRun.Data.FramesObserved, actualUnitRun.Data.FramesObserved);
                        Assert.AreEqual(expectedUnitRun.Data.ProjectID, actualUnitRun.Data.ProjectID);
                        Assert.AreEqual(expectedUnitRun.Data.ProjectRun, actualUnitRun.Data.ProjectRun);
                        Assert.AreEqual(expectedUnitRun.Data.ProjectClone, actualUnitRun.Data.ProjectClone);
                        Assert.AreEqual(expectedUnitRun.Data.ProjectGen, actualUnitRun.Data.ProjectGen);
                        Assert.AreEqual(expectedUnitRun.Data.WorkUnitResult, actualUnitRun.Data.WorkUnitResult);
                        if (expectedUnitRun.Data.Frames != null && actualUnitRun.Data.Frames != null)
                        {
                            Assert.AreEqual(expectedUnitRun.Data.Frames.Count, actualUnitRun.Data.Frames.Count);
                            foreach (int frameDataKey in expectedUnitRun.Data.Frames.Keys)
                            {
                                var expectedFrames = expectedUnitRun.Data.Frames[frameDataKey];
                                var actualFrames = actualUnitRun.Data.Frames[frameDataKey];
                                Assert.AreEqual(expectedFrames.ID, actualFrames.ID);
                                Assert.AreEqual(expectedFrames.RawFramesComplete, actualFrames.RawFramesComplete);
                                Assert.AreEqual(expectedFrames.RawFramesTotal, actualFrames.RawFramesTotal);
                                Assert.AreEqual(expectedFrames.TimeStamp, actualFrames.TimeStamp);
                                Assert.AreEqual(expectedFrames.Duration, actualFrames.Duration);
                            }
                        }
                    }
                }
            }
        }
    }
}


using System.Collections.Generic;

using NUnit.Framework;

using HFM.Log.Internal;

namespace HFM.Log
{
    [TestFixture]
    public class FahClientUnitRunDataTests
    {
        [Test]
        public void FahClientUnitRunData_CopyConstructor_Test()
        {
            // Arrange
            var data = new FahClientUnitRunData
            {
                FramesObserved = 50,
                CoreVersion = "1.23",
                ProjectID = 1,
                ProjectRun = 2,
                ProjectClone = 3,
                ProjectGen = 4,
                WorkUnitResult = WorkUnitResult.FINISHED_UNIT,
                Frames = new Dictionary<int, LogLineFrameData> { { 1, new LogLineFrameData { ID = 1 } } }
            };
            // Act
            var copy = new FahClientUnitRunData(data);
            // Assert
            Assert.AreEqual(data.FramesObserved, copy.FramesObserved);
            Assert.AreEqual(data.CoreVersion, copy.CoreVersion);
            Assert.AreEqual(data.ProjectID, copy.ProjectID);
            Assert.AreEqual(data.ProjectRun, copy.ProjectRun);
            Assert.AreEqual(data.ProjectClone, copy.ProjectClone);
            Assert.AreEqual(data.ProjectGen, copy.ProjectGen);
            Assert.AreEqual(data.WorkUnitResult, copy.WorkUnitResult);
            Assert.AreNotSame(data.Frames, copy.Frames);
            Assert.AreNotSame(data.Frames[1], copy.Frames[1]);
            Assert.AreEqual(data.Frames[1].ID, copy.Frames[1].ID);
        }

        [Test]
        public void FahClientUnitRunData_CopyConstructor_OtherIsNew_Test()
        {
            // Act
            var copy = new FahClientUnitRunData(new FahClientUnitRunData());
            // Assert
            Assert.AreEqual(0, copy.FramesObserved);
            Assert.AreEqual(null, copy.CoreVersion);
            Assert.AreEqual(0, copy.ProjectID);
            Assert.AreEqual(0, copy.ProjectRun);
            Assert.AreEqual(0, copy.ProjectClone);
            Assert.AreEqual(0, copy.ProjectGen);
            Assert.AreEqual(null, copy.WorkUnitResult);
            Assert.AreEqual(null, copy.Frames);
        }

        [Test]
        public void FahClientUnitRunData_CopyConstructor_OtherIsNull_Test()
        {
            // Act
            var copy = new FahClientUnitRunData(null);
            // Assert
            Assert.AreEqual(0, copy.FramesObserved);
            Assert.AreEqual(null, copy.CoreVersion);
            Assert.AreEqual(0, copy.ProjectID);
            Assert.AreEqual(0, copy.ProjectRun);
            Assert.AreEqual(0, copy.ProjectClone);
            Assert.AreEqual(0, copy.ProjectGen);
            Assert.AreEqual(null, copy.WorkUnitResult);
            Assert.AreEqual(null, copy.Frames);
        }
    }
}

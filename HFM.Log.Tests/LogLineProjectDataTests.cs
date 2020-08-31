using NUnit.Framework;

namespace HFM.Log
{
    [TestFixture]
    public class LogLineProjectDataTests
    {
        [Test]
        public void LogLineProjectData_CopyConstructor_Test()
        {
            // Arrange
            var data = new LogLineProjectData(1, 2, 3, 4);
            // Act
            var copy = new LogLineProjectData(data);
            // Assert
            Assert.AreEqual(data.ProjectID, copy.ProjectID);
            Assert.AreEqual(data.ProjectRun, copy.ProjectRun);
            Assert.AreEqual(data.ProjectClone, copy.ProjectClone);
            Assert.AreEqual(data.ProjectGen, copy.ProjectGen);
        }

        [Test]
        public void LogLineProjectData_CopyConstructor_OtherIsNull_Test()
        {
            // Act
            var copy = new LogLineProjectData(null);
            // Assert
            Assert.AreEqual(0, copy.ProjectID);
            Assert.AreEqual(0, copy.ProjectRun);
            Assert.AreEqual(0, copy.ProjectClone);
            Assert.AreEqual(0, copy.ProjectGen);
        }
    }
}

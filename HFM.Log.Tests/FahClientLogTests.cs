using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using HFM.Log.Internal;

namespace HFM.Log
{
    [TestFixture]
    public partial class FahClientLogTests
    {
        [Test]
        public async Task FahClientLog_ReadAsync_FromFahLogReader_Test()
        {
            // Arrange
            var log = new FahClientLog();
            using (var textReader = new StreamReader(TestDataReader.ReadStream("Client_v7_10_log.txt")))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                // Act
                await log.ReadAsync(reader);
            }
            // Assert
            Assert.IsTrue(log.ClientRuns.Count > 0);
        }

        [Test]
        public void FahClientLog_Read_FromPath_Test()
        {
            using (var artifacts = new ArtifactFolder())
            {
                var path = artifacts.GetRandomFilePath();
                using (var stream = File.OpenWrite(path))
                {
                    TestDataReader.ReadStream("Client_v7_10_log.txt").CopyTo(stream);
                }
                
                var log = FahClientLog.Read(path);
                Assert.IsTrue(log.ClientRuns.Count > 0);
            }
        }

        [Test]
        public async Task FahClientLog_ReadAsync_FromPath_Test()
        {
            using (var artifacts = new ArtifactFolder())
            {
                var path = artifacts.GetRandomFilePath();
                using (var stream = File.OpenWrite(path))
                {
                    await TestDataReader.ReadStream("Client_v7_10_log.txt").CopyToAsync(stream);
                }
                
                var log = await FahClientLog.ReadAsync(path);
                Assert.IsTrue(log.ClientRuns.Count > 0);
            }
        }

        [Test]
        public async Task FahClientLog_ReadAsync_FromStream_Test()
        {
            var log = await FahClientLog.ReadAsync(TestDataReader.ReadStream("Client_v7_10_log.txt"));
            Assert.IsTrue(log.ClientRuns.Count > 0);
        }

        [Test]
        public void FahClientLog_Clear_Test()
        {
            // Arrange
            var log = new FahClientLog();
            using (var textReader = new StreamReader(TestDataReader.ReadStream("Client_v7_10_log.txt")))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                log.Read(reader);
            }
            Assert.IsTrue(log.ClientRuns.Count > 0);
            // Act
            log.Clear();
            // Assert
            Assert.AreEqual(0, log.ClientRuns.Count);
        }

        // ReSharper disable InconsistentNaming

        [Test]
        public void FahClientLog_Read_Client_v7_BAD_FRAME_CHECKSUM_Test()
        {
            // Act
            var fahLog = FahClientLog.Read(TestDataReader.ReadStream("Client_v7_BAD_FRAME_CHECKSUM_log.txt"));
            // Assert
            Assert.AreEqual(1, fahLog.ClientRuns.Count);
            var clientRun = fahLog.ClientRuns.First();
            Assert.AreEqual(1, clientRun.SlotRuns.Count);
            var slotRun = clientRun.SlotRuns[1];
            Assert.AreEqual(2, slotRun.UnitRuns.Count);
            Assert.AreEqual(1, slotRun.Data.FailedUnits);
            var unitRun = slotRun.UnitRuns[0];
            Assert.AreEqual(0, unitRun.Data.Frames.Count);
            Assert.AreEqual(0, unitRun.Data.FramesObserved);
            Assert.AreEqual(WorkUnitResult.BAD_FRAME_CHECKSUM, unitRun.Data.WorkUnitResult);
        }

        [Test]
        public void FahClientLog_Read_Client_v7_CORE_RESTART_at_zero_Test()
        {
            // Act
            var fahLog = FahClientLog.Read(TestDataReader.ReadStream("Client_v7_CORE_RESTART_at_zero_log.txt"));
            // Assert
            Assert.AreEqual(1, fahLog.ClientRuns.Count);
            var clientRun = fahLog.ClientRuns.First();
            Assert.AreEqual(1, clientRun.SlotRuns.Count);
            var slotRun = clientRun.SlotRuns[0];
            Assert.AreEqual(3, slotRun.UnitRuns.Count);
            var unitRun = slotRun.UnitRuns[0];
            Assert.AreEqual(101, unitRun.Data.Frames.Count);
            Assert.AreEqual(101, unitRun.Data.FramesObserved);
            Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.Data.WorkUnitResult);
        }

        [Test]
        public void FahClientLog_Read_Client_v7_CORE_RESTART_Test()
        {
            // Act
            var fahLog = FahClientLog.Read(TestDataReader.ReadStream("Client_v7_CORE_RESTART_log.txt"));
            // Assert
            Assert.AreEqual(1, fahLog.ClientRuns.Count);
            var clientRun = fahLog.ClientRuns.First();
            Assert.AreEqual(1, clientRun.SlotRuns.Count);
            var slotRun = clientRun.SlotRuns[0];
            Assert.AreEqual(3, slotRun.UnitRuns.Count);
            var unitRun = slotRun.UnitRuns[0];
            Assert.AreEqual(66, unitRun.Data.Frames.Count);
            Assert.AreEqual(66, unitRun.Data.FramesObserved);
            Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.Data.WorkUnitResult);
        }

        [Test]
        public void FahClientLog_Read_Client_v7_fr_FR_Test()
        {
            // Act
            var fahLog = FahClientLog.Read(TestDataReader.ReadStream("Client_v7_fr-FR_log.txt"));
            // Assert
            Assert.AreEqual(1, fahLog.ClientRuns.Count);
            var clientRun = fahLog.ClientRuns.First();
            Assert.AreEqual(2, clientRun.SlotRuns.Count);
            var slotRun0 = clientRun.SlotRuns[0];
            Assert.AreEqual(1, slotRun0.UnitRuns.Count);
            var slotRun1 = clientRun.SlotRuns[1];
            Assert.AreEqual(1, slotRun1.UnitRuns.Count);
        }

        [Test]
        public void FahClientLog_Read_Client_v7_INTERRUPTED_Test()
        {
            // Act
            var fahLog = FahClientLog.Read(TestDataReader.ReadStream("Client_v7_INTERRUPTED_log.txt"));
            // Assert
            Assert.AreEqual(1, fahLog.ClientRuns.Count);

            var clientRun = fahLog.ClientRuns.First();
            Assert.AreEqual(2, clientRun.SlotRuns.Count);
            
            var slotRun0 = clientRun.SlotRuns[0];
            Assert.AreEqual(5, slotRun0.UnitRuns.Count);

            var interruptedUnitRun = slotRun0.UnitRuns[1];
            Assert.AreEqual(84, interruptedUnitRun.Data.FramesObserved);

            var interruptedFrames = interruptedUnitRun.Data.Frames;
            Assert.AreNotEqual(TimeSpan.Zero, interruptedFrames[16].Duration);
            Assert.AreEqual(TimeSpan.Zero, interruptedFrames[17].Duration);
            Assert.AreNotEqual(TimeSpan.Zero, interruptedFrames[18].Duration);
        }
    }

    // ReSharper restore InconsistentNaming
}

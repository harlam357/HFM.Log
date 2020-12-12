using NUnit.Framework;

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HFM.Log
{
    [TestFixture]
    public class FahClientLogReadAndGetRunDataInParallelTests
    {
        [Test]
        public void FahClientLog_Read_Client_v7_16_Test()
        {
            var log = new FahClientLog();
            var stream = TestDataReader.ReadStream("Client_v7_16_log.txt");

            var readTask = Task.Run(() =>
            {
                log.Read(new FahClientLogTextReader(new StreamReader(stream)));
                Thread.Sleep(100);
            });
            var runDataTask = Task.Run(() =>
            {
                int i = 0;
                while (i++ < 20)
                {
                    var clientRun = log.ClientRuns.FirstOrDefault();
                    var clientRunData = clientRun?.Data;
                    Console.WriteLine("StartTime: " + clientRunData?.StartTime);

                    if (clientRun != null)
                    {
                        var slotKeys = clientRun.SlotRuns.Keys.ToList();
                        foreach (var slotKey in slotKeys)
                        {
                            var slotRun = clientRun.SlotRuns[slotKey];
                            var slotRunData = slotRun.Data;
                            Console.WriteLine("CompletedUnits: " + slotRunData.CompletedUnits);
                            Console.WriteLine("FailedUnits: " + slotRunData.FailedUnits);

                            var unitRun = slotRun.UnitRuns.LastOrDefault();
                            var unitRunData = unitRun?.Data;
                            Console.WriteLine("ProjectID: " + unitRunData?.ProjectID);
                        }
                    }

                    Thread.Sleep(10);
                }
            });

            Task.WaitAll(readTask, runDataTask);
        }
    }
}

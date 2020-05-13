
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

namespace HFM.Log
{
    [TestFixture]
    public class FahClientLogTextReaderTests
    {
        [Test]
        public void FahClientLogTextReader_ReadLine_Test()
        {
            using (var textReader = new StreamReader(TestDataReader.ReadStream("Client_v7_10_log.txt")))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                LogLine logLine;
                while ((logLine = reader.ReadLine()) != null)
                {
                    Assert.IsNotNull(logLine);
                    Debug.WriteLine(logLine);
                }
            }
        }

        [Test]
        public async Task FahClientLogTextReader_ReadLineAsync_Test()
        {
            using (var textReader = new StreamReader(TestDataReader.ReadStream("Client_v7_10_log.txt")))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                LogLine logLine;
                while ((logLine = await reader.ReadLineAsync()) != null)
                {
                    Assert.IsNotNull(logLine);
                    Debug.WriteLine(logLine);
                }
            }
        }

        [Test]
        public void FahClientLogTextReader_ReadLine_FromLogIncludingHexCharacterRepresentations_Test()
        {
            using (var textReader = new StreamReader(TestDataReader.ReadStream("Client_v7_fr-FR_log.txt")))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                LogLine logLine;
                while ((logLine = reader.ReadLine()) != null)
                {
                    Assert.IsNotNull(logLine);
                    Debug.WriteLine(logLine);
                }
            }
        }
    }
}

using System.Reflection;
using System.Text;

namespace HFM.Log
{
    internal static class TestDataReader
    {
        internal static string ReadString(string resourceName)
        {
            using var reader = new StreamReader(ReadStream(resourceName));
            return reader.ReadToEnd();
        }

        internal static StringBuilder ReadStringBuilder(string resourceName) =>
            GetResourceStringBuilder(resourceName);

        private const string TestDataNamespace = "HFM.Log.TestData";

        internal static Stream ReadStream(string resourceName) =>
            Assembly.GetExecutingAssembly().GetManifestResourceStream($"{TestDataNamespace}.{resourceName}");

        private static StringBuilder GetResourceStringBuilder(string resourceName)
        {
            // build StringBuilder from Stream with no intermediate heap allocations
            // https://docs.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay

            var stream = ReadStream(resourceName);
            var result = new StringBuilder((int)stream.Length);
            Span<char> buffer = stackalloc char[128];

            using var reader = new StreamReader(stream);
            int bytesRead;
            while ((bytesRead = reader.ReadBlock(buffer)) > 0)
            {
                result.Append(buffer[..bytesRead]);
            }

            return result;
        }
    }
}

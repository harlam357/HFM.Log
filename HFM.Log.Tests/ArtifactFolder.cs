namespace HFM.Log
{
    internal sealed class ArtifactFolder : IDisposable
    {
        public string Path { get; }

        public ArtifactFolder() : this(Environment.CurrentDirectory)
        {

        }

        public ArtifactFolder(string basePath)
        {
            Path = System.IO.Path.Combine(basePath, System.IO.Path.GetRandomFileName());
            Directory.CreateDirectory(Path);
        }

        public string GetRandomFilePath()
        {
            return System.IO.Path.Combine(Path, System.IO.Path.GetRandomFileName());
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, true);
                }
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                // do nothing
            }
        }
    }
}

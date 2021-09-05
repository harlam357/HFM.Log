namespace HFM.Log
{
    /// <summary>
    /// Resolves the <see cref="LogLineType"/> based on the contents of a string line.
    /// </summary>
    public abstract class LogLineTypeResolver
    {
        /// <summary>
        /// Returns a <see cref="LogLineType"/> value if the line represents a known log line type; otherwise, returns <see cref="LogLineType.None"/>.
        /// </summary>
        public LogLineType Resolve(string line)
        {
            return OnResolveLogLineType(line);
        }

        /// <summary>
        /// Implement this method in a derived type and return a <see cref="LogLineType"/> value based on the contents of the string line.
        /// </summary>
        protected abstract LogLineType OnResolveLogLineType(string line);
    }
}

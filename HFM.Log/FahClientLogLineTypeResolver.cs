using System.Text.RegularExpressions;

namespace HFM.Log
{
    /// <summary>
    /// Identifies the log line type of FahClient client log lines.
    /// </summary>
    public class FahClientLogLineTypeResolver : LogLineTypeResolver
    {
        /// <summary>
        /// Gets a singleton instance of the <see cref="FahClientLogLineTypeResolver"/> class.
        /// </summary>
        public static FahClientLogLineTypeResolver Instance { get; } = new FahClientLogLineTypeResolver();

        /// <summary>
        /// Contains logic to identify the log line type of FahClient client log lines.
        /// </summary>
        protected override LogLineType OnResolveLogLineType(string line)
        {
            // ** Most frequent **
            if (line.Contains(":Completed ")) return LogLineType.WorkUnitFrame;

            // ** Per work unit **
            if (line.Contains(":Sending unit results:")) return LogLineType.ClientSendWorkToServer;
            if (line.Contains(":Requesting new work unit for slot")) return LogLineType.ClientAttemptGetWorkPacket;
            if (line.TrimEnd().EndsWith(":Starting", StringComparison.InvariantCulture)) return LogLineType.WorkUnitWorking;
            if (line.Contains(":Version")) return LogLineType.WorkUnitCoreVersion;
            // Ignore v7 client version information by looking for this pattern beyond index 8 - see TestFiles\Client_v7_14\log.txt for an example
            if (line.IndexOf(":    Version", StringComparison.InvariantCulture) > 8) return LogLineType.WorkUnitCoreVersion;
            if (line.Contains(":Project:")) return LogLineType.WorkUnitProject;
            if (line.Contains(":Folding@home Core Shutdown:")) return LogLineType.WorkUnitCoreShutdown;
            if (Regex.IsMatch(line, "FahCore returned: ")) return LogLineType.WorkUnitCoreReturn;
            if (line.Contains(":Cleaning up")) return LogLineType.WorkUnitCleaningUp;
            if (line.Contains(":Too many errors, failing")) return LogLineType.WorkUnitTooManyErrors;
            if (Regex.IsMatch(line, @":  Using \w+(\son\s[\w\s]+)? and gpu \d+")) return LogLineType.WorkUnitPlatform;

            // ** Least frequent **
            if (line.StartsWith("*********************** Log Started", StringComparison.Ordinal)) return LogLineType.LogOpen;

            return LogLineType.None;
        }
    }
}

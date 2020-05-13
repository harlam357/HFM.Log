
using System;

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
            var logLineType = Internal.CommonLogLineTypeResolver.ResolveLogLineType(line);
            if (logLineType != LogLineType.None)
            {
                return logLineType;
            }

            if (line.Contains("*********************** Log Started")) return LogLineType.LogOpen;
            if (line.Contains(":Sending unit results:")) return LogLineType.ClientSendWorkToServer;
            if (line.Contains(":Requesting new work unit for slot")) return LogLineType.ClientAttemptGetWorkPacket;
            if (line.Trim().EndsWith(":Starting", StringComparison.InvariantCulture)) return LogLineType.WorkUnitWorking;
            // Appears to be for v7.1.38 and previous only
            if (line.Contains(":Starting Unit")) return LogLineType.WorkUnitWorking;
            if (line.Contains(":*------------------------------*")) return LogLineType.WorkUnitCoreStart;
            if (line.Contains(":Version")) return LogLineType.WorkUnitCoreVersion;
            // Ignore v7 client version information by looking for this pattern beyond index 8 - see TestFiles\Client_v7_14\log.txt for an example
            if (line.IndexOf(":    Version", StringComparison.InvariantCulture) > 8) return LogLineType.WorkUnitCoreVersion;
            if (line.Contains(":Project:")) return LogLineType.WorkUnitProject;
            if (line.Contains(":Completed ")) return LogLineType.WorkUnitFrame;
            if (line.Contains(":Folding@home Core Shutdown:")) return LogLineType.WorkUnitCoreShutdown;
            if (System.Text.RegularExpressions.Regex.IsMatch(line, "FahCore returned: ")) return LogLineType.WorkUnitCoreReturn;
            // Appears to be for v7.1.38 and previous only
            if (System.Text.RegularExpressions.Regex.IsMatch(line, "FahCore, running Unit \\d{2}, returned: ")) return LogLineType.WorkUnitCoreReturn;
            if (line.Contains(":Cleaning up")) return LogLineType.WorkUnitCleaningUp;
            // Appears to be for v7.1.38 and previous only
            if (line.Contains(":Cleaning up Unit")) return LogLineType.WorkUnitCleaningUp;
            if (line.Contains(":Too many errors, failing")) return LogLineType.WorkUnitTooManyErrors;

            return LogLineType.None;
        }
    }
}
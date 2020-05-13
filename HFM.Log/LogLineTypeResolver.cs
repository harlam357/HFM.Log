
using System;

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

    namespace Internal
    {
        internal static class CommonLogLineTypeResolver
        {
            internal static LogLineType ResolveLogLineType(string line)
            {
                return IsLineTypeWorkUnitRunning(line) ? LogLineType.WorkUnitRunning : LogLineType.None;
            }

            private static bool IsLineTypeWorkUnitRunning(string line)
            {
                if (line.Contains("Preparing to commence simulation")) return true;
                if (line.Contains("Called DecompressByteArray")) return true;
                if (line.Contains("- Digital signature verified")) return true;
                if (line.Contains("Entering M.D.")) return true;
                return false;
            }
        }
    }

    namespace FahClient
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
}

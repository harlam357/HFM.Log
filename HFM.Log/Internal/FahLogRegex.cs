using System.Text.RegularExpressions;

namespace HFM.Log.Internal
{
    internal static class FahLogRegex
    {
        private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

        internal static class Common
        {
            internal static readonly Regex TimeStampRegex =
               new Regex("\\[?(?<Timestamp>\\d{2}:\\d{2}:\\d{2})[\\]|:]", Options);

            /// <summary>
            /// Regular Expression to match Work Unit Project string.
            /// </summary>
            internal static readonly Regex ProjectIdRegex =
               new Regex("\\[?\\d{2}:\\d{2}:\\d{2}[\\]|:].*Project: (?<ProjectNumber>\\d+) \\(Run (?<Run>\\d+), Clone (?<Clone>\\d+), Gen (?<Gen>\\d+)\\)", Options);

            /// <summary>
            /// Regular Expression to match Core Version string.
            /// </summary>
            internal static readonly Regex CoreVersionRegex =
               new Regex("\\[?\\d{2}:\\d{2}:\\d{2}[\\]|:].*Version:?\\s+(?<CoreVer>\\d+\\.\\d+(\\.\\d+)?)", Options);

            /// <summary>
            /// Regular Expression to match Standard and SMP Clients Frame Completion Lines (Gromacs Style).
            /// </summary>
            internal static readonly Regex FramesCompletedRegex =
               new Regex("\\[?(?<Timestamp>\\d{2}:\\d{2}:\\d{2})[\\]|:].*Completed (?<Completed>.*) out of (?<Total>.*) steps {1,2}\\((?<Percent>.*)%\\)", Options);

            /// <summary>
            /// Regular Expression to match GPU2 Client Frame Completion Lines
            /// </summary>
            internal static readonly Regex FramesCompletedGpuRegex =
               new Regex("\\[?(?<Timestamp>\\d{2}:\\d{2}:\\d{2})[\\]|:].*Completed (?<Percent>[0-9]{1,3})%", Options);

            /// <summary>
            /// Regular Expression to match Machine ID string.
            /// </summary>
            internal static readonly Regex CoreShutdownRegex =
               new Regex("\\[?\\d{2}:\\d{2}:\\d{2}[\\]|:].*Folding@home Core Shutdown: (?<UnitResult>.*)", Options);
        }

        internal static class FahClient
        {
            internal static readonly Regex LogOpenRegex =
               new Regex(@"\*{23} Log Started (?<StartTime>.+) \*+", Options);

            // a copy of this regex exists in HFM.Core.DataTypes
            internal static readonly Regex WorkUnitRunningRegex =
               new Regex("(?<Timestamp>\\d{2}:\\d{2}:\\d{2}):(.+:)?WU(?<UnitIndex>\\d{2}):FS(?<FoldingSlot>\\d{2}):.*", Options);

            internal static readonly Regex WorkUnitCoreReturnRegex =
               new Regex("\\d{2}:\\d{2}:\\d{2}:(.+:)?WU\\d{2}:FS\\d{2}:FahCore returned: (?<UnitResult>\\w+)", Options);
        }
    }
}

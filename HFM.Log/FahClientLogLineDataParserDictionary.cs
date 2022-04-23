using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using HFM.Log.Internal;

namespace HFM.Log
{
    /// <summary>
    /// Represents a collection of <see cref="LogLineType"/> / <see cref="LogLineDataParserFunction"/> pairs used to parse data from FahClient client log lines.
    /// </summary>
    [Serializable]
    public class FahClientLogLineDataParserDictionary : LogLineDataParserDictionary
    {
        /// <summary>
        /// Gets a singleton instance of the <see cref="FahClientLogLineDataParserDictionary"/> class.
        /// </summary>
        public static FahClientLogLineDataParserDictionary Instance { get; } = new FahClientLogLineDataParserDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientLogLineDataParserDictionary"/> class.
        /// </summary>
        public FahClientLogLineDataParserDictionary()
        {
            CommonLogLineParser.AddToDictionary(this);

            Add(LogLineType.LogOpen, ParseLogOpen);
            Add(LogLineType.WorkUnitCoreVersion, ParseWorkUnitCoreVersion);
            Add(LogLineType.WorkUnitCoreReturn, ParseWorkUnitCoreReturn);
            Add(LogLineType.WorkUnitPlatform, ParseWorkUnitPlatform);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientLogLineDataParserDictionary"/> class.
        /// </summary>
        protected FahClientLogLineDataParserDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        internal static object ParseLogOpen(LogLine logLine)
        {
            Match match;
            if ((match = FahLogRegex.FahClient.LogOpenRegex.Match(logLine.Raw)).Success)
            {
                string startTime = match.Result("${StartTime}");
                // Similar code found in HFM.Client DateTimeConverter
                // ISO 8601
                if (DateTime.TryParse(startTime, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var value))
                {
                    return value;
                }

                // custom format for older v7 clients
                if (DateTime.TryParseExact(startTime, "dd/MMM/yyyy-HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out value))
                {
                    return value;
                }
            }
            return null;
        }

        internal static object ParseWorkUnitCoreVersion(LogLine logLine)
        {
            Match match;
            return (match = FahLogRegex.Common.CoreVersionRegex.Match(logLine.Raw)).Success
                ? match.Groups["CoreVer"].Value.Trim()
                : null;
        }

        internal static object ParseWorkUnitCoreReturn(LogLine logLine)
        {
            Match match;
            return (match = FahLogRegex.FahClient.WorkUnitCoreReturnRegex.Match(logLine.Raw)).Success
                ? match.Groups["UnitResult"].Value
                : null;
        }

        internal static object ParseWorkUnitPlatform(LogLine logLine)
        {
            Match match;
            return (match = FahLogRegex.FahClient.WorkUnitPlatformRegex.Match(logLine.Raw)).Success
                ? match.Groups["Platform"].Value
                : null;
        }
    }
}

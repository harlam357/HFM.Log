
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using HFM.Log.Internal;

namespace HFM.Log
{
    namespace Internal
    {
        internal static class CommonLogLineParser
        {
            internal static void AddToDictionary(IDictionary<LogLineType, LogLineDataParserFunction> dictionary)
            {
                dictionary.Add(LogLineType.WorkUnitProject, ParseWorkUnitProject);
                dictionary.Add(LogLineType.WorkUnitFrame, ParseWorkUnitFrame);
                dictionary.Add(LogLineType.WorkUnitCoreShutdown, ParseWorkUnitCoreShutdown);
            }

            internal static WorkUnitProjectData ParseWorkUnitProject(LogLine logLine)
            {
                Match projectIdMatch;
                if ((projectIdMatch = FahLogRegex.Common.ProjectIdRegex.Match(logLine.Raw)).Success)
                {
                    return new WorkUnitProjectData(
                       Int32.Parse(projectIdMatch.Groups["ProjectNumber"].Value, CultureInfo.InvariantCulture),
                       Int32.Parse(projectIdMatch.Groups["Run"].Value, CultureInfo.InvariantCulture),
                       Int32.Parse(projectIdMatch.Groups["Clone"].Value, CultureInfo.InvariantCulture),
                       Int32.Parse(projectIdMatch.Groups["Gen"].Value, CultureInfo.InvariantCulture)
                    );
                }

                return null;
            }

            internal static WorkUnitFrameData ParseWorkUnitFrame(LogLine logLine)
            {
                WorkUnitFrameData frameData = GetFrameData(logLine);
                if (frameData != null)
                {
                    return frameData;
                }

                frameData = GetGpuFrameData(logLine);
                return frameData;
            }

            private static WorkUnitFrameData GetFrameData(LogLine logLine)
            {
                Debug.Assert(logLine != null);

                Match framesCompleted = FahLogRegex.Common.FramesCompletedRegex.Match(logLine.Raw);
                if (framesCompleted.Success)
                {
                    var frame = new WorkUnitFrameData();

                    if (Int32.TryParse(framesCompleted.Result("${Completed}"), out var result))
                    {
                        frame.RawFramesComplete = result;
                    }
                    else
                    {
                        return null;
                    }

                    if (Int32.TryParse(framesCompleted.Result("${Total}"), out result))
                    {
                        frame.RawFramesTotal = result;
                    }
                    else
                    {
                        return null;
                    }

                    string percentString = framesCompleted.Result("${Percent}");

                    Match match = FahLogRegex.Common.ProgressPercentRegex.Match(percentString);

                    int framePercent;
                    if (match.Success)
                    {
                        framePercent = Int32.Parse(match.Result("${Percent}"), CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        return null;
                    }

                    // Validate the steps are in tolerance with the detected frame percent - Issue 98
                    double calculatedPercent = ((double)frame.RawFramesComplete / frame.RawFramesTotal) * 100;
                    // ex. [00:19:40] Completed 82499 out of 250000 steps  (33%) - Would Validate
                    //     [00:19:40] Completed 82750 out of 250000 steps  (33%) - Would Validate
                    // 10% frame step tolerance. In the example the completed must be within 250 steps.
                    if (Math.Abs(calculatedPercent - framePercent) <= 0.1)
                    {
                        if (logLine.TimeStamp != null)
                        {
                            frame.TimeStamp = logLine.TimeStamp.Value;
                        }
                        frame.ID = framePercent;

                        return frame;
                    }

                    return null;
                }

                return null;
            }

            private static WorkUnitFrameData GetGpuFrameData(LogLine logLine)
            {
                Debug.Assert(logLine != null);

                Match framesCompletedGpu = FahLogRegex.Common.FramesCompletedGpuRegex.Match(logLine.Raw);
                if (framesCompletedGpu.Success)
                {
                    var frame = new WorkUnitFrameData();

                    frame.RawFramesComplete = Int32.Parse(framesCompletedGpu.Result("${Percent}"), CultureInfo.InvariantCulture);
                    frame.RawFramesTotal = 100; //Instance.CurrentProtein.Frames
                                                // I get this from the project data but what's the point. 100% is 100%.

                    if (logLine.TimeStamp != null)
                    {
                        frame.TimeStamp = logLine.TimeStamp.Value;
                    }
                    frame.ID = frame.RawFramesComplete;

                    return frame;
                }

                return null;
            }

            internal static string ParseWorkUnitCoreShutdown(LogLine logLine)
            {
                Match coreShutdownMatch;
                if ((coreShutdownMatch = FahLogRegex.Common.CoreShutdownRegex.Match(logLine.Raw)).Success)
                {
                    // remove any carriage returns from fahclient log lines - 12/30/11
                    string unitResultValue = coreShutdownMatch.Result("${UnitResult}").Replace("\r", String.Empty);
                    return unitResultValue;
                }

                return null;
            }
        }
    }

    /// <summary>
    /// Defines a function used to parse data from a log line.
    /// </summary>
    /// <param name="logLine">The log line to parse.</param>
    /// <returns>An object representing the data parsed from the log line.</returns>
    public delegate object LogLineDataParserFunction(LogLine logLine);

    /// <summary>
    /// Represents a read-only collection of <see cref="LogLineType"/> / <see cref="LogLineDataParserFunction"/> pairs used to parse data from client log lines.
    /// </summary>
    [Serializable]
    public abstract class LogLineDataParserDictionary : Dictionary<LogLineType, LogLineDataParserFunction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogLineDataParserDictionary"/> class.
        /// </summary>
        protected LogLineDataParserDictionary()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogLineDataParserDictionary"/> class.
        /// </summary>
        protected LogLineDataParserDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }
    }

    namespace FahClient
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
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="FahClientLogLineDataParserDictionary"/> class.
            /// </summary>
            protected FahClientLogLineDataParserDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            
            }

            internal static object ParseLogOpen(LogLine logLine)
            {
                Match logOpenMatch;
                if ((logOpenMatch = FahLogRegex.FahClient.LogOpenRegex.Match(logLine.Raw)).Success)
                {
                    string startTime = logOpenMatch.Result("${StartTime}");
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
                Match coreVersionMatch;
                if ((coreVersionMatch = FahLogRegex.Common.CoreVersionRegex.Match(logLine.Raw)).Success)
                {
                    return coreVersionMatch.Groups["CoreVer"].Value.Trim();
                }
                return null;
            }

            internal static string ParseWorkUnitCoreReturn(LogLine logLine)
            {
                Match coreReturnMatch;
                if ((coreReturnMatch = FahLogRegex.FahClient.WorkUnitCoreReturnRegex.Match(logLine.Raw)).Success)
                {
                    return coreReturnMatch.Groups["UnitResult"].Value;
                }
                return null;
            }
        }
    }
}

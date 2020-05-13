
using System.IO;

namespace HFM.Log
{
    /// <summary>
    /// Represents a reader that reads Folding@Home log line text data from a v7 or newer client.
    /// </summary>
    public class FahClientLogTextReader : FahLogTextReader
    {
        /// <summary>
        /// Gets the <see cref="LogLineTypeResolver"/> used to resolve the <see cref="LogLineType"/> from a log line.
        /// </summary>
        protected LogLineTypeResolver LogLineTypeResolver { get; }

        /// <summary>
        /// Gets the <see cref="LogLineDataParserDictionary"/> that provides parsing functions for each <see cref="LogLineType"/>.
        /// </summary>
        protected LogLineDataParserDictionary LogLineDataParserDictionary { get; }

        /// <summary>
        /// Gets the <see cref="LogLineTimeStampParser"/> used to parse time stamp information from a log line.
        /// </summary>
        protected LogLineTimeStampParser LogLineTimeStampParser { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientLogTextReader"/> class.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> that provides line data as a string.</param>
        public FahClientLogTextReader(TextReader textReader)
            : this(textReader, null, null, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FahClientLogTextReader"/> class.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> that provides line data as a string.</param>
        /// <param name="logLineTypeResolver">The <see cref="LogLineTypeResolver"/> used to resolve the <see cref="LogLineType"/> from a log line.</param>
        /// <param name="logLineDataParserDictionary">The <see cref="LogLineDataParserDictionary"/> that provides parsing functions for each <see cref="LogLineType"/>.</param>
        /// <param name="logLineTimeStampParser">The <see cref="LogLineTimeStampParser"/> used to parse time stamp information from a log line.</param>
        protected FahClientLogTextReader(TextReader textReader,
            LogLineTypeResolver logLineTypeResolver,
            LogLineDataParserDictionary logLineDataParserDictionary,
            LogLineTimeStampParser logLineTimeStampParser)
            : base(textReader)
        {
            LogLineTypeResolver = logLineTypeResolver ?? FahClientLogLineTypeResolver.Instance;
            LogLineDataParserDictionary = logLineDataParserDictionary ?? FahClientLogLineDataParserDictionary.Instance;
            LogLineTimeStampParser = logLineTimeStampParser ?? LogLineTimeStampParser.Instance;
        }

        /// <summary>
        /// Occurs after a line was read from the <see cref="TextReader"/> and returns a new <see cref="LogLine"/> object.
        /// </summary>
        /// <param name="line">The line read from the <see cref="TextReader"/>.</param>
        /// <param name="index">The index of the line read from the <see cref="TextReader"/>.</param>
        /// <returns>A new <see cref="LogLine"/> object from the string line and line index.</returns>
        protected override LogLine OnReadLine(string line, int index)
        {
            LogLineType lineType = LogLineTypeResolver.Resolve(line);
            LogLineTimeStampParserFunction timeStampParser = LogLineTimeStampParser.ParseTimeStamp;
            LogLineDataParserDictionary.TryGetValue(lineType, out LogLineDataParserFunction dataParser);
            return new LazyLogLine(line, index, lineType, timeStampParser, dataParser);
        }
    }
}
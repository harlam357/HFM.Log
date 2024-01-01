using System.Globalization;
using System.Text;

namespace HFM.Log.Tool
{
    public partial class LogLineRichTextBox : RichTextBox
    {
        public object Owner { get; private set; }

        public IReadOnlyCollection<LogLine> LogLines { get; private set; }

        public LogLineRichTextBox()
        {
            InitializeComponent();
        }

        private bool _colorLogFile;

        public bool ColorLogFile
        {
            get => _colorLogFile;
            set
            {
                if (_colorLogFile != value)
                {
                    _colorLogFile = value;
                    SetTextFromLogLines();
                }
            }
        }

        public void SetLogLines(object owner, IReadOnlyCollection<LogLine> logLines)
        {
            if (owner != null && logLines != null && logLines.Count > 0)
            {
                // different owner
                if (!ReferenceEquals(Owner, owner))
                {
                    SetLogLinesInternal(owner, logLines);
                }
                else if (Lines.Length > 0)
                {
                    // get the last text lines from the control and incoming LogLines collection
                    string lastLine = Lines.Last();
                    string lastLogLineText = logLines.LastOrDefault()?.Raw ?? String.Empty;

                    // don't reload ("flicker") if the log appears the same
                    if (lastLine != lastLogLineText)
                    {
                        SetLogLinesInternal(owner, logLines);
                    }
                }
                else
                {
                    SetLogLinesInternal(owner, logLines);
                }
            }
            else
            {
                SetLogLinesInternal(null, null);
            }
        }

        private void SetLogLinesInternal(object owner, IReadOnlyCollection<LogLine> logLines)
        {
            Owner = owner;
            LogLines = logLines;
            SetTextFromLogLines();
        }

        private void SetTextFromLogLines()
        {
            if (LogLines is null)
            {
                Rtf = null;
                Text = "No Log Available";
            }
            else
            {
                if (ColorLogFile)
                {
                    Rtf = LogLineRtf.Build(LogLines);
                }
                else
                {
                    Rtf = null;
                    Lines = LogLines.Select(line => line.Raw.Replace("\r", String.Empty, StringComparison.Ordinal)).ToArray();
                }
            }
        }

        public void ScrollToBottom()
        {
            SelectionStart = TextLength;

            NativeMethods.SendMessage(Handle, NativeMethods.WM_VSCROLL, new IntPtr(NativeMethods.SB_BOTTOM), new IntPtr(0));
        }

        public void ScrollToLine(int lineNumber) =>
            NativeMethods.SendMessage(Handle, NativeMethods.EM_LINESCROLL, new IntPtr(0), new IntPtr(lineNumber));
    }

    public static class LogLineRtf
    {
        public const string Definition = @"{\rtf1\ansi\deff0{\colortbl;\red0\green150\blue0;\red139\green0\blue0;\red255\green140\blue0;\red0\green0\blue255;\red120\green120\blue120;}";

        public static string Build(LogLine line) => String.Format(CultureInfo.InvariantCulture, @"{0}{1}\line", GetLineColor(line), line);

        public static string Build(IEnumerable<LogLine> logLines)
        {
            if (logLines is null)
            {
                return String.Empty;
            }

            // cf1 - Dark Green
            // cf2 - Dark Red
            // cf3 - Dark Orange
            // cf4 - Blue
            // cf5 - Slate Gray

            var sb = new StringBuilder();
            sb.Append(Definition);
            foreach (var line in logLines)
            {
                sb.Append(Build(line));
            }
            sb.Append('}');
            return sb.ToString();
        }

        private static string GetLineColor(LogLine line)
        {
            if (line is null)
            {
                return @"\cf4 ";
            }

            switch (line.LineType)
            {
                case LogLineType.None:
                    return @"\cf5 ";
                case LogLineType.WorkUnitFrame:
                    return @"\cf1 ";
                case LogLineType.WorkUnitCoreShutdown:
                case LogLineType.WorkUnitCoreReturn:
                    return @"\cf2 ";
                default:
                    return line.Data is LogLineDataParserError
                        ? @"\cf3 "
                        : @"\cf4 ";
            }
        }
    }
}


using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace HFM.Log.Tool
{
    public partial class RichTextBoxExt : RichTextBox
    {
        private IList<LogLine> _logLines;

        public RichTextBoxExt()
        {
            InitializeComponent();
        }

        public void SetLogLines(IEnumerable<LogLine> lines, bool highlightLines)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IEnumerable<LogLine>, bool>(SetLogLines), lines, highlightLines);
                return;
            }

            _logLines = lines.ToList();
            HighlightLines(highlightLines);
        }

        public void HighlightLines(bool value)
        {
#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
#endif
            if (value)
            {
                Rtf = BuildRtfString();
            }
            else
            {
                Rtf = null;
                Lines = _logLines.Select(line => line.Raw.Replace("\r", String.Empty)).ToArray();
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("HighlightLines: {0:#,##0} ms", sw.ElapsedMilliseconds);
#endif
        }

        private string BuildRtfString()
        {
            // cf1 - Dark Green
            // cf2 - Dark Red
            // cf3 - Dark Orange
            // cf4 - Blue
            // cf5 - Slate Gray

            var sb = new StringBuilder();
            sb.Append(@"{\rtf1\ansi\deff0{\colortbl;\red0\green150\blue0;\red139\green0\blue0;\red255\green140\blue0;\red0\green0\blue255;\red120\green120\blue120;}");
            foreach (var line in _logLines)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, @"{0}{1}\line", GetLineColor(line), line);
            }
            return sb.ToString();
        }

        private static string GetLineColor(LogLine line)
        {
            if (line.LineType == LogLineType.None)
            {
                return @"\cf5 ";
            }

            if (line.Data is LogLineDataParserError)
            {
                return @"\cf3 ";
            }

            if (line.LineType == LogLineType.WorkUnitFrame)
            {
                return @"\cf1 ";
            }

            if (line.LineType == LogLineType.WorkUnitCoreShutdown ||
                line.LineType == LogLineType.WorkUnitCoreReturn)
            {
                return @"\cf2 ";
            }

            return @"\cf4 ";
        }

        #region Native Scroll Messages (don't call under Mono)

        public void ScrollToLine(int lineNumber)
        {
            NativeMethods.SendMessage(Handle, NativeMethods.EM_LINESCROLL, new IntPtr(0), new IntPtr(lineNumber));
        }

        #endregion
    }
}

using System.Text;
using System.Windows.Forms;
using XpertiumSharp.Core;

namespace Xpertium
{
    public class Logger : IXLogger
    {
        private readonly RichTextBox textBox;

        public XLogLevel LogLevel { get; set; }

        public uint Indent { get; set; }

        public Logger(RichTextBox textBox)
        {
            this.textBox = textBox;
            LogLevel = XLogLevel.Debug;
            Indent = 0;
        }

        public void LogD(string format, params object[] args)
        {
            Log(XLogLevel.Debug, "D:", format, args);
        }

        public void LogE(string format, params object[] args)
        {
            Log(XLogLevel.Error, "E:", format, args);
        }

        public void LogF(string format, params object[] args)
        {
            Log(XLogLevel.Fatal, "F:", format, args);
        }

        public void LogI(string format, params object[] args)
        {
            Log(XLogLevel.Info, "I:", format, args);
        }

        public void LogW(string format, params object[] args)
        {
            Log(XLogLevel.Warning, "W:", format, args);
        }

        public void Clear()
        {
            textBox.Clear();
        }

        private void Log(XLogLevel level, string prefix, string format, params object[] args)
        {
            if ((int)level >= (int)LogLevel)
            {
                var msg = new StringBuilder(prefix)
                    .Append('-', (int)Indent * 4)
                    .AppendFormat(format, args)
                    .AppendLine()
                    .ToString();
                textBox.AppendText(msg);
            }
        }
    }
}

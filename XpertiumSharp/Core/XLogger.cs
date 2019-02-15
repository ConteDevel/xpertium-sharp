using System.Text;

namespace XpertiumSharp.Core
{
    public enum XLogLevel
    {
        Fatal = 0,
        Error,
        Warning,
        Info,
        Debug
    }

    public interface IXLogger
    {
        XLogLevel LogLevel { get; }
        uint Indent { get; set; }
        void LogF(string format, params object[] args);
        void LogE(string format, params object[] args);
        void LogW(string format, params object[] args);
        void LogI(string format, params object[] args);
        void LogD(string format, params object[] args);
        void Clear();
    }

    public class XLogger : IXLogger
    {
        private StringBuilder log = new StringBuilder();

        public XLogLevel LogLevel { get; set; }

        public uint Indent { get; set; }

        public XLogger()
        {
            Indent = 0;
            LogLevel = XLogLevel.Debug;
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
            log.Clear();
        }

        private void Log(XLogLevel level, string prefix, string format, params object[] args)
        {
            if ((int)level >= (int)LogLevel)
            {
                log.Append(prefix);
                log.Append('-', (int)Indent * 4);
                log.AppendFormat(format, args);
                log.AppendLine();
            }
        }

        public override string ToString()
        {
            return log.ToString();
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;


namespace UpdateProgram
{
    public static class Logger
    {
        private static readonly string FilePath = string.Empty;


        static Logger()
        {
            try
            {
                FilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\UpdateProgramErrors.txt";
            }
            catch { }
        }


        public static void Log(string text)
        {
            var callerMethodName = string.Empty;

            try
            {
                callerMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            }
            catch { }

            try
            {
                File.AppendAllText(FilePath, $"[{DateTime.Now}] [{callerMethodName}] {text}{Environment.NewLine}");
            }
            catch { }
        }
    }
}
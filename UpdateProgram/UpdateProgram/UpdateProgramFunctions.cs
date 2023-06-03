using Cassia;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Automation;


namespace UpdateProgram
{
    public static class UpdateProgramFunctions
    {
        private const string ExpertFileDirectoryName = "Experts";


        [DllExport("GetActualizedProgramInfoJson", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static string GetActualizedProgramInfoJson([MarshalAs(UnmanagedType.LPWStr)] string requestUrl)
        {
            string GetResponse()
            {
                string responseJson = string.Empty;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseJson = reader.ReadToEnd();
                }

                return responseJson;
            }

            try
            {
                return GetResponse();
            }
            catch
            {
                try
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    return GetResponse();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message);
                    return string.Empty;
                }
            }
        }


        [DllExport("UpdateProgram", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static bool UpdateProgram([MarshalAs(UnmanagedType.LPWStr)] string fileDownloadingUrl)
        {
            if (string.IsNullOrEmpty(fileDownloadingUrl))
                return false;

            var fullDllDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(fullDllDirectoryPath))
                return false;

            var fullEx4DirectoryInfo = Directory.GetParent(fullDllDirectoryPath);

            if (string.IsNullOrEmpty(fullEx4DirectoryInfo?.FullName))
                return false;

            var fileUri = new Uri(fileDownloadingUrl);
            var fileName = fileUri.Segments?.LastOrDefault()?.Replace("%20", " ");

            if (string.IsNullOrEmpty(fileName))
                return false;

            var fullEx4FilePath = $"{fullEx4DirectoryInfo.FullName}\\{ExpertFileDirectoryName}\\{fileName}";

            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(fileDownloadingUrl, fullEx4FilePath);
                    return true;
                }
                catch
                {
                    try
                    {
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        client.DownloadFile(fileDownloadingUrl, fullEx4FilePath);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.Message);
                        return false;
                    }  
                }
            }
        }


        [DllExport("RestartTerminal", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static void RestartTerminal()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();

                if (currentProcess == null)
                    return;

                var processModuleFileName = currentProcess.MainModule.FileName;

                var processStartInfo = new ProcessStartInfo()
                {
                    Arguments = "/C ping 127.0.0.1 -n 2 && \"" + processModuleFileName + "\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = "cmd.exe"
                };

                Process.Start(processStartInfo);

                currentProcess.Kill();
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }


        [DllExport("CheckRDPConnection", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static bool CheckRDPConnection()
        {
            try
            {
                ITerminalServicesManager manager = new TerminalServicesManager();
                using (ITerminalServer server = manager.GetLocalServer())
                {
                    server.Open();
                    return server.GetSessions().Any(s => s.ConnectionState == ConnectionState.Active);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return false;
            }
        }


        [DllExport("TerminalHide", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static void TerminalHide()
        {
            var currentProcess = Process.GetCurrentProcess();

            if (currentProcess == null)
                return;

            try
            {
                var windowElement = AutomationElement.FromHandle(currentProcess.MainWindowHandle);
                var windowPattern = windowElement.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;

                if (windowPattern.Current.CanMinimize)
                    windowPattern.SetWindowVisualState(WindowVisualState.Minimized);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }


        [DllExport("TerminalShow", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        public static void TerminalShow()
        {
            var currentProcess = Process.GetCurrentProcess();

            if (currentProcess == null)
                return;

            try
            {
                var windowElement = AutomationElement.FromHandle(currentProcess.MainWindowHandle);
                var windowPattern = windowElement.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;

                if (windowPattern.Current.WindowVisualState == WindowVisualState.Minimized)
                    windowPattern.SetWindowVisualState(WindowVisualState.Normal);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }
    }
}
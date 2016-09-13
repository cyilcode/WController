using System;
using System.Diagnostics;
using System.IO;

namespace WCS.MAIN.Globals
{
    public class GlobalHelper
    {
        private const string OSX_IDENTIFIER             = "Darwin";
        private const string LOG_FILE_PATH              = "error.log";

        public void coloredLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = default(ConsoleColor);
        }

        public string execute_shell_command(string command, string args)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = command;
            startInfo.Arguments = args;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            Process commandProcess = Process.Start(startInfo);
            string output = commandProcess.StandardOutput.ReadToEnd();
            if (output != null || output != string.Empty)
                    return output;
            return "";
        }

        public OS getOS()
        {
            PlatformID platform = Environment.OSVersion.Platform;
            if (platform == PlatformID.Win32NT || platform == PlatformID.Win32S)
                return OS.WINDOWS;
            else
            {
                string ret = execute_shell_command("uname", "").Trim();
                if (ret == OSX_IDENTIFIER)
                    return OS.MACOSX;
                else
                    return OS.LINUX;
            }
        }

        // This should stay here but i'm not gonna use this yet.
        public static void log(string log_message)
        {
            var trace = new StackTrace().GetFrame(1).GetMethod();
            string location = string.Format("[{0} / {1}]", trace.DeclaringType.Name, trace.Name);
            string format = string.Format("[{0}] - {1} - on function: {2}",DateTime.Now, log_message, location);
            // TODO: File checking
            File.AppendAllText(LOG_FILE_PATH, format);
        }
    }
}

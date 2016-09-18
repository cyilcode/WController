using System;
using System.Diagnostics;
using System.IO;

namespace WCS.MAIN.Globals
{
    public class GlobalHelper
    {
        private const string OSX_IDENTIFIER             = "Darwin";
        private const string LOG_FILE_PATH              = "error.log";
        private const int    MAX_FILE_LENGTH            = 1000000;

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

        public static void log(string log_message)
        {
            if (File.Exists(LOG_FILE_PATH))
            {
                var file = new FileInfo(LOG_FILE_PATH);
                /*
                    Basically if our file reaches 1 megabytes or goes over it, we should archive it
                    to keep it usable. This is not really needed but nice to have a feature like this.
                 */
                if (file.Length >= MAX_FILE_LENGTH)
                {
                    string[] explodeString = LOG_FILE_PATH.Split(':');
                    string archivedPath = string.Format("{0}-ARCHIVED-{1}.{2}",  // explodeString[0] = filename, explodeString[1] = extension. (error and .log)
                                                        explodeString[0],
                                                        DateTime.Now.ToShortDateString(),
                                                        explodeString[1]);
                    File.Move(LOG_FILE_PATH, archivedPath); // Rename the source.
                }
            }
            var trace = new StackTrace().GetFrame(1).GetMethod();
            string location = string.Format("[{0} / {1}]", trace.DeclaringType.Name, trace.Name);
            string format = string.Format("[{0}] - {1} - on function: {2}\n",DateTime.Now, log_message, location);          
            File.AppendAllText(LOG_FILE_PATH, format);
        }
    }
}

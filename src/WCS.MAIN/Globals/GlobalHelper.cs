using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using WCS.MAIN.Functions;
using WCS.MAIN.Functions.Handlers.AudioHandlers;
using WCS.MAIN.Functions.Handlers.InputHandlers;

namespace WCS.MAIN.Globals
{
    public class GlobalHelper
    {
        private const string OSX_IDENTIFIER             = "Darwin";
        private const string LOG_FILE_PATH              = "error.log";
        private const string FNC_MAP_PATH               = "src/WCS.MAIN/settings/functionMap.wcs";
        private const int    MAX_FILE_LENGTH            = 1000000;
        private const sbyte  SPLIT_PROP_LENGTH          = 3;
        private const sbyte  SPLIT_HAS_PARAMS           = 2;
        private List<Type>   _typeList;

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

        public FunctionHandler prepare_platform_handler()
        {
            FunctionHandler handler = new FunctionHandler();
            switch (getOS())
            {
                case OS.WINDOWS:
                    handler.AudioHandler = new WindowsAudioHandler();
                    handler.InputHandler = new WindowsInputHandler();
                    break;
                case OS.LINUX:
                    handler.AudioHandler = new LinuxAudioHandler(new Settings());
                    handler.InputHandler = new LinuxInputHandler();
                    break;
                case OS.MACOSX:
                    handler.AudioHandler = new OSXAudioHandler();
                    handler.InputHandler = new OSXInputHandler();
                    break;
            }
            return handler;
        }

        public Dictionary<int, Func<object>> InitActionTable(object handler)
        {
            InitTypeList();
            string[] strLines = File.ReadAllLines(FNC_MAP_PATH);
            Dictionary<int, Func<object>> table = null;
            for (int i = 0; i < strLines.Length; i++)
            {
                if (strLines[i][0] == '/' && strLines[i][1] == '/') continue; // Comment line.
                /*
                    Split by ('-');
                    0 for Function key
                    1 for Handler property
                    2 for Function name
                 */
                string[] splitProps = strLines[i].Split('-');
                if (splitProps.Length == SPLIT_PROP_LENGTH)
                {
                    byte key;
                    if (!byte.TryParse(splitProps[0], out key))
                    {
                        log("Invalid key format on line: " + (i + 1));
                        continue;
                    }
                    Type isEnumType = _typeList.FirstOrDefault(x => Enum.IsDefined(x, key));
                    if (isEnumType == null)
                    {
                        log("Couldn't find the command key in Type table. Key: " + key);
                        continue;
                    }
                    Type            mainHandler             = handler.GetType();
                    PropertyInfo    functionHandlerInfo     = mainHandler.GetProperty(splitProps[1]);
                    Object          instanceOfFncHandler    = functionHandlerInfo.GetValue(handler);
                    string[]        ejectParams             = splitProps[2].Split('>'); // Split by ('>'); 0 for function name, 1 for parameters(if any).
                    MethodInfo      functionInfo            = instanceOfFncHandler.GetType().GetMethod(ejectParams[0]);
                    if (functionInfo == null)
                    {
                        log("Couldn't find the function on this object instance." + functionInfo.Name);
                        continue;
                    }
                    table = new Dictionary<int, Func<object>>();
                    List<object> parameters = new List<object>();
                    if (ejectParams.Length == SPLIT_HAS_PARAMS)
                    {
                        string[] paramValues = ejectParams[1].Split('-'); // split all parameters, basically load every parameter and values into an array.
                        foreach (string paramItem in paramValues)
                        {
                            Type parameterType;
                            string[] pItemSplit = paramItem.Split(':'); // Split by (':'); 0 for parameter type, 1 for parameter value.
                            parameterType = Type.GetType(pItemSplit[0]);
                            // TODO: Expand behaviour for non primitive types.
                            object parameterValue = Convert.ChangeType(pItemSplit[1], parameterType);
                        }
                    }
                    // Since MethodInfo.Invoke() sucks in terms of performance by itself. We need a move like this. (still needs to improve)
                    Func<object> fncPtr = () => functionInfo.Invoke(instanceOfFncHandler, parameters.ToArray());
                    table.Add(key, fncPtr);
                }
            }
            return table;
        }

        private void InitTypeList()
        {
            /*TODO: Implement init from file.*/
            _typeList.Add(typeof(OS_COMMANDS));
            _typeList.Add(typeof(INPUT_COMMANDS));
            _typeList.Add(typeof(AUDIO_COMMANDS));
        }
    }
}

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
        private const string FNC_MAP_PATH               = "../../src/WCS.MAIN/settings/functionMap.wcs";
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
            handler.PluginHandler = new Functions.Handlers.PluginHandler("v4.0"); // we'll keep it constant right now.
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
            // This will be our table that holds the function pointers
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
                 // Split every prop.
                string[] ejectParams = strLines[i].Split('>'); // Split by ('>'); 0 properties string, 1 for parameters(if any).
                string[] splitProps  = ejectParams[0].Split('-');
                // We should have SPLIT_PROP_LENGTH props when we split the string.
                if (splitProps.Length == SPLIT_PROP_LENGTH)
                {
                    // Currently(and probably always), this program won't register a key higher than a byte.
                    byte key;
                    if (!byte.TryParse(splitProps[0], out key))
                    {
                        log("Invalid key format on line: " + (i + 1));
                        continue;
                    }
                    // Check if the key exists in enums.
                    Type isEnumType = _typeList.FirstOrDefault(x => Enum.IsDefined(x, (int)key));
                    if (isEnumType == null)
                    {
                        log("Couldn't find the command key in Type table. Key: " + key);
                        continue;
                    }
                    Object          instanceOfFncHandler    = null; // instance of the object that we seek the function.
                    Type            mainHandler             = handler.GetType(); // basically the main FunctionHandler instance.
                    PropertyInfo    functionHandlerInfo     = mainHandler.GetProperty(splitProps[1]);  // Get the property of the handler(the instance of handler. ex: WindowsAudioHandler)
                    // If the prop has PC_ prefix that will indicate us that this is a plugin function. We should get the instance of it.
                    if (isPluginClass(splitProps[1]))
                    {
                                    FunctionHandler hndl = (FunctionHandler)handler;
                                    // We only send the real class name without the prefix. WARNING: filename and the class name should be the same !!
                                    instanceOfFncHandler    = hndl.PluginHandler.getPluginInstance(splitProps[1].Substring(3));
                    }
                    else
                                    instanceOfFncHandler    = functionHandlerInfo.GetValue(handler);

                    MethodInfo      functionInfo            = instanceOfFncHandler.GetType().GetMethod(splitProps[2]);
                    if (functionInfo == null)
                    {
                        log("Couldn't find the function on this object instance." + functionInfo.Name);
                        continue;
                    }
                    // Now, everything worked out perfectly so far, we can initialize our function table.
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

        private bool isPluginClass(string propvalue)
        {
            string propPrefix = "PC_";
            bool isPrefix     = true;
            for (int i = 0; i < propPrefix.Length; i++)
            {
                if (propvalue[i] != propPrefix[i])
                {
                    isPrefix  = false;
                    break;
                }
            }
            return isPrefix;
        }

        private void InitTypeList()
        {
            _typeList = new List<Type>();
            /*TODO: Implement init from file.*/
            _typeList.Add(typeof(OS_COMMANDS));
            _typeList.Add(typeof(INPUT_COMMANDS));
            _typeList.Add(typeof(AUDIO_COMMANDS));
            _typeList.Add(typeof(PLUGINS));
        }
    }
}

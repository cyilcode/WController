using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WCS.MAIN.Globals
{
    public class Settings
    {
        private const string OSX_CONF_FILE = "settings/OSXSettings.config";
        private const string WIN_CONF_FILE = "settings/WindowsSettings.config";
        private const string LIN_CONF_FILE = "settings/LinuxSettings.config";

        /// <summary>
        /// Queries the configuration files.
        /// </summary>
        /// <typeparam name="T">Queried key type</typeparam>
        /// <param name="key">Key to query</param>
        /// <returns>Type converted key value.</returns>
        public T confQuery<T>(string key)
        {
            string[] fileContents = null;
            string callerType     = new StackTrace()
                                        .GetFrame(1)
                                        .GetMethod()
                                        .DeclaringType
                                        .Name;

            if (callerType == "linuxFunctions")
                fileContents = File.ReadAllLines(LIN_CONF_FILE);
            else if (callerType == "osxFunctions")
                fileContents = File.ReadAllLines(OSX_CONF_FILE);
            else if (callerType == "windowsFunctions")
                fileContents = File.ReadAllLines(WIN_CONF_FILE);
            else throw new InvalidOperationException("This function is only usable for platform function instances.");

            string keyValue = fileContents.FirstOrDefault(x => x.Contains(key))
                                          .Split(':')[1]
                                          .TrimStart();
            if (keyValue == null)
                throw new KeyNotFoundException("Requested key not found.");
            return (T)Convert.ChangeType(keyValue, typeof(T));
        }
    }
}

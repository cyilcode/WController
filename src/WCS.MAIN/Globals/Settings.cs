﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WCS.MAIN.Globals
{
    public class Settings
    {
        private const string OSX_CONF_FILE = "src/WCS.MAIN/settings/OSXSettings.config";
        private const string WIN_CONF_FILE = "src/WCS.MAIN/settings/WindowsSettings.config";
        private const string LIN_CONF_FILE = "src/WCS.MAIN/settings/LinuxSettings.config";

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

            if(!callerType.Contains("Handler"))
                throw new InvalidOperationException("This function is only usable for platform handler instances.");

            if (callerType.Contains("Linux"))
                fileContents = File.ReadAllLines(LIN_CONF_FILE);
            else if (callerType.Contains("OSX"))
                fileContents = File.ReadAllLines(OSX_CONF_FILE);
            else if (callerType.Contains("Windows"))
                fileContents = File.ReadAllLines(WIN_CONF_FILE);
            else throw new InvalidOperationException("This function is only usable for platform handler instances.");

            string keyValue = fileContents.FirstOrDefault(x => x.Contains(key))
                                          .Split(':')[1]
                                          .TrimStart();
            if (keyValue == null)
                throw new KeyNotFoundException("Requested key not found.");
            return (T)Convert.ChangeType(keyValue, typeof(T));
        }
    }
}

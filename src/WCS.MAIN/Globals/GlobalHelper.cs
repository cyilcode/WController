using System;
using System.IO;

namespace WCS.MAIN.Globals
{
    public class GlobalHelper
    {
        // Some distros throw this instead of the values in the Platform enum.
        private const byte LINUX_IS_BEING_A_DICK = 128;
        public void coloredLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = default(ConsoleColor);
        }

        public OS getOS()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                case (PlatformID)LINUX_IS_BEING_A_DICK:
                    return OS.LINUX;
                case PlatformID.MacOSX:
                    return OS.MACOSX;
                default:
                    return OS.WINDOWS;
            }
        }
    }
}

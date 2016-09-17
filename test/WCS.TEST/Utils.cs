using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.MAIN.Functions;
using WCS.MAIN.Functions.Handlers.AudioHandlers;
using WCS.MAIN.Functions.Handlers.InputHandlers;
using WCS.MAIN.Globals;

namespace WCS.TEST
{
    public static class Utils
    {
        public static FunctionHandler prepare_function_handler()
        {
            FunctionHandler handler = new FunctionHandler();
            switch (new GlobalHelper().getOS())
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
    }
}

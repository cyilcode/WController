using System.Collections.Generic;
using WCS.MAIN.Functions.Handlers;
using WCS.MAIN.Interfaces;

namespace WCS.MAIN.Functions
{
    public class FunctionHandler
    {
        public IAudioHandler AudioHandler  { get; set; }
        public IInputHandler InputHandler  { get; set; }
        public PluginHandler PluginHandler { get; set; }
    }
}

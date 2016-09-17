using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;

namespace WCS.MAIN.Functions
{
    public class FunctionHandler
    {
        public IAudioHandler AudioHandler { get; set; }
        public IInputHandler InputHandler { get; set; }
        public object FUNCTION_FAIL_RET   { get { return 9998; } }
    }
}

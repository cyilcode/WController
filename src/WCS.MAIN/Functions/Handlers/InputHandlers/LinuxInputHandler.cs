using WCS.MAIN.Interfaces;
using WCS.MAIN.Globals;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System;

#region typedefs
// not needed at all but i always wanted to use these badboys :)
using xdo_t = System.IntPtr;
using Window = System.UInt64;
#endregion

namespace WCS.MAIN.Functions.Handlers.InputHandlers
{
    public class LinuxInputHandler : IInputHandler
    {

        private const string                LIBXDO_LIB_PATH                 = "libxdo.so.3";
        private const int                   INPUT_SUCCESS                   = 0;
        private const int                   MOUSE_BUTTON_LEFT               = 1;
        private const int                   MOUSE_BUTTON_MIDDLE             = 2;
        private const int                   MOUSE_BUTTON_RIGHT              = 3;
        private const int                   MOUSE_BUTTON_WHEELUP            = 4;    // Should figure out horizontal wheel aswell. instead of sending left & right keys.
        private const int                   MOUSE_BUTTON_WHEELDOWN          = 5;
        private int                         ret                             = 0;
        public readonly object              FUNCTION_FAIL_RET               = 9998;

        /*
            More information is no where because this library doesn't have a up to date online documentation.
            At least the ones that i've found were either for python wrappers or deprecated.
            Instead refer here: https://github.com/jordansissel/xdotool
        */

        /// <summary>
        /// Create a new xdo_t instance.
        /// </summary>
        /// <param name="display">Display the string display name, such as ":0". If null, uses the environment variable DISPLAY just like XOpenDisplay(NULL).</param>
        /// <returns>Pointer to a new xdo_t or NULL on failure</returns>
        [DllImport(LIBXDO_LIB_PATH)]
        static extern xdo_t xdo_new(string display);

        /// <summary>
        /// Get the currently-active window.
        /// Requires your window manager to support this. Uses _NET_ACTIVE_WINDOW from the EWMH spec.
        /// </summary>
        /// <param name="xdo">xdo_t instance</param>
        /// <param name="window_ret">window_ret	Pointer to Window where the active window is stored</param>
        /// <returns>0 on success.(obviously not zero on fail lel)</returns>
        [DllImport(LIBXDO_LIB_PATH)]
        static extern int xdo_get_active_window(xdo_t xdo, out Window window_ret);

        /// <summary>
        /// Send a keysequence to the specified window.
        /// This allows you to send keysequences by symbol name. Any combination of X11 KeySym names separated by '+' are valid. Single KeySym names are valid, too.
        /// Examples: "l" "semicolon" "alt+Return" "Alt_L+Tab"
        /// If you want to type a string, such as "Hello world." you want to instead use xdo_enter_text_window.
        /// </summary>
        /// <param name="xdo">xdo_t instance</param>
        /// <param name="window_ret">The window you want to send the keysequence to or CURRENTWINDOW(0)</param>
        /// <param name="keysequence">The string keysequence to send</param>
        /// <param name="delay">The delay between keystrokes in microseconds</param>
        /// <returns>0 on success.(obviously not zero on fail lel)</returns>
        [DllImport(LIBXDO_LIB_PATH)]
        static extern int xdo_enter_text_window(xdo_t xdo, Window window_ret, string keysequence, uint delay = 12000);

        /// <summary>
        /// Send a mouse press (aka mouse down) for a given button at the current mouse location.
        /// </summary>
        /// <param name="xdo">xdo_t instance</param>
        /// <param name="window_ret">The window you want to send the keysequence to or CURRENTWINDOW(0)</param>
        /// <param name="button"></param>
        /// <returns>0 on success.(obviously not zero on fail lel)</returns>
        [DllImport(LIBXDO_LIB_PATH)]
        static extern int xdo_mouse_down(xdo_t xdo, Window window_ret, int button);
        [DllImport(LIBXDO_LIB_PATH)]
        static extern int xdo_mouse_up(xdo_t xdo, Window window_ret, int button);

        /// <summary>
        /// Free and destroy an xdo_t instance.
        /// </summary>
        /// <param name="xdo">xdo_t instance</param>
        [DllImport(LIBXDO_LIB_PATH)]
        static extern void xdo_free(xdo_t xdo);

        public Point getMousePosition() => Cursor.Position;

        public int setMousePosition(Point mousePoint)
        {
            if (mousePoint == null) return (int)FUNCTION_FAIL_RET;
            Cursor.Position = mousePoint;
            return INPUT_SUCCESS;
        }

        public int sendKeyStroke(string key)
        {
            /*
                This function is great unless it sucks.
                Cuz of reasons that i'm not really sure yet, xdo_enter_text_window acts really weird on Turkish characters
                or in general, non ASCI characters. I assume the function reacts unicodes as unicoRNS since its performance gets better
                as the function delay increases. Will check on that but for now, libxdo is a lot more viable solution than coding an X11 shared lib
                by myself.
             */
            if (key == null || key == string.Empty)
                return (int)FUNCTION_FAIL_RET;
            Window w_ret;
            xdo_t mXDO = xdo_new(":0"); // basically a NULL
            ret = xdo_get_active_window(mXDO, out w_ret);
            if (ret != INPUT_SUCCESS)
            {
                GlobalHelper.log("xdo_get_active_window failed with errcode: " + ret);
                xdo_free(mXDO);
                return (int)FUNCTION_FAIL_RET;
            }
            ret = xdo_enter_text_window(mXDO, w_ret, key);
            if (ret != INPUT_SUCCESS)
            {
                GlobalHelper.log("xdo_enter_text_window failed with errcode: " + ret);
                xdo_free(mXDO);
                return (int)FUNCTION_FAIL_RET;
            }
            xdo_free(mXDO);
            return INPUT_SUCCESS;
        }

        public int sendMouseEvent(COMMANDS evt)
        {
            throw new NotImplementedException();
        }
    }
}

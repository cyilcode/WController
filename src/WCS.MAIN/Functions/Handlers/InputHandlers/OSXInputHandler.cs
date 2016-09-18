using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System;

#region typedefs
using CGEventRef = System.IntPtr;
using __notused__ = System.String;
#endregion

namespace WCS.MAIN.Functions.Handlers.InputHandlers
{
    public class OSXInputHandler : IInputHandler
    {
        private const   string            QUARTZ_LIB_PATH             = "Quartz.framework/Versions/A/Quartz";        // Quartz P/Invoke lib.
        private const   uint              FUNCTION_SUCCESS            = 0;            // Basically a function fail const
        private const   uint              CARBON_NO_KEY               = 0;            // Carbon event no key init.
        private const   uint              KCGHIDEEVENTTAP             = 0;            // Event tapping point
        private const   bool              CARBON_KEYDOWN              = true;         // Flag to init the event as Keydown
        public readonly object            FUNCTION_FAIL_RET           = 9998;         // WController function fail ret. TODO: MORE ID'S.

        [StructLayout(LayoutKind.Sequential)]
        struct CGPoint
        {
            public float x;
            public float y;
            public CGPoint(bool initFromMonoCursor) { x = Cursor.Position.X; y = Cursor.Position.Y; }
        }

        [DllImport(QUARTZ_LIB_PATH)]
        static extern CGEventRef CGEventCreateKeyboardEvent(__notused__ source, uint keyCode, bool isKeyDown);
        [DllImport(QUARTZ_LIB_PATH)]
        static extern CGEventRef CGEventCreateMouseEvent(__notused__ source, int mouseType, CGPoint mouseCursorPosition, int mouseButton);
        [DllImport(QUARTZ_LIB_PATH)]
        static extern CGEventRef CGEventCreateScrollWheelEvent(__notused__ source, int units, uint wheelCount, int v, int h);
        [DllImport(QUARTZ_LIB_PATH)]
        static extern void CGEventPost(uint tap, CGEventRef CGEventPtr);
        [DllImport(QUARTZ_LIB_PATH)]
        static extern void CGEventKeyboardSetUnicodeString(CGEventRef evtPtr, int strcount, byte[] unicodeString);
        [DllImport(QUARTZ_LIB_PATH)]
        static extern void CFRelease(CGEventRef CGEventPtr);

        public Point getMousePosition() => Cursor.Position;

        public int setMousePosition(Point mousePoint)
        {
            if (mousePoint == null)
            {
                GlobalHelper.log("Recieved a NULL Forms.Point struct");
                return (int)FUNCTION_FAIL_RET;
            }
            Cursor.Position = mousePoint;
            return (int)FUNCTION_SUCCESS;
        }

        public int sendKeyStroke(string key)
        {
            if (key == null || key == string.Empty)
            {
                GlobalHelper.log("Recieved a NULL or empty key string");
                return (int)FUNCTION_FAIL_RET;
            }
            CGEventRef iEvent = CGEventCreateKeyboardEvent(null,
                                                           CARBON_NO_KEY,
                                                           CARBON_KEYDOWN);
            CGEventKeyboardSetUnicodeString(iEvent,
                                            key.Length,
                                            Encoding.Unicode.GetBytes(key));
            CGEventPost(KCGHIDEEVENTTAP, iEvent);
            CFRelease(iEvent);
            return (int)FUNCTION_SUCCESS;
        }

        public int sendMouseEvent(COMMANDS evt)
        {
            throw new NotImplementedException();
        }
    }
}

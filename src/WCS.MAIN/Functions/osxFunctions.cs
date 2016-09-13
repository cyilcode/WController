using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WCS.MAIN.Interfaces;
#region typedefs
using CGEventRef  = System.IntPtr;
using __notused__ = System.String;
using System.Text;
using WCS.MAIN.Globals;
#endregion

namespace WCS.MAIN.Functions
{
    public class osxFunctions : IFunctions
    {
#if DNX451

        public  const int       MIXER_MAX_VOL                    = 100;          // Max scalar value
        public  const int       MIXER_MIN_VOL                    = 0;            // Min scalar value
        private const int       NO_QUALIFIER                     = 0;            // A UInt32 indicating the size of the buffer pointed to by inQualifierData. Note that not all properties require qualification, in which case thisvalue will be 0.
        private const int       OBJ_SYSTEM_OBJ                   = 1;            // The AudioObjectID that always refers to the one and only instance of the AudioSystemObject class.
        private const int       WHEELEVENT_UNITS_LINE            = 1;            // Line flag for unit type
        private const int       WHEELEVENT_WHEEL_COUNT           = 2;            // 1 = Vertical, 2 = Veritcal - Horizontal
        private const int       WHEELEVENT_WHEEL_Y_SENSITIVITY   = 50;           // Positive value = Upwards, Negative Value = Downwards
        private const int       WHEELEVENT_WHEEL_X_SENSITIVITY   = 50;           
        private const uint      PROP_ELEM_MASTER                 = 0;            // 0
        private const uint      PROP_SCOPE_GLOBAL                = 1735159650;   // 'glob'
        private const uint      PROP_SCOPE_OUTPUT                = 1869968496;   // 'outp'
        private const uint      PROP_SELECTOR_V_MASTER           = 1986885219;   // 'vmvc'
        private const uint      PROP_SELECTOR_DEF_DEV            = 1682929012;   // 'dOut'
        private const uint      PROP_SELECTOR_VOL_SCA            = 1987013741;   // 'volm'
        private const uint      PROP_SELECTOR_VOL_MUTE           = 1836414053;   // 'mute'
        private const uint      FUNCTION_SUCCESS                 = 0;            // Basically a function fail const
        private const uint      CARBON_NO_KEY                    = 0;            // Carbon event no key init.
        private const uint      KCGHIDEEVENTTAP                  = 0;            // Event tapping point
        private const bool      CARBON_KEYDOWN                   = true;         // Flag to init the event as Keydown
        private const string    CORE_AUDIO_LIB_PATH              = "AudioUnit.framework/AudioUnit";             // Core Audio P/Invoke lib.
        private const string    QUARTZ_LIB_PATH                  = "Quartz.framework/Versions/A/Quartz";        // Quartz P/Invoke lib.
        private const string    GET_DATA_FAIL_LOG_MESSAGE        = "AudioObjectGetPropertyData failed with: ";  // It will always log the same thing so, might aswell make my job easier.
        private const string    PROP_FAIL_LOG_MESSAGE            = "Couldn't find properties.";                 // Same above.
        private       uint      SIZE                             = 0;            // A global var to prevent redefiniton
        private readonly uint   DEFAULT_DEVICE                   = 0;            // global deviceID
        public  readonly object FUNCTION_FAIL_RET                = 9998;        // WController function fail ret. TODO: MORE ID'S.
        private       int       ret                              = 0;

        #region CoreAudio
        /* P/Invoke to CoreAudio and CoreHardware API's(Honestly the worst low level api i've ever seen).
         * More information here: https://developer.apple.com/reference/coreaudio/
         * Also: https://developer.apple.com/reference/audiounit/
         */

        [StructLayout(LayoutKind.Sequential)]
        struct AudioObjectPropertyAddress
        {
            public uint mElement;
            public uint mScope;
            public uint mSelector;

            public AudioObjectPropertyAddress(uint Element, uint Scope, uint Selector)
            {
                mElement = Element;
                mScope = Scope;
                mSelector = Selector;
            }
        }

        /* This api uses operation codes to execute a function.
         * On a real osx platform, these values belong to their corresponding enumerators
         * but we don't need that kind of object structure here. Only the key values should do the trick.
         * the values in quotes are the values that osx api originally use. 
         * In here we pass the str -> hex -> decimal values.
         * Ex: 'volm' -> (hex)0x766f6c6d -> (decimal)1987013741 */

        [DllImport(CORE_AUDIO_LIB_PATH)]
        private static extern int AudioObjectGetPropertyData
        (
            uint inObjectID,
            ref AudioObjectPropertyAddress inAddress,
            uint inQualifierDataSize,
            IntPtr inQualifierData,
            ref uint ioDataSize,
            [Out] byte[] outData
        /* I could've just said 'out uint outData' here but that would make my life harder 
           since i should redefine the function for every data type i use. With [Out] attribute
           i just convert the data into a byte buffer and let the core do the conversion. */
        );

        [DllImport(CORE_AUDIO_LIB_PATH)]
        private static extern int AudioObjectSetPropertyData
        (
            uint inObjectID,
            ref AudioObjectPropertyAddress inAddress,
            uint inQualifierDataSize,
            IntPtr inQualifierData,
            uint inDataSize,
            byte[] inData
        );

        [DllImport(CORE_AUDIO_LIB_PATH)]
        private static extern bool AudioObjectHasProperty(uint inObjectID, ref AudioObjectPropertyAddress inAddress);
        #endregion

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

        public osxFunctions()
        {
            DEFAULT_DEVICE = get_mixer();
        }

        private uint get_mixer()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_DEF_DEV,
                                                        PROP_SCOPE_GLOBAL,
                                                        PROP_ELEM_MASTER);
            SIZE = sizeof(uint);
            byte[] deviceIDBuffer = new byte[SIZE];
            ret = AudioObjectGetPropertyData(OBJ_SYSTEM_OBJ,
                                             ref objAdr,
                                             NO_QUALIFIER,
                                             IntPtr.Zero,
                                             ref SIZE,
                                             deviceIDBuffer);
            uint deviceID = BitConverter.ToUInt32(deviceIDBuffer, 0);
            if (deviceID != FUNCTION_SUCCESS)
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                if (ret != FUNCTION_SUCCESS)
                    GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
                return (uint)FUNCTION_FAIL_RET;
            }
            else
               return deviceID;
        }

        public float GetVolumeLevel()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_V_MASTER,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return Convert.ToSingle(FUNCTION_FAIL_RET);
            }
            SIZE = sizeof(float);
            byte[] volumeBuffer = new byte[SIZE];
            ret = AudioObjectGetPropertyData(DEFAULT_DEVICE,
                                             ref objAdr,
                                             NO_QUALIFIER,
                                             IntPtr.Zero,
                                             ref SIZE,
                                             volumeBuffer);
            if (ret != FUNCTION_SUCCESS)
            {
                GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
                return Convert.ToSingle(FUNCTION_FAIL_RET);
            }
            return (float)(Math.Round(BitConverter.ToSingle(volumeBuffer, 0), 2) * 100);
        }

        public bool isMixerMuted()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_MUTE,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return false;
            }
            /* even tho i want a bool here, the api returns an uint. So, allocate enough space for an uint not a bool. */
            SIZE = sizeof(uint);
            byte[] mixerStateBuffer = new byte[SIZE];
            ret = AudioObjectGetPropertyData(DEFAULT_DEVICE,
                                             ref objAdr,
                                             NO_QUALIFIER,
                                             IntPtr.Zero,
                                             ref SIZE,
                                             mixerStateBuffer);
            if (ret != FUNCTION_SUCCESS)
            {
                GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
                return false;
            }
            return BitConverter.ToBoolean(mixerStateBuffer, 0);
        }

        public void muteMixer()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_MUTE,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return;
            }
            // I could go (uint)buffer.length here but not gonna do that to keep the code unite.
            SIZE = sizeof(uint);
            byte[] mixerStateBuffer  = BitConverter.GetBytes(true);
            ret = AudioObjectSetPropertyData(DEFAULT_DEVICE,
                                             ref objAdr,
                                             NO_QUALIFIER,
                                             IntPtr.Zero,
                                             SIZE,
                                             mixerStateBuffer);
            if (ret != FUNCTION_SUCCESS)
                GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
        }

        public void unmuteMixer()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_MUTE,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return;
            }
            SIZE = sizeof(uint);
            byte[] mixerStateBuffer = BitConverter.GetBytes(false);
            ret = AudioObjectSetPropertyData(DEFAULT_DEVICE,
                                             ref objAdr,
                                             NO_QUALIFIER,
                                             IntPtr.Zero,
                                             SIZE,
                                             mixerStateBuffer);
            if (ret != FUNCTION_SUCCESS)
                GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
        }

        public void VolumeDownBy(float value)
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_V_MASTER,
                                                         PROP_SCOPE_OUTPUT,
                                                         PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return;
            }
            SIZE = sizeof(float);
            float volume_to_set = (GetVolumeLevel() - value) / 100;
            byte[] mixerVolumeLevel = BitConverter.GetBytes(volume_to_set);
            ret = AudioObjectSetPropertyData(DEFAULT_DEVICE,
                                             ref objAdr,
                                             NO_QUALIFIER,
                                             IntPtr.Zero,
                                             SIZE,
                                             mixerVolumeLevel);
            if (ret != FUNCTION_SUCCESS)
                GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
        }

        public void VolumeUpBy(float value)
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_V_MASTER,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return;
            }
            SIZE = sizeof(float);
            float volume_to_set = (GetVolumeLevel() + value) / 100;
            byte[] mixerVolumeLevel = BitConverter.GetBytes(volume_to_set);
            ret = AudioObjectSetPropertyData(DEFAULT_DEVICE,
                                             ref objAdr,
                                             NO_QUALIFIER,
                                             IntPtr.Zero,
                                             SIZE,
                                             mixerVolumeLevel);
            if (ret != FUNCTION_SUCCESS)
                GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
        }

        public Point getMousePosition() => Cursor.Position;

        public void setMousePosition(Point mousePoint) => Cursor.Position = mousePoint;

        public void sendKeyStroke(string key)
        {
            CGEventRef iEvent = CGEventCreateKeyboardEvent(null, 
                                                           CARBON_NO_KEY, 
                                                           CARBON_KEYDOWN);
            CGEventKeyboardSetUnicodeString(iEvent, 
                                            key.Length, 
                                            Encoding.Unicode.GetBytes(key));
            CGEventPost(KCGHIDEEVENTTAP, iEvent);
            CFRelease(iEvent);
        }
#endif
    }
}
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WCS.MAIN.Interfaces;

namespace WCS.MAIN.Functions
{
    public class osxFunctions : IFunctions
    {
#if DNX451
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

        public  const int       MIXER_MAX_VOL                    = 100;          // Max scalar value
        public  const int       MIXER_MIN_VOL                    = 100;          // Min scalar value
        private const int       NO_QUALIFIER                     = 0;            // A UInt32 indicating the size of the buffer pointed to by inQualifierData. Note that not all properties require qualification, in which case thisvalue will be 0.
        private const int       OBJ_SYSTEM_OBJ                   = 1;            // The AudioObjectID that always refers to the one and only instance of the AudioSystemObject class.
        private const uint      PROP_ELEM_MASTER                 = 0;            // 0
        private const uint      PROP_ELEM_S_CHANNEL_F            = 1;            // Default device sound channels
        private const uint      PROP_ELEM_S_CHANNEL_S            = 2;            // Default device sound channels
        private const uint      PROP_SCOPE_GLOBAL                = 1735159650;   // 'glob'
        private const uint      PROP_SCOPE_OUTPUT                = 1869968496;   // 'outp'
        private const uint      PROP_SELECTOR_DEF_DEV            = 1682929012;   // 'dOut'
        private const uint      PROP_SELECTOR_VOL_SCA            = 1987013741;   // 'volm'
        private const uint      PROP_SELECTOR_VOL_MUTE           = 1836414053;   // 'mute'
        private const uint      FUNCTION_FAIL                    = 0;            // Basically a function fail const
        private const string    LIB_PATH                         = "/System/Library/Frameworks/AudioUnit.framework/AudioUnit"; // P/Invoke lib.
        private       uint      SIZE                             = 0;            // A global var to prevent redefiniton
        private readonly uint   DEFAULT_DEVICE                   = 0;            // global deviceID
        public  readonly object FUNCTION_FAIL_RET                = -1337;        // WController function fail ret. TODO: MORE ID'S.

        [DllImport(LIB_PATH)]
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

        [DllImport(LIB_PATH)]
        private static extern int AudioObjectSetPropertyData
        (
            uint inObjectID,
            ref AudioObjectPropertyAddress inAddress,
            uint inQualifierDataSize,
            IntPtr inQualifierData,
            uint inDataSize,
            byte[] inData
        );

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
            AudioObjectGetPropertyData(OBJ_SYSTEM_OBJ,
                                       ref objAdr,
                                       NO_QUALIFIER,
                                       IntPtr.Zero,
                                       ref SIZE,
                                       deviceIDBuffer);
            uint deviceID = BitConverter.ToUInt32(deviceIDBuffer, 0);
            if (deviceID <= FUNCTION_FAIL)
            {
                return (uint)FUNCTION_FAIL_RET;
                // TODO: Log !
            }
            else
               return deviceID;
        }

        public float GetVolumeLevel()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_SCA,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_S_CHANNEL_S);
            SIZE = sizeof(float);
            byte[] volumeBuffer = new byte[SIZE];
            int ret = AudioObjectGetPropertyData(DEFAULT_DEVICE,
                                                 ref objAdr,
                                                 NO_QUALIFIER,
                                                 IntPtr.Zero,
                                                 ref SIZE,
                                                 volumeBuffer);
            if (ret == FUNCTION_FAIL) return (float)FUNCTION_FAIL_RET;
            return (float)(Math.Round(BitConverter.ToSingle(volumeBuffer, 0), 0) * 100);
        }

        public bool isMixerMuted()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_MUTE,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_S_CHANNEL_S);
            /* even tho i want a bool here, the api returns an uint. So, allocate enough space for an uint not a bool. */
            SIZE = sizeof(uint);
            byte[] mixerStateBuffer = new byte[SIZE];
            AudioObjectGetPropertyData(DEFAULT_DEVICE,
                                       ref objAdr,
                                       NO_QUALIFIER,
                                       IntPtr.Zero,
                                       ref SIZE,
                                       mixerStateBuffer);
            return BitConverter.ToBoolean(mixerStateBuffer, 0);
        }

        public void muteMixer()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_MUTE,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_S_CHANNEL_S);
            // I could go (uint)buffer.length here but not gonna do that to keep the code unite.
            SIZE = sizeof(uint);
            byte[] mixerStateBuffer  = BitConverter.GetBytes(true);
            AudioObjectSetPropertyData(DEFAULT_DEVICE,
                                       ref objAdr,
                                       NO_QUALIFIER,
                                       IntPtr.Zero,
                                       SIZE,
                                       mixerStateBuffer);
        }

        public void unmuteMixer()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_MUTE,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_S_CHANNEL_S);
            SIZE = sizeof(uint);
            byte[] mixerStateBuffer = BitConverter.GetBytes(false);
            AudioObjectSetPropertyData(DEFAULT_DEVICE,
                                       ref objAdr,
                                       NO_QUALIFIER,
                                       IntPtr.Zero,
                                       SIZE,
                                       mixerStateBuffer);
        }

        public void VolumeDownBy(float value)
        {
            var objrAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_SCA,
                                                         PROP_SCOPE_OUTPUT,
                                                         PROP_ELEM_S_CHANNEL_S);
            SIZE = sizeof(float);
            float volume_to_set = GetVolumeLevel() - value;
            byte[] mixerVolumeLevel = BitConverter.GetBytes(volume_to_set);
            AudioObjectSetPropertyData(DEFAULT_DEVICE,
                                       ref objrAdr,
                                       NO_QUALIFIER,
                                       IntPtr.Zero,
                                       SIZE,
                                       mixerVolumeLevel);
        }

        public void VolumeUpBy(float value)
        {
            var objrAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_SCA,
                                             PROP_SCOPE_OUTPUT,
                                             PROP_ELEM_S_CHANNEL_S);
            SIZE = sizeof(float);
            float volume_to_set = GetVolumeLevel() + value;
            byte[] mixerVolumeLevel = BitConverter.GetBytes(volume_to_set);
            AudioObjectSetPropertyData(DEFAULT_DEVICE,
                                       ref objrAdr,
                                       NO_QUALIFIER,
                                       IntPtr.Zero,
                                       SIZE,
                                       mixerVolumeLevel);
        }

        public Point getMousePosition() => Cursor.Position;

        public void setMousePosition(Point mousePoint) => Cursor.Position = mousePoint;

        public void sendKeyStroke(string key)
        {
            throw new NotImplementedException();
        }
#endif
    }
}

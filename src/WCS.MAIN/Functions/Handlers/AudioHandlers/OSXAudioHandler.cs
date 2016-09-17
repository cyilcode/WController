using System;
using System.Runtime.InteropServices;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;

namespace WCS.MAIN.Functions.Handlers.AudioHandlers
{
    public class OSXAudioHandler : IAudioHandler
    {
		public  const int       MIXER_MAX_VOL                    = 100;          // Max scalar value
        public  const int       MIXER_MIN_VOL                    = 0;            // Min scalar value
        private const int       NO_QUALIFIER                     = 0;            // A UInt32 indicating the size of the buffer pointed to by inQualifierData. Note that not all properties require qualification, in which case thisvalue will be 0.
        private const int       OBJ_SYSTEM_OBJ                   = 1;            // The AudioObjectID that always refers to the one and only instance of the AudioSystemObject class.       
        private const uint      PROP_ELEM_MASTER                 = 0;            // 0
        private const uint      PROP_SCOPE_GLOBAL                = 1735159650;   // 'glob'
        private const uint      PROP_SCOPE_OUTPUT                = 1869968496;   // 'outp'
        private const uint      PROP_SELECTOR_V_MASTER           = 1986885219;   // 'vmvc'
        private const uint      PROP_SELECTOR_DEF_DEV            = 1682929012;   // 'dOut'
        private const uint      PROP_SELECTOR_VOL_SCA            = 1987013741;   // 'volm'
        private const uint      PROP_SELECTOR_VOL_MUTE           = 1836414053;   // 'mute'
        private const uint      FUNCTION_SUCCESS                 = 0;            // Basically a function fail const
        private const string    CORE_AUDIO_LIB_PATH              = "AudioUnit.framework/AudioUnit";             // Core Audio P/Invoke lib.
        private const string    GET_DATA_FAIL_LOG_MESSAGE        = "AudioObjectGetPropertyData failed with: ";  // It will always log the same thing so, might aswell make my job easier.
        private const string    PROP_FAIL_LOG_MESSAGE            = "Couldn't find properties.";                 // Same above.
        private       uint      SIZE                             = 0;            // A global var to prevent redefiniton
        private readonly uint   DEFAULT_DEVICE                   = 0;            // global deviceID
        public  readonly object FUNCTION_FAIL_RET                = 9998;         // WController function fail ret.
        private       int       ret                              = 0;

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

        public OSXAudioHandler()
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

        public int GetVolumeLevel()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_V_MASTER,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return (int)FUNCTION_FAIL_RET;
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
                return (int)FUNCTION_FAIL_RET;
            }
            return (int)(Math.Round(BitConverter.ToSingle(volumeBuffer, 0), 2)) * 100;
        }

        public int isMixerMuted()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_MUTE,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return (int)FUNCTION_FAIL_RET;
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
                return (int)FUNCTION_FAIL_RET;
            }
            return BitConverter.ToInt32(mixerStateBuffer, 0);
        }

        public int muteMixer()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_MUTE,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return (int)FUNCTION_FAIL_RET;
            }
            // I could go (uint)buffer.length here but not gonna do that to keep the code unite.
            SIZE = sizeof(uint);
            byte[] mixerStateBuffer = BitConverter.GetBytes(true);
            ret = AudioObjectSetPropertyData(DEFAULT_DEVICE,
                                             ref objAdr,
                                             NO_QUALIFIER,
                                             IntPtr.Zero,
                                             SIZE,
                                             mixerStateBuffer);
            if (ret != FUNCTION_SUCCESS)
            {
                GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
                return (int)FUNCTION_FAIL_RET;
            }
            return ret;
        }

        public int unmuteMixer()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_VOL_MUTE,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return (int)FUNCTION_FAIL_RET;
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
            {
                GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
                return (int)FUNCTION_FAIL_RET;
            }
            return ret;
        }

        public int VolumeDownBy(int value)
        {
            if (value > MIXER_MAX_VOL || value < MIXER_MIN_VOL)
            {
                GlobalHelper.log("Invalid volume scalar range");
                return (int)FUNCTION_FAIL_RET;
            }
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_V_MASTER,
                                                         PROP_SCOPE_OUTPUT,
                                                         PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return (int)FUNCTION_FAIL_RET;
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
            {
                GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
                return (int)FUNCTION_FAIL_RET;
            }
            return ret;
        }

        public int VolumeUpBy(int value)
        {
            if (value > MIXER_MAX_VOL || value < MIXER_MIN_VOL)
            {
                GlobalHelper.log("Invalid volume scalar range");
                return (int)FUNCTION_FAIL_RET;
            }
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_V_MASTER,
                                                        PROP_SCOPE_OUTPUT,
                                                        PROP_ELEM_MASTER);
            if (!AudioObjectHasProperty(DEFAULT_DEVICE, ref objAdr))
            {
                GlobalHelper.log(PROP_FAIL_LOG_MESSAGE);
                return (int)FUNCTION_FAIL_RET;
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
            {
                GlobalHelper.log(GET_DATA_FAIL_LOG_MESSAGE + ret);
                return (int)FUNCTION_FAIL_RET;
            }
            return ret;
        }
    }
}

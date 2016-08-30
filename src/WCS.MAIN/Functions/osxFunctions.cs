using System;
using System.Drawing;
using System.Runtime.InteropServices;
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

        private const int  NO_QUALIFIER                     = 0;            // A UInt32 indicating the size of the buffer pointed to by inQualifierData. Note that not all properties require qualification, in which case thisvalue will be 0.
        private const int  OBJ_SYSTEM_OBJ                   = 1;            //The AudioObjectID that always refers to the one and only instance of the AudioSystemObject class.
        private const uint PROP_ELEM_MASTER                 = 0;            // 0
        private const uint PROP_SCOPE_GLOBAL                = 1735159650;   // 'glob'
        private const uint PROP_SCOPE_OUTPUT                = 1869968496;   // 'outp'
        private const uint PROP_SELECTOR_DEF_DEV            = 1682929012;   // 'dOut'
        private const uint PROP_SELECTOR_VOL_SCA            = 1987013741;   // 'volm'
        private const string LIB_PATH                       = "/System/Library/Frameworks/AudioUnit.framework/AudioUnit"; // P/Invoke lib.

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


        private uint get_default_mixer()
        {
            var objAdr = new AudioObjectPropertyAddress(PROP_SELECTOR_DEF_DEV,
                                                        PROP_SCOPE_GLOBAL,
                                                        PROP_ELEM_MASTER);
            uint size = sizeof(uint);
            byte[] deviceIDBuffer = new byte[sizeof(uint)];
            int ret = AudioObjectGetPropertyData(OBJ_SYSTEM_OBJ, 
                                                 ref objAdr, 
                                                 NO_QUALIFIER, 
                                                 IntPtr.Zero, 
                                                 ref size, 
                                                 deviceIDBuffer);
            return BitConverter.ToUInt32(deviceIDBuffer, 0);
        }

        public Point getMousePosition()
        {
            throw new NotImplementedException();
        }

        public float GetVolumeLevel()
        {
            throw new NotImplementedException();
        }

        public bool isMixerMuted()
        {
            throw new NotImplementedException();
        }

        public void muteMixer()
        {
            throw new NotImplementedException();
        }

        public void setMousePosition(Point mousePoint)
        {
            throw new NotImplementedException();
        }

        public void unmuteMixer()
        {
            throw new NotImplementedException();
        }

        public void VolumeDownBy(float value)
        {
            throw new NotImplementedException();
        }

        public void VolumeUpBy(float value)
        {
            throw new NotImplementedException();
        }
#endif
    }
}

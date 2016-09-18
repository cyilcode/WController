using System;
using System.Runtime.InteropServices;

namespace WCS.MAIN.Interfaces.Windows.CoreAudioAPI
{
    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioEndpointVolume
    {
        int not_implemented_1();            int not_implemented_2();
        int not_implemented_3();            int not_implemented_4();
        int SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext = default(Guid));
        int not_implemented_5();
        int GetMasterVolumeLevelScalar(out float pfLevel);
        int not_implemented_6();            int not_implemented_7();
        int not_implemented_8();            int not_implemented_9();
        int SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, Guid pguidEventContext = default(Guid));
        int GetMute(out bool pbMute);
        /*
            What are those not_implemented_x() functions?:
            This method of API implementation requires exact order of an Interface on both sides.
            The function definitions details are not important but the order should exaclty be the same(from top to the last function that you need. The rest are still can be skipped.)
            Example: this interface was hooked from endpointvolume.h header.
            Source: https://github.com/EddieRingle/portaudio/blob/master/src/hostapi/wasapi/mingw-include/endpointvolume.h#L191
         */
    }
}

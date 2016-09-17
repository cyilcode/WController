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
    }
}

using System;
using System.Runtime.InteropServices;

namespace WCS.MAIN.Interfaces.Windows.CoreAudioAPI
{
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {
        int not_implemented_1();
        int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice ppDevice);
    }
}

#if DNX451
using WCS.MAIN.Interfaces;
using NAudio.CoreAudioApi;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace Functions
{
    public class windowsFunctions : IFunctions
    {
        private MMDeviceEnumerator devices = new MMDeviceEnumerator();
        public MMDevice defaultSoundDevice { get { return devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia); } }

        public void VolumeDownBy(float value)
        {
            var minDecibels = defaultSoundDevice.AudioEndpointVolume.VolumeRange
                                                                    .MinDecibels;
            if ((defaultSoundDevice
                .AudioEndpointVolume
                .MasterVolumeLevel - value) > minDecibels)
                defaultSoundDevice.AudioEndpointVolume.MasterVolumeLevel -= value;
            else
                defaultSoundDevice.AudioEndpointVolume.MasterVolumeLevel = minDecibels;
        }
        
        public void VolumeUpBy(float value)
        {
            var maxDecibles = defaultSoundDevice.AudioEndpointVolume.VolumeRange
                                                                    .MaxDecibels;
            if ((defaultSoundDevice
                .AudioEndpointVolume
                .MasterVolumeLevel + value) < maxDecibles)
                defaultSoundDevice.AudioEndpointVolume.MasterVolumeLevel += value;
            else
                defaultSoundDevice.AudioEndpointVolume.MasterVolumeLevel = maxDecibles;
        }

        public float GetVolumeLevel() => defaultSoundDevice.AudioEndpointVolume.MasterVolumeLevel;

        public bool isMixerMuted() => defaultSoundDevice.AudioEndpointVolume.Mute;

        public void muteMixer() => defaultSoundDevice.AudioEndpointVolume.Mute = true;

        public void unmuteMixer() => defaultSoundDevice.AudioEndpointVolume.Mute = false;

        public Point getMousePosition() => Cursor.Position;

        public void setMousePosition(Point mousePoint) => Cursor.Position = mousePoint;

        public void sendKeyStroke(string key) => SendKeys.SendWait(key); // How i love .net devs...
    }
}
#endif
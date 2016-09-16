﻿#if DNX451
using WCS.MAIN.Interfaces;
using NAudio.CoreAudioApi;
using System.Drawing;
using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;

namespace Functions
{
    public class windowsFunctions : IFunctions
    {
        private const uint   MOUSEEVENTF_WHEEL      = 0x0800;
        private const uint   MOUSEEVENTF_LEFTDOWN   = 0x0002;
        private const uint   MOUSEEVENTF_LEFTUP     = 0x0004;
        private const uint   MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint   MOUSEEVENTF_MIDDLEUP   = 0x0040;
        private const uint   MOUSEEVENTF_RIGHTDOWN  = 0x0008;
        private const uint   MOUSEEVENTF_RIGHTUP    = 0x0010;
        private const uint   MOUSEEVENTF_HWHEEL     = 0x1000;
        private const uint   MOUSE_CURRENT_POS      = 0;
        private const  int   NO_MOUSE_DATA          = 0;
        private const  int   SCROLL_UP              = 120;
        private const  int   SCROLL_DOWN            = -120;
        private const  int   SCROLL_LEFT            = -200;
        private const  int   SCROLL_RIGHT           = 200;
		private const  int 	 FUNCTION_SUCCESS		= 0;
		private readonly object   FUNCTION_FAIL_RET = 9998;
        private const string WIN_USER32_PATH        = "User32.dll";
        private const string MOUSE_EVENT_EP         = "mouse_event";

        #region Mouse Key Simulation

        /* Ref: https://msdn.microsoft.com/en-us/library/windows/desktop/ms646260(v=vs.85).aspx */

        [DllImport(WIN_USER32_PATH, EntryPoint = MOUSE_EVENT_EP)]
        private static extern void  ScrollDown(uint dwFlags = MOUSEEVENTF_WHEEL,
                                               uint dx      = MOUSE_CURRENT_POS,
                                               uint dy      = MOUSE_CURRENT_POS,
                                               int dwData   = SCROLL_DOWN,
                                               IntPtr dwExt = default(IntPtr));

        [DllImport(WIN_USER32_PATH, EntryPoint = MOUSE_EVENT_EP)]
        private static extern void    ScrollUp(uint dwFlags = MOUSEEVENTF_WHEEL,
                                               uint dx      = MOUSE_CURRENT_POS,
                                               uint dy      = MOUSE_CURRENT_POS,
                                               int dwData   = SCROLL_UP,
                                               IntPtr dwExt = default(IntPtr));

        [DllImport(WIN_USER32_PATH, EntryPoint = MOUSE_EVENT_EP)]
        private static extern void  ScrollLeft(uint dwFlags = MOUSEEVENTF_HWHEEL,
                                               uint dx      = MOUSE_CURRENT_POS,
                                               uint dy      = MOUSE_CURRENT_POS,
                                               int dwData   = SCROLL_LEFT,
                                               IntPtr dwExt = default(IntPtr));

        [DllImport(WIN_USER32_PATH, EntryPoint = MOUSE_EVENT_EP)]
        private static extern void ScrollRight(uint dwFlags = MOUSEEVENTF_HWHEEL,
                                               uint dx      = MOUSE_CURRENT_POS,
                                               uint dy      = MOUSE_CURRENT_POS,
                                               int dwData   = SCROLL_RIGHT,
                                               IntPtr dwExt = default(IntPtr));

        // While for a regular click (DOWN+UP) we'll call this. To hold we'll call, LeftClick(MOUSEEVENTF_LEFTDOWN) only.
        [DllImport(WIN_USER32_PATH, EntryPoint = MOUSE_EVENT_EP)]
        private static extern void   LeftClick(uint dwFlags = MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP,
                                               uint dx      = MOUSE_CURRENT_POS,
                                               uint dy      = MOUSE_CURRENT_POS,
                                               int dwData   = NO_MOUSE_DATA,
                                               IntPtr dwExt = default(IntPtr));

        [DllImport(WIN_USER32_PATH, EntryPoint = MOUSE_EVENT_EP)]
        private static extern void  RightClick(uint dwFlags = MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP,
                                               uint dx      = MOUSE_CURRENT_POS,
                                               uint dy      = MOUSE_CURRENT_POS,
                                               int dwData   = NO_MOUSE_DATA,
                                               IntPtr dwExt = default(IntPtr)); 
        #endregion

        private MMDeviceEnumerator devices = new MMDeviceEnumerator();
        public MMDevice defaultSoundDevice { get { return devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia); } }

        public int VolumeDownBy(float value)
        {
            var minDecibels = defaultSoundDevice.AudioEndpointVolume.VolumeRange
                                                                    .MinDecibels;
            if ((defaultSoundDevice
                .AudioEndpointVolume
                .MasterVolumeLevel - value) > minDecibels)
                defaultSoundDevice.AudioEndpointVolume.MasterVolumeLevel -= value;
            else
                defaultSoundDevice.AudioEndpointVolume.MasterVolumeLevel = minDecibels;
                return FUNCTION_SUCCESS;
        }
        
        public int VolumeUpBy(float value)
        {
            var maxDecibles = defaultSoundDevice.AudioEndpointVolume.VolumeRange
                                                                    .MaxDecibels;
            if ((defaultSoundDevice
                .AudioEndpointVolume
                .MasterVolumeLevel + value) < maxDecibles)
                defaultSoundDevice.AudioEndpointVolume.MasterVolumeLevel += value;
            else
                defaultSoundDevice.AudioEndpointVolume.MasterVolumeLevel = maxDecibles;
                return FUNCTION_SUCCESS;
        }

        public float GetVolumeLevel() => defaultSoundDevice.AudioEndpointVolume.MasterVolumeLevel;

        public int isMixerMuted()
        {
            return Convert.ToInt32(defaultSoundDevice.AudioEndpointVolume.Mute);
        }

        public int muteMixer()
        {
            defaultSoundDevice.AudioEndpointVolume.Mute = true;
            return FUNCTION_SUCCESS;
        }

        public int unmuteMixer()
        {
            defaultSoundDevice.AudioEndpointVolume.Mute = false;
            return FUNCTION_SUCCESS;
        }

        public Point getMousePosition() => Cursor.Position;

        public int setMousePosition(Point mousePoint)
        {
            if(mousePoint != null)
              Cursor.Position = mousePoint;
            return FUNCTION_SUCCESS;
        }

        public int sendKeyStroke(string key)
        {
			if (key == null || key == string.Empty)
				return (int)FUNCTION_FAIL_RET;
            SendKeys.SendWait(key); // How i love .net devs...
            return FUNCTION_SUCCESS;
        }
    }
}
#endif
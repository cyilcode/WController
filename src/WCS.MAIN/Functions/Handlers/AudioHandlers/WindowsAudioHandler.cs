using System;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;
using WCS.MAIN.Interfaces.Windows.CoreAudioAPI;

namespace WCS.MAIN.Functions.Handlers.AudioHandlers
{
    public class WindowsAudioHandler : IAudioHandler
    {
        /*
            Windows Core Audio Interfaces more information here: 
            https://msdn.microsoft.com/en-us/library/windows/desktop/dd370805(v=vs.85).aspx
         */
        private const int                       D_TYPE_RENDER         = 0;
        private const int                       D_ROLE_MULTIMEDIA     = 1;
        private const int                       CLSCTX_INPROC_SERVER  = 1;
        private const int                       S_OK                  = 0;
        private const float                     SCALAR_MAX            = 100;
        private const float                     SCALAR_MIN            = 0;
        private int                             HRESULT               = 0;
        private readonly IAudioEndpointVolume   DEFAULT_ENDPOINT      = null;
        private readonly object                 FUNCTION_FAIL_RET     = 9998;

        public  bool HasDefaultEndpoint { get { return DEFAULT_ENDPOINT == null; } }

        public WindowsAudioHandler()
        {
            // Init the default audio endpoint.
            IMMDeviceEnumerator dEnum = (IMMDeviceEnumerator)new MMDeviceEnumerator();
            IMMDevice defaultDevice;
            dEnum.GetDefaultAudioEndpoint(D_TYPE_RENDER, 
                                          D_ROLE_MULTIMEDIA, 
                                          out defaultDevice);

            Guid IAudioEndpointVolumeTypeGuid = typeof(IAudioEndpointVolume).GUID;
            defaultDevice.Activate(ref IAudioEndpointVolumeTypeGuid, 
                                   CLSCTX_INPROC_SERVER, 
                                   IntPtr.Zero, 
                                   out DEFAULT_ENDPOINT);
        }
          
        public int GetVolumeLevel()
        {
            float pLevel;
            HRESULT = DEFAULT_ENDPOINT.GetMasterVolumeLevelScalar(out pLevel);
            if (HRESULT != S_OK)
            {
                GlobalHelper.log("WINAPI GetMasterVolumeLevelScalar failed with errcode: " + HRESULT);
                return (int)FUNCTION_FAIL_RET;
            }
            return Convert.ToInt32(pLevel * 100);
        }

        public int isMixerMuted()
        {
            bool mixerState;
            HRESULT = DEFAULT_ENDPOINT.GetMute(out mixerState);
            if (HRESULT != S_OK)
            {
                GlobalHelper.log("WINAPI GetMute failed with errcode: " + HRESULT);
                return (int)FUNCTION_FAIL_RET;
            }
            return Convert.ToInt32(mixerState);
        }

        public int muteMixer()
        {
            HRESULT = DEFAULT_ENDPOINT.SetMute(true);
            if (HRESULT != S_OK)
            {
                GlobalHelper.log("WINAPI GetMute failed with errcode: " + HRESULT);
                return (int)FUNCTION_FAIL_RET;
            }
            return S_OK;
        }

        public int unmuteMixer()
        {
            HRESULT = DEFAULT_ENDPOINT.SetMute(false);
            if (HRESULT != S_OK)
            {
                GlobalHelper.log("WINAPI GetMute failed with errcode: " + HRESULT);
                return (int)FUNCTION_FAIL_RET;
            }
            return S_OK;
        }

        public int VolumeDownBy(int value)
        {
            if (value > SCALAR_MAX || value < SCALAR_MIN)
            {
                GlobalHelper.log("Input value was too high or too low. Scalar input range: 0 - 100");
                return (int)FUNCTION_FAIL_RET;
            }
            int current_level = GetVolumeLevel();
            if (current_level == (int)FUNCTION_FAIL_RET)
            {
                GlobalHelper.log("Couldn't recieve current level from GetCurrentLevel()");
                return (int)FUNCTION_FAIL_RET;
            }
            float value_to_set = current_level - value;
            if (value_to_set < SCALAR_MIN)
                value_to_set = SCALAR_MIN;
            HRESULT = DEFAULT_ENDPOINT.SetMasterVolumeLevelScalar((value_to_set /= 100));
            if (HRESULT != S_OK)
            {
                GlobalHelper.log("WINAPI SetMasterVolumeLevelScalar failed with errcode: " + HRESULT);
                return (int)FUNCTION_FAIL_RET;
            }
            return S_OK;
        }

        public int VolumeUpBy(int value)
        {
            if (value > SCALAR_MAX || value < SCALAR_MIN)
            {
                GlobalHelper.log("Input value was too high or too low. Scalar input range: 0 - 100");
                return (int)FUNCTION_FAIL_RET;
            }
            int current_level = GetVolumeLevel();
            if (current_level == (int)FUNCTION_FAIL_RET)
            {
                GlobalHelper.log("Couldn't recieve current level from GetCurrentLevel()");
                return (int)FUNCTION_FAIL_RET;
            }
            float value_to_set = current_level + value;
            if (value_to_set > SCALAR_MAX)
                value_to_set = SCALAR_MAX;
            HRESULT = DEFAULT_ENDPOINT.SetMasterVolumeLevelScalar((value_to_set /= 100));
            if (HRESULT != S_OK)
            {
                GlobalHelper.log("WINAPI SetMasterVolumeLevelScalar failed with errcode: " + HRESULT);
                return (int)FUNCTION_FAIL_RET;
            }
            return S_OK;
        }
    }
}

#if DNX451
using Functions;
using NAudio.CoreAudioApi;
using System;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;
using Xunit;

namespace WCS.TEST
{
    public class winTests
    {
          /* Q: Why IFunctions here rather than a direct windowsFunction ?
             
           */

        private readonly IFunctions winFunctions = new windowsFunctions();
        private readonly MMDeviceEnumerator devices = new MMDeviceEnumerator();
        private const float WINDOWS_TEN_PERCENT = 1.6f;
        private const string Category = "Windows Functions Validations.";

        [CompatibleFact(OS.WINDOWS, false), Trait("Category", Category)]
        public void VolumeDownBy_turns_volume_down_by_value()
        {
            float newVolumeLevel = defaultDevice().AudioEndpointVolume
                                                  .MasterVolumeLevel - WINDOWS_TEN_PERCENT;
            winFunctions.VolumeDownBy(WINDOWS_TEN_PERCENT);
            Assert.Equal(newVolumeLevel, defaultDevice().AudioEndpointVolume.MasterVolumeLevel);
        }

        [CompatibleFact(OS.WINDOWS, false), Trait("Category", Category)]
        public void VolumeUpBy_turns_volume_up_by_value()
        {
            float newVolumeLevel = defaultDevice().AudioEndpointVolume
                                                  .MasterVolumeLevel + WINDOWS_TEN_PERCENT;
            // Since 0 is the max value for our mixer, if we get 0 + 1.6f make the variable 0 to pass the test.
            if (newVolumeLevel == WINDOWS_TEN_PERCENT) newVolumeLevel = 0;
            winFunctions.VolumeUpBy(WINDOWS_TEN_PERCENT);
            Assert.Equal(newVolumeLevel, defaultDevice().AudioEndpointVolume.MasterVolumeLevel);
        }

        [CompatibleFact(OS.WINDOWS, false), Trait("Category", Category)]
        public void GetVolumeLevel_gets_the_correct_value()
        {
            float fncLevel = winFunctions.GetVolumeLevel();
            Assert.Equal(fncLevel, defaultDevice().AudioEndpointVolume.MasterVolumeLevel);
        }

        [CompatibleFact(OS.WINDOWS, false), Trait("Category", Category)]
        public void isMixerMuted_returns_the_correct_value() => Assert.Equal(winFunctions.isMixerMuted(), 
                                                                             defaultDevice().AudioEndpointVolume
                                                                             .Mute);

        [CompatibleFact(OS.WINDOWS, false), Trait("Category", Category)]
        public void muteMixer_mutes_the_mixer()
        {
            // Get the mixer's current state
            bool mixerState = defaultDevice().AudioEndpointVolume.Mute;
            // Mute the mixer
            winFunctions.muteMixer();
            /* Here, test the data, did we really mute the mixer ?
               EX: mixerState:false(not muted), defaultDevice().AudioEndpointVolume.Mute: true -> Pass
               Same thing for else, mixerState:true(not muted), defaultDevice().AudioEndpointVolume.Mute: true -> Pass
             */
            if (mixerState)
                Assert.Equal(mixerState, defaultDevice().AudioEndpointVolume.Mute);
            else
                Assert.NotEqual(mixerState, defaultDevice().AudioEndpointVolume.Mute);
        }

        [CompatibleFact(OS.WINDOWS, false), Trait("Category", Category)]
        public void unmuteMixer_unmutes_the_mixer()
        {
            bool mixerState = defaultDevice().AudioEndpointVolume.Mute;
            winFunctions.unmuteMixer();
            if (mixerState)
                Assert.NotEqual(mixerState, defaultDevice().AudioEndpointVolume.Mute);
            else
                Assert.Equal(mixerState, defaultDevice().AudioEndpointVolume.Mute);
        }

        private MMDevice defaultDevice() => devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }
}
#endif
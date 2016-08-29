#if DNX451
using Functions;
using NAudio.CoreAudioApi;
using System;
using System.Drawing;
using System.Windows.Forms;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;
using Xunit;

namespace WCS.TEST
{
    public class winTests
    {
        private readonly IFunctions winFunctions    = new windowsFunctions();
        private readonly MMDeviceEnumerator devices = new MMDeviceEnumerator();
        private const float WINDOWS_TEN_PERCENT     = 1.6f;
        private const byte TEST_X                   = 200;
        private const byte TEST_Y                   = 200;
        private const string Category               = "Windows Functions Validations.";

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

        /* Mouse cursor should not be moving while testing this function.
            It would be very hard to make it fail but just in case. */
        [CompatibleFact(OS.WINDOWS, true), Trait("Category", Category)]
        public void getMousePosition_returns_correct_position()
        {
            var cpfPosition = winFunctions.getMousePosition();
            Assert.Equal(Cursor.Position, cpfPosition);
        }

        [CompatibleFact(OS.WINDOWS, true), Trait("Category", Category)]
        public void setMousePosition_sets_mouse_to_correct_position()
        {
            var newPoint = new Point(TEST_X, TEST_Y);
            winFunctions.setMousePosition(newPoint);
            Assert.Equal(Cursor.Position, newPoint);
        }

        private MMDevice defaultDevice() => devices.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
    }
}
#endif
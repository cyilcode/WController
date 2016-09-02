using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCS.MAIN.Functions;
using System.Reflection;
using WCS.MAIN.Globals;
using Xunit;

namespace WCS.TEST.Functions
{
    public class osxTests
    {
        /*
        ====================================================
        WARNING: THESE ARE PLATFORM TRIAL TESTS AND
        MOST OF THEM(PROBABLY) WILL BE RE-WRITTEN AS SOON AS
        I TRY OSX PLATFORM COMPATABILIES.
        ====================================================
         */
        private readonly osxFunctions osxFnc = new osxFunctions();
        private const uint NO_MIXER = 0;
        private const float VOLUME_TO_SET = 10;
        private const string Category = "OSX Functions tests";

        [CompatibleFact(OS.MACOSX, true), Trait("Category", Category)]
        public void get_mixer()
        {
            /* In common practice, you don't really test private functions but i'm a freak. */
            uint mixer = (uint)osxFnc.GetType().InvokeMember("get_mixer",
                                                             BindingFlags.Instance |
                                                             BindingFlags.InvokeMethod |
                                                             BindingFlags.NonPublic,
                                                             null, osxFnc, null);
            Assert.NotEqual(NO_MIXER, mixer);
        }

        [CompatibleFact(OS.MACOSX, true), Trait("Category", Category)]
        public void GetVolumeLevel_gets_the_correct_value()
        {
            float level = osxFnc.GetVolumeLevel();
            // If the function succeeds we always get a value between 0 - 100. Above or below that is not valid.
            Assert.Equal(true, (level >= osxFunctions.MIXER_MIN_VOL) &&
                               (level <= osxFunctions.MIXER_MAX_VOL));
        }

        [CompatibleFact(OS.MACOSX, true), Trait("Category", Category)]
        public void VolumeDownBy_turns_volume_down_by_value()
        {
            float cur_vol = osxFnc.GetVolumeLevel();
            osxFnc.VolumeDownBy(VOLUME_TO_SET);
            float expected_vol = cur_vol - VOLUME_TO_SET;
            Assert.Equal(expected_vol, osxFnc.GetVolumeLevel());
        }

        [CompatibleFact(OS.MACOSX, true), Trait("Category", Category)]
        public void VolumeUpBy_turns_volume_up_by_value()
        {
            float cur_vol = osxFnc.GetVolumeLevel();
            osxFnc.VolumeUpBy(VOLUME_TO_SET);
            float expected_vol = cur_vol + VOLUME_TO_SET;
            Assert.Equal(expected_vol, osxFnc.GetVolumeLevel());
        }

        [CompatibleFact(OS.MACOSX, true), Trait("Category", Category)]
        public void muteMixer_mutes_the_mixer()
        {
            bool switchStatus = osxFnc.isMixerMuted();
            if (!switchStatus)
            {
                osxFnc.muteMixer();
                Assert.NotEqual(switchStatus, osxFnc.isMixerMuted());
            }
            else // Will keep this for now.
                Console.WriteLine("mixer should not be muted to pass the muteMixer_mutes_the_mixer() test.");
        }

        [CompatibleFact(OS.MACOSX, true), Trait("Category", Category)]
        public void unmuteMixer_unmutes_the_mixer()
        {
            bool switchStatus = osxFnc.isMixerMuted();
            if (switchStatus)
            {
                osxFnc.unmuteMixer();
                Assert.NotEqual(switchStatus, osxFnc.isMixerMuted());
            }
            else
                Console.WriteLine("mixer should be muted to pass the unmuteMixer_unmutes_the_mixer() test.");
        }

        [CompatibleFact(OS.MACOSX, true), Trait("Category", Category)]
        public void isMixerMuted_returns_the_correct_value()
        {
            bool switchStatus = osxFnc.isMixerMuted();
            if (switchStatus)
            {
                osxFnc.unmuteMixer();
                Assert.NotEqual(switchStatus, osxFnc.isMixerMuted());
                osxFnc.muteMixer();
                Assert.Equal(switchStatus, osxFnc.isMixerMuted());
            }
            else
            {
                osxFnc.muteMixer();
                Assert.NotEqual(switchStatus, osxFnc.isMixerMuted());
                osxFnc.unmuteMixer();
                Assert.Equal(switchStatus, osxFnc.isMixerMuted());
            }
        }
    }
}

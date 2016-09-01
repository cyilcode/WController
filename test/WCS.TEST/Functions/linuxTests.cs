using System;
using WCS.MAIN.Functions;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;
using Xunit;

namespace WCS.TEST.linFunctions
{
    public class linuxTests
    {
        private readonly linuxFunctions linFunctions    = new linuxFunctions();
        private const sbyte INVALID_RANGE               = -1;
        private const sbyte RANGE_MINIMUM               = 0;
        private const sbyte RANGE_MAXIMUM               = 1;
        private const sbyte RANGE_PERCENT_MAX           = 100;
        private const sbyte RANGE_PERCENT_MIN           = 0;
        private const sbyte LINUX_TEN_PERCENT           = 10;
        private const string Category                   = "Linux Functions tests";

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void getMixer_returns_pointer()
        {
            Assert.Equal(ALSAERRCODE.NONE, linFunctions.ErrorCode);
            Assert.NotEqual(linFunctions.getMixer(), IntPtr.Zero);
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void getVolumeRange_returns_mixer_range()
        {
            Assert.Equal(ALSAERRCODE.NONE, linFunctions.ErrorCode);
            var ranges = linFunctions.getVolumeRange();
            Assert.NotEqual(ranges, null);
            Assert.NotEqual(ranges[RANGE_MINIMUM], INVALID_RANGE);
            Assert.NotEqual(ranges[RANGE_MAXIMUM], INVALID_RANGE);
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void GetVolumeLevel_gets_the_correct_value()
        {
            Assert.Equal(ALSAERRCODE.NONE, linFunctions.ErrorCode);
            var level = linFunctions.GetVolumeLevel();
            Assert.NotEqual(level, -1);
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void VolumeDownBy_turns_volume_down_by_value()
        {
            Assert.Equal(ALSAERRCODE.NONE, linFunctions.ErrorCode);
            var ranges = linFunctions.getVolumeRange();
            var level = linFunctions.GetVolumeLevel();
            linFunctions.VolumeDownBy(LINUX_TEN_PERCENT);
            if (level == RANGE_PERCENT_MIN)
                Assert.Equal(level, linFunctions.GetVolumeLevel());
            else
                Assert.Equal(level - LINUX_TEN_PERCENT, linFunctions.GetVolumeLevel());
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void VolumeUpBy_turns_volume_up_by_value()
        {
            Assert.Equal(ALSAERRCODE.NONE, linFunctions.ErrorCode);
            var ranges = linFunctions.getVolumeRange();
            var level = linFunctions.GetVolumeLevel();
            if (level == RANGE_PERCENT_MAX)
                Assert.Equal(level, linFunctions.GetVolumeLevel());
            else
            {
                var valToAdd = level + LINUX_TEN_PERCENT;
                if (valToAdd >= RANGE_PERCENT_MAX)
                    valToAdd = RANGE_PERCENT_MAX;
                else
                    valToAdd = LINUX_TEN_PERCENT;
                linFunctions.VolumeUpBy(valToAdd);
                Assert.Equal(valToAdd, linFunctions.GetVolumeLevel());
            }
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void muteMixer_mutes_the_mixer()
        {
            Assert.Equal(ALSAERRCODE.NONE, linFunctions.ErrorCode);
            var switchStatus = linFunctions.isMixerMuted();
            if (switchStatus)
            {
                linFunctions.muteMixer();
                Assert.Equal(switchStatus, linFunctions.isMixerMuted());
            }
            else
            {
                linFunctions.muteMixer();
                Assert.NotEqual(switchStatus, linFunctions.isMixerMuted());
            }
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void unmuteMixer_mutes_the_mixer()
        {
            Assert.Equal(ALSAERRCODE.NONE, linFunctions.ErrorCode);
            var switchStatus = linFunctions.isMixerMuted();
            if (switchStatus)
            {
                linFunctions.unmuteMixer();
                Assert.NotEqual(switchStatus, linFunctions.isMixerMuted());
            }
            else
            {
                linFunctions.unmuteMixer();
                Assert.Equal(switchStatus, linFunctions.isMixerMuted());
            }
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void isMixerMuted_returns_the_correct_value()
        {
            Assert.Equal(ALSAERRCODE.NONE, linFunctions.ErrorCode);
            var switchStatus = linFunctions.isMixerMuted();
            if (switchStatus)
            {
                linFunctions.unmuteMixer();
                Assert.NotEqual(switchStatus, linFunctions.isMixerMuted());
                linFunctions.muteMixer();
                Assert.Equal(switchStatus, linFunctions.isMixerMuted());
            }
            else
            {
                linFunctions.muteMixer();
                Assert.NotEqual(switchStatus, linFunctions.isMixerMuted());
                linFunctions.unmuteMixer();
                Assert.Equal(switchStatus, linFunctions.isMixerMuted());
            }
        }
    }
}

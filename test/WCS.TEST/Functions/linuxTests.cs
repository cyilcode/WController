using System;
using WCS.MAIN.Functions;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;
using Xunit;

namespace WCS.TEST.Functions
{
    public class linuxTests
    {
        private const sbyte INVALID_RANGE = -1;
        private const sbyte RANGE_MINIMUM = 0;
        private const sbyte RANGE_MAXIMUM = 1;
        private const sbyte RANGE_PERCENT_MAX = 100;
        private const sbyte RANGE_PERCENT_MIN = 0;
        private const sbyte LINUX_TEN_PERCENT = 10;
        private const string Category = "Linux functions test";
        private readonly linuxFunctions functions = new linuxFunctions();

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void getMixer_returns_pointer()
        {
            Assert.Equal(ALSAERRCODE.NONE, functions.ErrorCode);
            Assert.NotEqual(functions.getMixer(), IntPtr.Zero);
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void getVolumeRange_returns_mixer_range()
        {
            Assert.Equal(ALSAERRCODE.NONE, functions.ErrorCode);
            var ranges = functions.getVolumeRange();
            Assert.NotEqual(ranges, null);
            Assert.NotEqual(ranges[RANGE_MINIMUM], INVALID_RANGE);
            Assert.NotEqual(ranges[RANGE_MAXIMUM], INVALID_RANGE);
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void GetVolumeLevel_gets_the_correct_value()
        {
            Assert.Equal(ALSAERRCODE.NONE, functions.ErrorCode);
            var level = functions.GetVolumeLevel();
            Assert.NotEqual(level, -1);
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void VolumeDownBy_turns_volume_down_by_value()
        {
            Assert.Equal(ALSAERRCODE.NONE, functions.ErrorCode);
            var ranges = functions.getVolumeRange();
            var level = functions.GetVolumeLevel();
            functions.VolumeDownBy(LINUX_TEN_PERCENT);
            if (level == RANGE_PERCENT_MIN)
                Assert.Equal(level, functions.GetVolumeLevel());
            else
                Assert.Equal(level - LINUX_TEN_PERCENT, functions.GetVolumeLevel());
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void VolumeUpBy_turns_volume_up_by_value()
        {
            Assert.Equal(ALSAERRCODE.NONE, functions.ErrorCode);
            var ranges = functions.getVolumeRange();
            var level = functions.GetVolumeLevel();
            if (level == RANGE_PERCENT_MAX)
                Assert.Equal(level, functions.GetVolumeLevel());
            else
            {
                var valToAdd = level + LINUX_TEN_PERCENT;
                if (valToAdd >= RANGE_PERCENT_MAX)
                    valToAdd = RANGE_PERCENT_MAX;
                else
                    valToAdd = LINUX_TEN_PERCENT;
                functions.VolumeUpBy(valToAdd);
                Assert.Equal(valToAdd, functions.GetVolumeLevel());
            }
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void muteMixer_mutes_the_mixer()
        {
            Assert.Equal(ALSAERRCODE.NONE, functions.ErrorCode);
            var switchStatus = functions.isMixerMuted();
            if (switchStatus)
            {
                functions.muteMixer();
                Assert.Equal(switchStatus, functions.isMixerMuted());
            }
            else
            {
                functions.muteMixer();
                Assert.NotEqual(switchStatus, functions.isMixerMuted());
            }
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void unmuteMixer_mutes_the_mixer()
        {
            Assert.Equal(ALSAERRCODE.NONE, functions.ErrorCode);
            var switchStatus = functions.isMixerMuted();
            if (switchStatus)
            {
                functions.unmuteMixer();
                Assert.NotEqual(switchStatus, functions.isMixerMuted());
            }
            else
            {
                functions.unmuteMixer();
                Assert.Equal(switchStatus, functions.isMixerMuted());
            }
        }

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void isMixerMuted_returns_the_correct_value()
        {
            Assert.Equal(ALSAERRCODE.NONE, functions.ErrorCode);
            var switchStatus = functions.isMixerMuted();
            if (switchStatus)
            {
                functions.unmuteMixer();
                Assert.NotEqual(switchStatus, functions.isMixerMuted());
                functions.muteMixer();
                Assert.Equal(switchStatus, functions.isMixerMuted());
            }
            else
            {
                functions.muteMixer();
                Assert.Equal(switchStatus, functions.isMixerMuted());
                functions.unmuteMixer();
                Assert.NotEqual(switchStatus, functions.isMixerMuted());
            }
        }
    }
}

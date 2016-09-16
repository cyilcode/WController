using System;
using WCS.MAIN.Functions;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;
using Xunit;

namespace WCS.TEST.linFunctions
{
    public class linuxTests
    {
        private readonly linuxFunctions linFunctions    = new linuxFunctions(new Settings());
        private const sbyte LINUX_TEN_PERCENT           = 10;
        private const string Category                   = "Linux Functions tests";

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void getMixer_returns_pointer() => 
            Assert.NotEqual(linFunctions.getMixer(), IntPtr.Zero);

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void getVolumeRange_returns_mixer_range() =>
            Assert.NotNull(linFunctions.getVolumeRange());

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void GetVolumeLevel_gets_the_correct_value() => // Revise this.
            Assert.NotEqual(linFunctions.GetVolumeLevel(), 
                            Convert.ToSingle(linFunctions.FUNCTION_FAIL_RET));

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void VolumeDownBy_turns_volume_down_by_value() =>
            Assert.NotEqual(linFunctions.VolumeDownBy(LINUX_TEN_PERCENT), 
                           (int)linFunctions.FUNCTION_FAIL_RET);

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void VolumeUpBy_turns_volume_up_by_value() =>
            Assert.NotEqual(linFunctions.VolumeUpBy(LINUX_TEN_PERCENT), 
                           (int)linFunctions.FUNCTION_FAIL_RET);

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void muteMixer_mutes_the_mixer() =>
            Assert.NotEqual(linFunctions.isMixerMuted(), 
                           (int)linFunctions.FUNCTION_FAIL_RET);

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void unmuteMixer_unmutes_the_mixer() =>
            Assert.NotEqual(linFunctions.unmuteMixer(), 
                           (int)linFunctions.FUNCTION_FAIL_RET);

        [CompatibleFact(OS.LINUX, false), Trait("Category", Category)]
        public void isMixerMuted_returns_the_correct_value() =>
            Assert.NotEqual(linFunctions.isMixerMuted(), 
                           (int)linFunctions.FUNCTION_FAIL_RET);
		[CompatibleFact(OS.LINUX, true), Trait("Category", Category)]
		public void getmouseposition_returns_position () =>
			Assert.NotNull(linFunctions.getMousePosition());
    }
}

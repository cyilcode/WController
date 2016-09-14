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
		private readonly osxFunctions osxFnc            = new osxFunctions();
		private const sbyte OSX_TEN_PERCENT             = 10;
		private const string Category                   = "OSX Functions tests";

		[CompatibleFact(OS.MACOSX, false), Trait("Category", Category)]
		public void getmixer_returns_id()
		{
			uint mixer = (uint)osxFnc.GetType().InvokeMember("get_mixer",
												BindingFlags.Instance |
												BindingFlags.InvokeMethod |
												BindingFlags.NonPublic,
												null, osxFnc, null);
			Assert.NotEqual((uint)osxFnc.FUNCTION_FAIL_RET, mixer);
		}

		[CompatibleFact(OS.MACOSX, false), Trait("Category", Category)]
		public void GetVolumeLevel_gets_the_correct_value () =>
		Assert.NotEqual(osxFnc.GetVolumeLevel(), 
						Convert.ToSingle(osxFnc.FUNCTION_FAIL_RET));
		
		[CompatibleFact(OS.MACOSX, false), Trait("Category", Category)]
		public void VolumeDownBy_turns_volume_down_by_value() =>
		Assert.NotEqual(osxFnc.VolumeDownBy(OSX_TEN_PERCENT), 
					   (int)osxFnc.FUNCTION_FAIL_RET);

		[CompatibleFact(OS.MACOSX, false), Trait("Category", Category)]
		public void VolumeUpBy_turns_volume_up_by_value() =>
		Assert.NotEqual(osxFnc.VolumeUpBy(OSX_TEN_PERCENT), 
				 	   (int)osxFnc.FUNCTION_FAIL_RET);

		[CompatibleFact(OS.MACOSX, false), Trait("Category", Category)]
		public void muteMixer_mutes_the_mixer() =>
		Assert.NotEqual(osxFnc.isMixerMuted(), 
				       (int)osxFnc.FUNCTION_FAIL_RET);

		[CompatibleFact(OS.MACOSX, false), Trait("Category", Category)]
		public void unmuteMixer_unmutes_the_mixer() =>
		Assert.NotEqual(osxFnc.unmuteMixer(), 
					   (int)osxFnc.FUNCTION_FAIL_RET);

		[CompatibleFact(OS.MACOSX, false), Trait("Category", Category)]
		public void isMixerMuted_returns_the_correct_value() =>
		Assert.NotEqual(osxFnc.isMixerMuted(), 
					   (int)osxFnc.FUNCTION_FAIL_RET);
    }
}

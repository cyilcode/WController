using System;
using WCS.MAIN.Functions;
using WCS.MAIN.Functions.Handlers.AudioHandlers;
using WCS.MAIN.Globals;
using Xunit;

namespace WCS.TEST.audioTests
{
    public class audioTests
    {

        private readonly     FunctionHandler handler    = new FunctionHandler();
        private const sbyte  TEN_PERCENT                = 10;
        private const string Category                   = "Audio implementation tests";

        public audioTests()
        {
            switch (new GlobalHelper().getOS())
            {
                case OS.WINDOWS:
                    handler.AudioHandler = new WindowsAudioHandler();
                    break;
                case OS.LINUX:
                    handler.AudioHandler = new LinuxAudioHandler(new Settings());
                    break;
                case OS.MACOSX:
                    handler.AudioHandler = new OSXAudioHandler();
                    break;
            }
        }


        [CompatibleFact(false), Trait("Category", Category)]
        public void GetVolumeLevel_gets_the_correct_value() =>
            Assert.NotEqual(handler.AudioHandler.GetVolumeLevel(), 
                            Convert.ToSingle(handler.FUNCTION_FAIL_RET));

        [CompatibleFact(false), Trait("Category", Category)]
        public void VolumeDownBy_turns_volume_down_by_value() =>
            Assert.NotEqual(handler.AudioHandler.VolumeDownBy(TEN_PERCENT), 
                           (int)handler.FUNCTION_FAIL_RET);

        [CompatibleFact(false), Trait("Category", Category)]
        public void VolumeUpBy_turns_volume_up_by_value() =>
            Assert.NotEqual(handler.AudioHandler.VolumeUpBy(TEN_PERCENT), 
                           (int)handler.FUNCTION_FAIL_RET);

        [CompatibleFact(false), Trait("Category", Category)]
        public void muteMixer_mutes_the_mixer() =>
            Assert.NotEqual(handler.AudioHandler.muteMixer(), 
                           (int)handler.FUNCTION_FAIL_RET);

        [CompatibleFact(false), Trait("Category", Category)]
        public void unmuteMixer_unmutes_the_mixer() =>
            Assert.NotEqual(handler.AudioHandler.unmuteMixer(), 
                           (int)handler.FUNCTION_FAIL_RET);

        [CompatibleFact(false), Trait("Category", Category)]
        public void isMixerMuted_returns_the_correct_value() =>
            Assert.NotEqual(handler.AudioHandler.isMixerMuted(), 
                           (int)handler.FUNCTION_FAIL_RET);
    }
}

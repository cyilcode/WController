using WCS.MAIN.Functions;
using WCS.MAIN.Functions.Handlers.AudioHandlers;
using WCS.MAIN.Globals;
using Xunit;

namespace WCS.TEST
{
    public class CompatibleFactAttribute : FactAttribute
    {
        private const string WRONG_PLATFORM             = "Your platform is not suitable for this test !";
        private const string NO_MASTER_MIXER            = "Couldn't find a master mixer !";
        private const string NO_DESKTOP                 = "Your platform doesn't have a desktop !";
        private const string NO_ALSA                    = "Linux Audio Tests requires AlsaMixer and libraries.";
        private const string NO_SOUND_CARD              = "Your system doesn't have a soundcard.";
        private const int    NODESKTOP_HRESULT          = -2146233036;
        private readonly     OS platform;
        private FunctionHandler handler;

        public CompatibleFactAttribute(bool requiresDesktop)
        {
            GlobalHelper        hlp      = new GlobalHelper();
                                handler  = Utils.prepare_function_handler();
            platform = hlp.getOS();

                if (!platformCheck(hlp))
                    Skip += " " + WRONG_PLATFORM;
                if (platform == OS.WINDOWS && masterMixerEnabled())
                    Skip += " " + NO_MASTER_MIXER;
                if (platform == OS.LINUX && !checkALSA(hlp))
                    Skip += " " + NO_ALSA;
                if (platform == OS.LINUX && !checkSoundCard(hlp))
                    Skip += " " + NO_SOUND_CARD;
            if (requiresDesktop && !isDesktop())
                Skip += " " + NO_DESKTOP;
        }

        private bool checkALSA(GlobalHelper gHelper) => gHelper.execute_shell_command("dpkg", "-l | grep libasound2-dev").Contains("libasound2-dev");

        private bool checkSoundCard(GlobalHelper gHelper)
        {
            string output = gHelper.execute_shell_command("arecord", "-l");
            if (output != string.Empty)
                if (!output.Contains("no soundcards found"))
                    return true;
            return false;
        }

        private bool platformCheck(GlobalHelper gHelper)
        {
            var detectedOS = gHelper.getOS();
            if (detectedOS != platform) return false;
            return true;
        }

        private bool masterMixerEnabled() => new WindowsAudioHandler().HasDefaultEndpoint;

        private bool isDesktop()
        {
            var exp = Record.Exception(() => handler.InputHandler.getMousePosition());
            if (exp != null && exp.HResult == NODESKTOP_HRESULT)
                return false;
            return true;
        }
    }
}

using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.IO;
using WCS.MAIN.Functions;
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

        public CompatibleFactAttribute(OS id, bool requiresDesktop)
        {
            platform = id;
            if (id != OS.CROSSPLATFORM) // No need to check any platform and master mixer for cross platform functions.
            {
                if (!platformCheck())
                    Skip += " " + WRONG_PLATFORM;
                if (id == OS.WINDOWS && !masterMixerEnabled())
                    Skip += " " + NO_MASTER_MIXER;
                if (id == OS.LINUX && !checkALSA())
                    Skip += " " + NO_ALSA;
                if (id == OS.LINUX && !checkSoundCard())
                    Skip += " " + NO_SOUND_CARD;
            }
            if (requiresDesktop && !isDesktop())
                Skip += " " + NO_DESKTOP;
        }

        private bool checkALSA() =>
               File.Exists("/usr/lib/x86_64-linux-gnu/libasound.so.2")
            || File.Exists("/usr/lib/i386-linux-gnu/libasound.so.2");

        private bool checkSoundCard()
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "arecord";
            startInfo.Arguments = "-l";
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            var commandProcess = Process.Start(startInfo);
            var output = commandProcess.StandardOutput.ReadToEnd();
            if (output != null || output != string.Empty)
                if (!output.Contains("no soundcards found"))
                    return true;
            return false;
        }

        private bool platformCheck()
        {
            var gHelper = new GlobalHelper();
            var detectedOS = gHelper.getOS();
            if (detectedOS != platform) return false;
            return true;
        }

        private bool masterMixerEnabled()
        {
            var devices = new MMDeviceEnumerator();
            return devices.HasDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        private bool isDesktop()
        {
            var function = new linuxFunctions();
            var exp = Record.Exception(() => function.getMousePosition());
            if (exp != null && exp.HResult == NODESKTOP_HRESULT)
                return false;
            return true;
        }
    }
}

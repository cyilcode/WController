using System;
using System.Runtime.InteropServices;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;

namespace WCS.MAIN.Functions.Handlers.AudioHandlers
{
    public class LinuxAudioHandler : IAudioHandler
    {
        private IntPtr                  mixerHandle = IntPtr.Zero;                   // Main mixer handle
        private const string            DEFAULT_SOUND_CARD_KEY          = "default_sound_card";
        private const string            MIXER_NAME_KEY                  = "default_mixer_name";
        private const string            LIBASOUND_LIB_PATH              = "libasound.so.2";
        private const sbyte             MODE_DEFAULT                    = 0;         // Open mode
        private const sbyte             RANGE_MINIMUM                   = 0;         // Scalar minimum level
        private const sbyte             RANGE_MAXIMUM                   = 1;         // Scalar maximum level
        private const sbyte             RANGE_PERCENT_MAX               = 100;
        private const sbyte             RANGE_PERCENT_MIN               = 0;
        private const sbyte             INVALID_RANGE                   = -1;
        private const sbyte             PERCENT                         = 100;
        private const sbyte             ROUND                           = 1;
        private const sbyte             ALSA_SUCCESS                    = 0;
        private const int               INDEX_ZERO                      = 0;
        private const int               SND_MIXER_SCHN_FRONT_LEFT       = 1;
        private const int               MUTE_MIXER                      = 0;
        private const int               UNMUTE_MIXER                    = 1;
        private int                     ret                             = 0;
        public readonly object          FUNCTION_FAIL_RET               = 9998;
        private readonly Settings       g_Settings;

        /*
            More information on : http://www.alsa-project.org/alsa-doc/alsa-lib/files.html
            Please refer to the link above for more information on these functions aswell.
            libasound2 P/Invoke.
            PKG Required: libasound2-dev
        */

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_open(ref IntPtr mixer, int mode);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_attach(IntPtr handle, string soundcard);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_selem_register(IntPtr mixer, IntPtr options, IntPtr classp);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_load(IntPtr mixer);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern void snd_mixer_selem_id_malloc(ref IntPtr ptr);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern void snd_mixer_selem_id_set_index(IntPtr obj, uint val);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern void snd_mixer_selem_id_set_name(IntPtr handle, string val);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern IntPtr snd_mixer_find_selem(IntPtr mixer, IntPtr id);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_selem_get_playback_volume_range(IntPtr elem, ref long min, ref long max);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_selem_set_playback_volume_all(IntPtr elem, long value);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_selem_has_playback_switch(IntPtr elem);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_selem_get_playback_switch(IntPtr elem, int channel, ref int value);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_selem_set_playback_switch_all(IntPtr elem, int value);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_close(IntPtr mixer);

        [DllImport(LIBASOUND_LIB_PATH)]
        static extern int snd_mixer_selem_get_playback_volume(IntPtr elem, int channel, ref long value);

        public LinuxAudioHandler(Settings set)
        {
            g_Settings = set;
        }

        public int GetVolumeLevel()
        {
            long level = 0;
            var ranges = getVolumeRange();
            ret = snd_mixer_selem_get_playback_volume(getMixer(), SND_MIXER_SCHN_FRONT_LEFT, ref level);
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_selem_get_playback_volume failed with errcode: " + ret);
                return (int)FUNCTION_FAIL_RET;
            }
            return (int)(level * PERCENT / ranges[RANGE_MAXIMUM]);
        }

        public int isMixerMuted()
        {
            var mixer = getMixer();
            var val = 0;
            var hasSwitch = Convert.ToBoolean(snd_mixer_selem_has_playback_switch(mixer));
            if (!hasSwitch)
            {
                GlobalHelper.log("snd_mixer_selem_has_playback_switch failed. No playback switch found.");
                return (int)FUNCTION_FAIL_RET;
            }
            ret = snd_mixer_selem_get_playback_switch(mixer, SND_MIXER_SCHN_FRONT_LEFT, ref val);
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_selem_get_playback_switch failed with errcode: " + ret);
                return (int)FUNCTION_FAIL_RET;
            }
            /* get_playback_switch returns the status of the toggle. If it retuns 1 means mixer is not muted.
               To make it same with windows and mac osx. If we get 0, that means mixer is muted and we should return true.
               Basically, we reversed the return data. 
             */
            if (val == MUTE_MIXER)
                return UNMUTE_MIXER; // Revise here
            else
                return MUTE_MIXER;
        }

        public int muteMixer()
        {
            ret = snd_mixer_selem_set_playback_switch_all(getMixer(), MUTE_MIXER);
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_selem_set_playback_switch_all failed with errcode: " + ret);
                return (int)FUNCTION_FAIL_RET;
            }
            return ret;
        }

        public int unmuteMixer()
        {
            ret = snd_mixer_selem_set_playback_switch_all(getMixer(), UNMUTE_MIXER);
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_selem_set_playback_switch_all failed with errcode: " + ret);
                return (int)FUNCTION_FAIL_RET;
            }
            return ret;
        }

        public int VolumeDownBy(int value)
        {
            long[] ranges = getVolumeRange();
            float level = GetVolumeLevel();
            if (level <= RANGE_PERCENT_MIN) return ALSA_SUCCESS; // Revise here
            var valueToSet = ((long)(level - value) * ranges[RANGE_MAXIMUM] / PERCENT) + ROUND;
            ret = snd_mixer_selem_set_playback_volume_all(getMixer(), valueToSet);
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_selem_set_playback_volume_all failed with errcode: " + ret);
                return (int)FUNCTION_FAIL_RET;
            }
            return ret;
        }

        public int VolumeUpBy(int value)
        {
            long valueToSet = 0;
            long[] ranges = getVolumeRange();
            float level = GetVolumeLevel();
            if (value >= RANGE_PERCENT_MAX) return ALSA_SUCCESS; // Revise here
            else
                valueToSet = ((long)(level + value) * ranges[RANGE_MAXIMUM] / PERCENT) + ROUND;
            ret = snd_mixer_selem_set_playback_volume_all(getMixer(), valueToSet);
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_selem_set_playback_volume_all failed with errcode: " + ret);
                return (int)FUNCTION_FAIL_RET;
            }
            return ret;
        }

        public long[] getVolumeRange()
        {
            long min = INVALID_RANGE;
            long max = INVALID_RANGE;
            var mixer = getMixer();
            ret = snd_mixer_selem_get_playback_volume_range(getMixer(), ref min, ref max);
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_selem_get_playback_volume_range failed with errcode: " + ret);
                return null;
            }
            return new long[] { min, max };
        }

        public IntPtr getMixer()
        {
            var sID = IntPtr.Zero;
            var mixer = IntPtr.Zero;
            var simpleElement = IntPtr.Zero;
            ret = snd_mixer_open(ref mixerHandle, MODE_DEFAULT);
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_open failed with errcode: " + ret);
                return IntPtr.Zero;
            }
            ret = snd_mixer_attach(mixerHandle, g_Settings.confQuery<string>(DEFAULT_SOUND_CARD_KEY));
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_attach failed with errcode: " + ret);
                return IntPtr.Zero;
            }
            ret = snd_mixer_selem_register(mixerHandle, IntPtr.Zero, IntPtr.Zero);
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_selem_register failed with errcode: " + ret);
                return IntPtr.Zero;
            }
            ret = snd_mixer_load(mixerHandle);
            if (ret != ALSA_SUCCESS)
            {
                GlobalHelper.log("snd_mixer_load failed with errcode: " + ret);
                return IntPtr.Zero;
            }
            snd_mixer_selem_id_malloc(ref sID);
            snd_mixer_selem_id_set_index(sID, INDEX_ZERO);
            snd_mixer_selem_id_set_name(sID, g_Settings.confQuery<string>(MIXER_NAME_KEY));
            simpleElement = snd_mixer_find_selem(mixerHandle, sID);
            if (simpleElement != IntPtr.Zero)
                return simpleElement;
            else
                GlobalHelper.log("snd_mixer_find_selem failed with errcode: " + ret);
            return IntPtr.Zero;
        }
    }
}

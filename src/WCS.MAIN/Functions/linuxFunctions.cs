using System;
using System.Runtime.InteropServices;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;

namespace WCS.MAIN.Functions
{
    public class linuxFunctions : IFunctions
    {
        private IntPtr mixerHandle = IntPtr.Zero;
        private const string defaultSoundCard = "default";
        private const string mixerName = "Master";
        private const sbyte MODE_DEFAULT = 0;
        private const sbyte RANGE_MINIMUM = 0;
        private const sbyte RANGE_MAXIMUM = 1;
        private const sbyte RANGE_PERCENT_MAX = 100;
        private const sbyte RANGE_PERCENT_MIN = 0;
        private const sbyte INVALID_RANGE = -1;
        private const sbyte PERCENT = 100;
        private const sbyte ROUND = 1;
        private const sbyte ALSA_SUCCESS = 0;
        private const int INDEX_ZERO = 0;
        private const int SND_MIXER_SCHN_FRONT_LEFT = 1;
        private const int MUTE_MIXER = 0;
        private const int UNMUTE_MIXER = 1;
        private int ret = 0;
        public ALSAERRCODE ErrorCode { get; set; }
        /*
          libasound2 P/Invoke.
          PKG Required: libasound2-dev
         */
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_open(ref IntPtr mixer, int mode);
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_attach(IntPtr handle, string soundcard);
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_selem_register(IntPtr mixer, IntPtr options, IntPtr classp);
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_load(IntPtr mixer);
        [DllImport("libasound.so.2")]
        static extern void snd_mixer_selem_id_malloc(ref IntPtr ptr);
        [DllImport("libasound.so.2")]
        static extern void snd_mixer_selem_id_set_index(IntPtr obj, uint val);
        [DllImport("libasound.so.2")]
        static extern void snd_mixer_selem_id_set_name(IntPtr handle, string val);
        [DllImport("libasound.so.2")]
        static extern IntPtr snd_mixer_find_selem(IntPtr mixer, IntPtr id);
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_selem_get_playback_volume_range(IntPtr elem, ref long min, ref long max);
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_selem_set_playback_volume_all(IntPtr elem, long value);
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_selem_has_playback_switch(IntPtr elem);
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_selem_get_playback_switch(IntPtr elem, int channel, ref int value);
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_selem_set_playback_switch_all(IntPtr elem, int value);
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_close(IntPtr mixer);
        [DllImport("libasound.so.2")]
        static extern int snd_mixer_selem_get_playback_volume(IntPtr elem, int channel, ref long value);

        public float GetVolumeLevel()
        {
            long level = 0;
            var ranges = getVolumeRange();
            ret = snd_mixer_selem_get_playback_volume(getMixer(), SND_MIXER_SCHN_FRONT_LEFT, ref level);
            if (ret != ALSA_SUCCESS) { ErrorCode = ALSAERRCODE.GET_PLAYBACK_VOLUME; return -1; }
            return level * PERCENT / ranges[RANGE_MAXIMUM];
        }

        public bool isMixerMuted()
        {
            var mixer = getMixer();
            var val = 0;
            var hasSwitch = Convert.ToBoolean(snd_mixer_selem_has_playback_switch(mixer));
            if (!hasSwitch) return false; // TODO: Expand behaviour.
            ret = snd_mixer_selem_get_playback_switch(mixer, SND_MIXER_SCHN_FRONT_LEFT, ref val);
            if (ret != ALSA_SUCCESS) { ErrorCode = ALSAERRCODE.GET_PLAYBACK_SWITCH; return false; }
            /* get_playback_switch returns the status of the toggle. If its on(not muted, returns 1)
               To make it same with windows and mac osx. If we get 0, that means mixer is muted and we should return true.
               Basically, we reversed the return data. 
             */
            if (val == MUTE_MIXER)
                return true;
            else
                return false;
        }

        public void muteMixer()
        {
            if (isMixerMuted()) return;
            var mixer = getMixer();
            ret = snd_mixer_selem_set_playback_switch_all(mixer, MUTE_MIXER);
            if (ret != ALSA_SUCCESS)
                ErrorCode = ALSAERRCODE.SET_PLAYBACK_SWITCH_ALL;
        }

        public void unmuteMixer()
        {
            if (!isMixerMuted()) return;
            var mixer = getMixer();
            ret = snd_mixer_selem_set_playback_switch_all(mixer, UNMUTE_MIXER);
        }

        public void VolumeDownBy(float value)
        {
            var ranges = getVolumeRange();
            var level = GetVolumeLevel();
            if (level <= RANGE_PERCENT_MIN) return;
            var mixer = getMixer();
            var valueToSet = ((long)(level - value) * ranges[RANGE_MAXIMUM] / PERCENT) + ROUND;
            ret = snd_mixer_selem_set_playback_volume_all(mixer, valueToSet);
            if (ret != ALSA_SUCCESS)
                ErrorCode = ALSAERRCODE.SET_PLAYBACK_VOLUME_ALL;
        }

        public void VolumeUpBy(float value)
        {
            long valueToSet = 0;
            var ranges = getVolumeRange();
            var level = GetVolumeLevel();
            var mixer = getMixer();
            if (value >= RANGE_PERCENT_MAX)
                valueToSet = ranges[RANGE_MAXIMUM];
            else
                valueToSet = ((long)(level + value) * ranges[RANGE_MAXIMUM] / PERCENT) + ROUND;
            ret = snd_mixer_selem_set_playback_volume_all(mixer, valueToSet);
            if (ret != ALSA_SUCCESS)
                ErrorCode = ALSAERRCODE.SET_PLAYBACK_VOLUME_ALL;
        }

        public long[] getVolumeRange()
        {
            long min = INVALID_RANGE;
            long max = INVALID_RANGE;
            var mixer = getMixer();
            ret = snd_mixer_selem_get_playback_volume_range(mixer, ref min, ref max);
            if (ret != ALSA_SUCCESS) { ErrorCode = ALSAERRCODE.GET_PLAYBACK_VOLUME_RANGE; return null; }
            return new long[] { min, max };
        }

        public IntPtr getMixer()
        {
            var sID = IntPtr.Zero;
            var mixer = IntPtr.Zero;
            var simpleElement = IntPtr.Zero;
            ret = snd_mixer_open(ref mixerHandle, MODE_DEFAULT);
            if (ret != ALSA_SUCCESS) { ErrorCode = ALSAERRCODE.MIXER_OPEN; return IntPtr.Zero; }
            ret = snd_mixer_attach(mixerHandle, defaultSoundCard);
            if (ret != ALSA_SUCCESS) { ErrorCode = ALSAERRCODE.MIXER_ATTACH; return IntPtr.Zero; }
            ret = snd_mixer_selem_register(mixerHandle, IntPtr.Zero, IntPtr.Zero);
            if (ret != ALSA_SUCCESS) { ErrorCode = ALSAERRCODE.MIXER_SELEM_REGISTER; return IntPtr.Zero; }
            ret = snd_mixer_load(mixerHandle);
            if (ret != ALSA_SUCCESS) { ErrorCode = ALSAERRCODE.MIXER_LOAD; return IntPtr.Zero; }
            snd_mixer_selem_id_malloc(ref sID);
            snd_mixer_selem_id_set_index(sID, INDEX_ZERO);
            snd_mixer_selem_id_set_name(sID, mixerName);
            simpleElement = snd_mixer_find_selem(mixerHandle, sID);
            if (simpleElement != IntPtr.Zero)
                return simpleElement;
            else
                ErrorCode = ALSAERRCODE.FIND_SELEM;
            return IntPtr.Zero;
        }
    }
}

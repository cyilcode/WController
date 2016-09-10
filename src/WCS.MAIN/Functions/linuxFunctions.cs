using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;

#region typedefs
// not needed at all but i always wanted to use these badboys :)
using xdo_t  = System.IntPtr;
using Window = System.UInt64;
#endregion

namespace WCS.MAIN.Functions
{
    public class linuxFunctions : IFunctions
    {
        private IntPtr              mixerHandle                        = IntPtr.Zero;           // Main mixer handle
        private const string        DEFAULT_SOUND_CARD_KEY             = "default_sound_card";
        private const string        MIXER_NAME_KEY                     = "default_mixer_name";
        private const string        LIBASOUND_LIB_PATH                 = "libasound.so.2";
        private const string        LIBXDO_LIB_PATH                    = "libxdo.so.3";
        private const sbyte         MODE_DEFAULT                       = 0;                     // Open mode
        private const sbyte         RANGE_MINIMUM                      = 0;                     // Scalar minimum level
        private const sbyte         RANGE_MAXIMUM                      = 1;                     // Scalar maximum level
        private const sbyte         RANGE_PERCENT_MAX                  = 100;
        private const sbyte         RANGE_PERCENT_MIN                  = 0;
        private const sbyte         INVALID_RANGE                      = -1;
        private const sbyte         PERCENT                            = 100;
        private const sbyte         ROUND                              = 1;
        private const sbyte         ALSA_SUCCESS                       = 0;
        private const sbyte         LIBXDO_SUCCESS                     = 0;
        private const int           INDEX_ZERO                         = 0;
        private const int           SND_MIXER_SCHN_FRONT_LEFT          = 1;
        private const int           MUTE_MIXER                         = 0;
        private const int           UNMUTE_MIXER                       = 1;
        private const int           MOUSE_BUTTON_LEFT                  = 1;
        private const int           MOUSE_BUTTON_MIDDLE                = 2;
        private const int           MOUSE_BUTTON_RIGHT                 = 3;
        private const int           MOUSE_BUTTON_WHEELUP               = 4;                     // Should figure out horizontal wheel aswell. instead of sending left & right keys.
        private const int           MOUSE_BUTTON_WHEELDOWN             = 5;

        private int                 ret                                = 0;
        private readonly Settings   g_Settings;


        public ALSAERRCODE ErrorCode { get; set; }

        #region ALSAPINVOKE

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
        #endregion

        #region LIBXDOPINVOKE

        /*
             More information is no where because this library doesn't have a up to date online documentation.
             At least the ones that i've found were either for python wrappers or deprecated.
             Instead refer here: https://github.com/jordansissel/xdotool
         */

        /// <summary>
        /// Create a new xdo_t instance.
        /// </summary>
        /// <param name="display">Display the string display name, such as ":0". If null, uses the environment variable DISPLAY just like XOpenDisplay(NULL).</param>
        /// <returns>Pointer to a new xdo_t or NULL on failure</returns>
        [DllImport(LIBXDO_LIB_PATH)]
        static extern xdo_t xdo_new(string display);

        /// <summary>
        /// Get the currently-active window.
        /// Requires your window manager to support this. Uses _NET_ACTIVE_WINDOW from the EWMH spec.
        /// </summary>
        /// <param name="xdo">xdo_t instance</param>
        /// <param name="window_ret">window_ret	Pointer to Window where the active window is stored</param>
        /// <returns>0 on success.(obviously not zero on fail lel)</returns>
        [DllImport(LIBXDO_LIB_PATH)]
        static extern int xdo_get_active_window(xdo_t xdo, out Window window_ret);

        /// <summary>
        /// Send a keysequence to the specified window.
        /// This allows you to send keysequences by symbol name. Any combination of X11 KeySym names separated by '+' are valid. Single KeySym names are valid, too.
        /// Examples: "l" "semicolon" "alt+Return" "Alt_L+Tab"
        /// If you want to type a string, such as "Hello world." you want to instead use xdo_enter_text_window.
        /// </summary>
        /// <param name="xdo">xdo_t instance</param>
        /// <param name="window_ret">The window you want to send the keysequence to or CURRENTWINDOW(0)</param>
        /// <param name="keysequence">The string keysequence to send</param>
        /// <param name="delay">The delay between keystrokes in microseconds</param>
        /// <returns>0 on success.(obviously not zero on fail lel)</returns>
        [DllImport(LIBXDO_LIB_PATH)]
        static extern int xdo_enter_text_window(xdo_t xdo, Window window_ret, string keysequence, uint delay = 12000);

        /// <summary>
        /// Send a mouse press (aka mouse down) for a given button at the current mouse location.
        /// </summary>
        /// <param name="xdo">xdo_t instance</param>
        /// <param name="window_ret">The window you want to send the keysequence to or CURRENTWINDOW(0)</param>
        /// <param name="button"></param>
        /// <returns>0 on success.(obviously not zero on fail lel)</returns>
        [DllImport(LIBXDO_LIB_PATH)]
        static extern int xdo_mouse_down(xdo_t xdo, Window window_ret, int button);
        [DllImport(LIBXDO_LIB_PATH)]
        static extern int xdo_mouse_up(xdo_t xdo, Window window_ret, int button);

        /// <summary>
        /// Free and destroy an xdo_t instance.
        /// </summary>
        /// <param name="xdo">xdo_t instance</param>
        [DllImport(LIBXDO_LIB_PATH)]
        static extern void xdo_free(xdo_t xdo);
        #endregion

        public linuxFunctions(Settings set)
        {
            g_Settings = set;
        }

        public float GetVolumeLevel()
        {
            long level = 0;
            var ranges = getVolumeRange();
            ret = snd_mixer_selem_get_playback_volume(getMixer(), SND_MIXER_SCHN_FRONT_LEFT, ref level);
            if (ret != ALSA_SUCCESS)
            {
                ErrorCode = ALSAERRCODE.GET_PLAYBACK_VOLUME;
                GlobalHelper.log("snd_mixer_selem_get_playback_volume failed with errcode: " + ret);
                return INVALID_RANGE;
            }
            return level * PERCENT / ranges[RANGE_MAXIMUM];
        }

        public bool isMixerMuted()
        {
            var mixer = getMixer();
            var val = 0;
            var hasSwitch = Convert.ToBoolean(snd_mixer_selem_has_playback_switch(mixer));
            if (!hasSwitch)
            {
                GlobalHelper.log("snd_mixer_selem_has_playback_switch failed. No playback switch found.");
                return false;
            }
            ret = snd_mixer_selem_get_playback_switch(mixer, SND_MIXER_SCHN_FRONT_LEFT, ref val);
            if (ret != ALSA_SUCCESS)
            {
                ErrorCode = ALSAERRCODE.GET_PLAYBACK_SWITCH;
                GlobalHelper.log("snd_mixer_selem_get_playback_switch failed with errcode: " + ret);
                return false;
            }
            /* get_playback_switch returns the status of the toggle. If it retuns 1 means mixer is not muted.
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
            ret = snd_mixer_selem_set_playback_switch_all(getMixer(), MUTE_MIXER);
            if (ret != ALSA_SUCCESS)
            {
                ErrorCode = ALSAERRCODE.SET_PLAYBACK_SWITCH_ALL;
                GlobalHelper.log("snd_mixer_selem_set_playback_switch_all failed with errcode: " + ret);
            }
        }

        public void unmuteMixer()
        {
            if (!isMixerMuted()) return;
            ret = snd_mixer_selem_set_playback_switch_all(getMixer(), UNMUTE_MIXER);
            if (ret != ALSA_SUCCESS)
            {
                ErrorCode = ALSAERRCODE.SET_PLAYBACK_SWITCH_ALL;
                GlobalHelper.log("snd_mixer_selem_set_playback_switch_all failed with errcode: " + ret);
            }
        }

        public void VolumeDownBy(float value)
        {
            var ranges = getVolumeRange();
            var level = GetVolumeLevel();
            if (level <= RANGE_PERCENT_MIN) return;
            var valueToSet = ((long)(level - value) * ranges[RANGE_MAXIMUM] / PERCENT) + ROUND;
            ret = snd_mixer_selem_set_playback_volume_all(getMixer(), valueToSet);
            if (ret != ALSA_SUCCESS)
            {
                ErrorCode = ALSAERRCODE.SET_PLAYBACK_VOLUME_ALL;
                GlobalHelper.log("snd_mixer_selem_set_playback_volume_all failed with errcode: " + ret);
            }
        }

        public void VolumeUpBy(float value)
        {
            long valueToSet = 0;
            var ranges = getVolumeRange();
            var level = GetVolumeLevel();
            if (value >= RANGE_PERCENT_MAX)
                valueToSet = ranges[RANGE_MAXIMUM];
            else
                valueToSet = ((long)(level + value) * ranges[RANGE_MAXIMUM] / PERCENT) + ROUND;
            ret = snd_mixer_selem_set_playback_volume_all(getMixer(), valueToSet);
            if (ret != ALSA_SUCCESS)
            {
                ErrorCode = ALSAERRCODE.SET_PLAYBACK_VOLUME_ALL;
                GlobalHelper.log("snd_mixer_selem_set_playback_volume_all failed with errcode: " + ret);
            }
        }

        public long[] getVolumeRange()
        {
            long min = INVALID_RANGE;
            long max = INVALID_RANGE;
            var mixer = getMixer();
            ret = snd_mixer_selem_get_playback_volume_range(getMixer(), ref min, ref max);
            if (ret != ALSA_SUCCESS)
            {
                ErrorCode = ALSAERRCODE.GET_PLAYBACK_VOLUME_RANGE;
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
                ErrorCode = ALSAERRCODE.MIXER_OPEN;
                GlobalHelper.log("snd_mixer_open failed with errcode: " + ret);
                return IntPtr.Zero;
            }
            ret = snd_mixer_attach(mixerHandle, g_Settings.confQuery<string>(DEFAULT_SOUND_CARD_KEY));
            if (ret != ALSA_SUCCESS)
            {
                ErrorCode = ALSAERRCODE.MIXER_ATTACH;
                GlobalHelper.log("snd_mixer_attach failed with errcode: " + ret);
                return IntPtr.Zero;
            }
            ret = snd_mixer_selem_register(mixerHandle, IntPtr.Zero, IntPtr.Zero);
            if (ret != ALSA_SUCCESS)
            {
                ErrorCode = ALSAERRCODE.MIXER_SELEM_REGISTER;
                GlobalHelper.log("snd_mixer_selem_register failed with errcode: " + ret);
                return IntPtr.Zero;
            }
            ret = snd_mixer_load(mixerHandle);
            if (ret != ALSA_SUCCESS)
            {
                ErrorCode = ALSAERRCODE.MIXER_LOAD;
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
            {
                ErrorCode = ALSAERRCODE.FIND_SELEM;
                GlobalHelper.log("snd_mixer_find_selem failed with errcode: " + ret);
            }
            return IntPtr.Zero;
        }

        public Point getMousePosition() => Cursor.Position;

        public void setMousePosition(Point mousePoint) => Cursor.Position = mousePoint;

        public void sendKeyStroke(string key)
        {
            /*
                This function is great unless it sucks.
                Cuz of reasons that i'm not really sure yet, xdo_enter_text_window acts really weird on Turkish characters
                or in general, non ASCI characters. I assume the function reacts unicodes as unicoRNS since its performance gets better
                as the function delay increases. Will check on that but for now, libxdo is a lot more viable solution than coding an X11 shared lib
                by myself.
             */
            Window w_ret;
            xdo_t mXDO = xdo_new(":0"); // basically a NULL
            ret = xdo_get_active_window(mXDO, out w_ret);
            if (ret != LIBXDO_SUCCESS)
                GlobalHelper.log("xdo_get_active_window failed with errcode: " + ret);
            ret = xdo_enter_text_window(mXDO, w_ret, key);
            if (ret != LIBXDO_SUCCESS)
                GlobalHelper.log("xdo_enter_text_window failed with errcode: " + ret);
            xdo_free(mXDO);
        }

    }
}

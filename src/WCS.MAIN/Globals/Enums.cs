namespace WCS.MAIN.Globals
{
    public enum COMMANDS
    {
        SHUTDOWN = 0,
        VOLUP = 1,
        VOLDOWN = 2,
        MUTE = 3,
        UNMUTE = 4,
        MOUSE_MOV_LEFT_ACTIVE = 5,
        MOUSE_MOV_LEFT_DEACTIVE = 6,
        MOUSE_MOV_RIGHT_ACTIVE = 7,
        MOUSE_MOV_RIGHT_DEACTVE = 8,
        MOUSE_MOV_UP_ACTIVE = 9,
        MOUSE_MOV_UP_DEACTIVE = 10,
        MOUSE_MOV_DOWN_ACTIVE = 11,
        MOUSE_MOV_DOWN_DEACTIVE = 12,
        MOUSE_LCLICK = 13,
        MOUSE_RCLICK = 14
    }
    public enum OS
    {
        WINDOWS = 0,
        LINUX = 1,
        MACOSX = 2,
        CROSSPLATFORM = 3
    }
    /// <summary>
    /// Why this while alsa functions throw their own error codes ? 
    /// I've added these for the developmental reasons. They act like typedefs of the real error codes.
    /// </summary>
    public enum ALSAERRCODE
    {
        NONE,
        MIXER_OPEN,
        MIXER_ATTACH,
        MIXER_SELEM_REGISTER,
        MIXER_LOAD,
        FIND_SELEM,
        GET_PLAYBACK_VOLUME_RANGE,
        GET_PLAYBACK_VOLUME,
        GET_PLAYBACK_SWITCH,
        SET_PLAYBACK_SWITCH_ALL,
        SET_PLAYBACK_VOLUME_ALL,
        MIXER_CLOSE
    }
}

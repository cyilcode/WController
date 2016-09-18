namespace WCS.MAIN.Globals
{
    public enum COMMANDS
    {
        SHUTDOWN,
        VOLUP,
        VOLDOWN,
        MUTE,
        UNMUTE,
        MOUSE_MOV_LEFT_ACTIVE,
        MOUSE_MOV_LEFT_DEACTIVE,
        MOUSE_MOV_RIGHT_ACTIVE,
        MOUSE_MOV_RIGHT_DEACTVE,
        MOUSE_MOV_UP_ACTIVE,
        MOUSE_MOV_UP_DEACTIVE,
        MOUSE_MOV_DOWN_ACTIVE,
        MOUSE_MOV_DOWN_DEACTIVE,
        MOUSE_LCLICK_DOWN,
        MOUSE_LCLICK_UP,
        MOUSE_RCLICK_DOWN,      // TODO: These commands should belong to different enums. Will implement them on Input 3/4.
        MOUSE_RCLICK_UP,
        MWHEEL_DOWN,
        MWHEEL_UP,
        MWHEEL_RIGHT,
        MWHEEL_LEFT
    }

    public enum OS
    {
        WINDOWS,
        LINUX,
        MACOSX,
        CROSSPLATFORM
    }
}

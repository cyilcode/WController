﻿namespace WCS.MAIN.Globals
{
    public enum OS_COMMANDS
    {
        SHUTDOWN,
        SLEEP,
        LOGOUT,
        OS_COMMANDS_COUNT
    }
    
    public enum AUDIO_COMMANDS
    { 
        VOLUP = OS_COMMANDS.OS_COMMANDS_COUNT + 1,
        VOLDOWN,
        MUTE,
        UNMUTE,
        AUDIO_COMMANDS_COUNT
    }

    public enum INPUT_COMMANDS
    {
        MOUSE_MOV_LEFT_ACTIVE = AUDIO_COMMANDS.AUDIO_COMMANDS_COUNT + 1,
        MOUSE_MOV_LEFT_DEACTIVE,
        MOUSE_MOV_RIGHT_ACTIVE,
        MOUSE_MOV_RIGHT_DEACTVE,
        MOUSE_MOV_UP_ACTIVE,
        MOUSE_MOV_UP_DEACTIVE,
        MOUSE_MOV_DOWN_ACTIVE,
        MOUSE_MOV_DOWN_DEACTIVE,
        MOUSE_LCLICK_DOWN,
        MOUSE_LCLICK_UP,
        MOUSE_RCLICK_DOWN,
        MOUSE_RCLICK_UP,
        MWHEEL_DOWN,
        MWHEEL_UP,
        MWHEEL_RIGHT,
        MWHEEL_LEFT,
        KEYBOARD_KEY_PRESS_DOWN,
        KEYBOARD_KEY_PRESS_UP,
        INPUT_COMMANDS_COUNT
    }

    public enum PLUGINS
    {
        PLUGIN_TEST = INPUT_COMMANDS.INPUT_COMMANDS_COUNT + 1
    }

    public enum OS
    {
        WINDOWS,
        LINUX,
        MACOSX,
        CROSSPLATFORM
    }
}

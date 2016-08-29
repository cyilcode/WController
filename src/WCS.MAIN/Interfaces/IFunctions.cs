using System.Drawing;

namespace WCS.MAIN.Interfaces
{
    public interface IFunctions
    {
        /// <summary>
        /// Turns down the volume of the master mixer by the float amount.
        /// </summary>
        /// <param name="value">percentage</param>
        void VolumeDownBy(float value);
        /// <summary>
        /// Turns up the volume of the master mixer by the float amount.
        /// </summary>
        /// <param name="value">percentage</param>
        void VolumeUpBy(float value);
        /// <summary>
        /// Gets the current master mixer volume level.
        /// </summary>
        /// <returns>Current volume level</returns>
        float GetVolumeLevel();
        /// <summary>
        /// Gets the master mixer mute state.
        /// </summary>
        /// <returns>Mixer mute state</returns>
        bool isMixerMuted();
        /// <summary>
        /// Mutes the master mixer.
        /// </summary>
        void muteMixer();
        /// <summary>
        /// Unmutes the master mixer.
        /// </summary>
        void unmuteMixer();
        /// <summary>
        /// Returns X,Y coordinates of the mouse cursor.
        /// </summary>
        /// <returns>Point position</returns>
        Point getMousePosition();
        /// <summary>
        /// Sets the location of the mouse cursor and visually warps it.
        /// </summary>
        /// <param name="mousePoint">Mouse position to set</param>
        void setMousePosition(Point mousePoint);
    }
}

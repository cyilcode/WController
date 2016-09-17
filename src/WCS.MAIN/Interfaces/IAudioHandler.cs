namespace WCS.MAIN.Interfaces
{
    public interface IAudioHandler
    {
        /// <summary>
        /// Turns down the volume of the master mixer by the float amount.
        /// </summary>
        /// <param name="value">percentage</param>
        int VolumeDownBy(int value);
        /// <summary>
        /// Turns up the volume of the master mixer by the float amount.
        /// </summary>
        /// <param name="value">percentage</param>
        int VolumeUpBy(int value);
        /// <summary>
        /// Gets the current master mixer volume level.
        /// </summary>
        /// <returns>Current volume level</returns>
        int GetVolumeLevel();
        /// <summary>
        /// Gets the master mixer mute state.
        /// </summary>
        /// <returns>Mixer mute state</returns>
        int isMixerMuted();
        /// <summary>
        /// Mutes the master mixer.
        /// </summary>
        int muteMixer();
        /// <summary>
        /// Unmutes the master mixer.
        /// </summary>
        int unmuteMixer();
    }
}

using System.Drawing;
using WCS.MAIN.Globals;

namespace WCS.MAIN.Interfaces
{
    public interface IInputHandler
    {
        /// <summary>
        /// Returns X,Y coordinates of the mouse cursor.
        /// </summary>
        /// <returns>Point position</returns>
        Point getMousePosition();
        /// <summary>
        /// Sets the location of the mouse cursor and visually warps it.
        /// </summary>
        /// <param name="mousePoint">Mouse position to set</param>
        int setMousePosition(Point mousePoint);
        /// <summary>
        /// Sends the full key event(key press, down, up) to active window
        /// </summary>
        /// <param name="key">Key to send</param>
        int sendKeyStroke(string key);
        /// <summary>
        /// Executes a mouse event.
        /// </summary>
        /// <param name="evt">Command to execute</param>
        /// <returns>0 on success. 9998 on fail.</returns>
        int sendMouseEvent(INPUT_COMMANDS evt);
    }
}

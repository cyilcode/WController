using System.Drawing;

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
    }
}

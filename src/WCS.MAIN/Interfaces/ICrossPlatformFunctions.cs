using System.Drawing;

namespace WCS.MAIN.Interfaces
{
    public interface ICrossPlatformFunctions
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
        void setMousePosition(Point mousePoint);
    }
}

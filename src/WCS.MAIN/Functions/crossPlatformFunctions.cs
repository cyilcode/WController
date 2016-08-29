using System;
using System.Drawing;
using System.Windows.Forms;
using WCS.MAIN.Interfaces;

namespace Functions
{
    public class crossPlatformFunctions : ICrossPlatformFunctions
    {
        public Point getMousePosition() => Cursor.Position;

        public void setMousePosition(Point mousePoint) => Cursor.Position = mousePoint;
    }
}
using Functions;
using System.Drawing;
using System.Windows.Forms;
using WCS.MAIN.Globals;
using WCS.MAIN.Interfaces;
using Xunit;

namespace WCS.TEST
{
    public class crossplatformTests
    {
        private readonly ICrossPlatformFunctions crossFunctions;
        private const string Category = "Cross platform functions Tests";
        private const byte TEST_X = 200;
        private const byte TEST_Y = 200;
        public crossplatformTests()
        {
            crossFunctions = new crossPlatformFunctions();
        }
        /* Mouse cursor should not be moving while testing this function.
           It would be very hard to make it fail but just in care. */
        [CompatibleFact(OS.CROSSPLATFORM, true), Trait("Category", Category)]
        public void getMousePosition_returns_correct_position()
        {
            var cpfPosition = crossFunctions.getMousePosition();
            Assert.Equal(Cursor.Position, cpfPosition);
        }

        [CompatibleFact(OS.CROSSPLATFORM, true), Trait("Category", Category)]
        public void setMousePosition_sets_mouse_to_correct_position()
        {
            var newPoint = new Point(TEST_X, TEST_Y);
            crossFunctions.setMousePosition(newPoint);
            Assert.Equal(Cursor.Position, newPoint);
        }
    }
}

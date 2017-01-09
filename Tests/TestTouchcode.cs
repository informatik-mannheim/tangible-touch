using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication4.Geometry;
using WpfApplication4.Geometry.Elements;
using NUnit.Framework;

namespace WpfApplication4.Tests
{
    [TestFixture]
    class TestTouchcode
    {
        [Test]
        public void TestInvalidTouchcodeWithNoTouchpoints()
        {
            // Arrange
            var vectors = new Vector2d[] { };

            // SUT + Act
            var touchcode = Touchcode.From(vectors);
            var id = touchcode.Id;

            // Assert
            Assert.AreEqual(id, int.MinValue);
            Assert.IsInstanceOf<InvalidTouchcode>(touchcode);
        }

        [Test]
        public void TestInvalidTouchcodeWithOneTouchpoint()
        {
            // Arrange
            var vectors = new Vector2d[] { new Vector2d(1, 1) };

            // SUT + Act
            var touchcode = Touchcode.From(vectors);
            var id = touchcode.Id;

            // Assert
            Assert.AreEqual(id, int.MinValue);
            Assert.IsInstanceOf<InvalidTouchcode>(touchcode);
        }

        [Test]
        public void TestInvalidTouchcodeWithTwoTouchpoints()
        {
            // Arrange
            var vectors = new Vector2d[] { new Vector2d(1, 1), new Vector2d(4, 4) };

            // SUT + Act
            var touchcode = Touchcode.From(vectors);
            var id = touchcode.Id;

            // Assert
            Assert.AreEqual(id, int.MinValue);
            Assert.IsInstanceOf<InvalidTouchcode>(touchcode);
        }
    }
}

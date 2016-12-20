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

        [Test]
        public void TestZeroTouchcode()
        {
            // Arrange
            // Arrange
            // these are the outer box..
            var vectorA = new Vector2d(1, 1);
            var vectorB = new Vector2d(1, 4);
            var vectorC = new Vector2d(4, 1);

            var vectors = new Vector2d[] { vectorA, vectorB, vectorC };

            // SUT
            var touchcode = Touchcode.From(vectors);

            // Act
            var id = touchcode.Id;

            // Assert
            Assert.AreEqual(id, 0x0);
            Assert.IsTrue(touchcode.FixPoints.Contains(vectorA));
            Assert.IsTrue(touchcode.FixPoints.Contains(vectorB));
            Assert.IsTrue(touchcode.FixPoints.Contains(vectorC));
            Assert.IsNotInstanceOf<InvalidTouchcode>(touchcode);
        }

        [Test]
        public void TestComplicatedThing()
        {
            // Arrange
            // these are the outer box..
            var vectorA = new Vector2d(20, 50);
            var vectorB = new Vector2d(17, 53);
            var vectorC = new Vector2d(23, 53);

            // ...those are inside
            var vectorD = new Vector2d(20, 51);
            var vectorE = new Vector2d(20, 52);
            var vectorF = new Vector2d(22, 53);


            Vector2d[] vectors = new Vector2d[] { vectorA, vectorB, vectorC, vectorD, vectorE, vectorF };

            // Act
            var touchcode = Touchcode.From(vectors);

            // Assert
            Assert.IsTrue(touchcode.FixPoints.Contains(vectorA));
            Assert.IsTrue(touchcode.FixPoints.Contains(vectorB));
            Assert.IsTrue(touchcode.FixPoints.Contains(vectorC));
        }

        [Test]
        public void TestSomeId()
        {
            // Arrange
            // these are the outer box..
            var vectorA = new Vector2d(20, 50);
            var vectorB = new Vector2d(17, 53);
            var vectorC = new Vector2d(23, 53);

            // ...those are inside
            var vectorD = new Vector2d(20, 51); // 8
            var vectorE = new Vector2d(22, 52); // 12


            Vector2d[] vectors = new Vector2d[] { vectorA, vectorB, vectorC, vectorD, vectorE };

            // Act
            var touchcode = Touchcode.From(vectors);

            // Assert
            Assert.IsTrue(touchcode.FixPoints.Contains(vectorA));
            Assert.IsTrue(touchcode.FixPoints.Contains(vectorB));
            Assert.IsTrue(touchcode.FixPoints.Contains(vectorC));

            Assert.AreEqual(touchcode.Id, 0x100010000000);
        }
    }
}

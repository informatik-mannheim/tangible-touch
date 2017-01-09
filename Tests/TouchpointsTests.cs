using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication4.Geometry.Elements;
using NUnit.Framework;

namespace WpfApplication4.Tests
{
    [TestFixture]
    class TouchpointsTests
    {
        [Test]
        public void ShouldInitializeCorrectly()
        {
            // Arrange
            var vectors = new List<Vector2d>() { new Vector2d(1, 1), new Vector2d(1, 4), new Vector2d(4, 1)};

            // Act
            var touchpoints = new Touchpoints(vectors);

            // Assert
            Assert.IsTrue(touchpoints.AllTouchPoints.All(vec => vectors.Contains(vec)));
        }

        public void ShouldThrowOnInvalidTouchpoints()
        {
            // Arrange
            var vectors = new List<Vector2d>() { new Vector2d(1, 1), new Vector2d(1, 3), new Vector2d(7, 18)};

            // Act + Assert
            Assert.Throws<InvalidTouchPointsException>(() => new Touchpoints(vectors));
        }
        
        [Test]
        public void TestFindThreeFixpoints()
        {
            // Arrange
            // Arrange
            // these are the outer box..
            var vectorA = new Vector2d(1, 1);
            var vectorB = new Vector2d(1, 4);
            var vectorC = new Vector2d(4, 1);

            var vectors = new List<Vector2d>() { vectorA, vectorB, vectorC };

            // SUT
            var touchpoints = new Touchpoints(vectors);

            // Assert
            Assert.AreEqual(touchpoints.FixPoints.Count, 3);
            Assert.IsTrue(touchpoints.FixPoints.Contains(vectorA));
            Assert.IsTrue(touchpoints.FixPoints.Contains(vectorB));
            Assert.IsTrue(touchpoints.FixPoints.Contains(vectorC));
        }


        [Test]
        public void TestFindThreeFixpointsRotated()
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


            List<Vector2d> vectors = new List<Vector2d> { vectorB, vectorC, vectorD, vectorE, vectorF, vectorA };

            // Act
            var touchpoints = new Touchpoints(vectors);

            // Assert
            Assert.AreEqual(touchpoints.FixPoints.Count, 3);
            Assert.IsTrue(touchpoints.FixPoints.Contains(vectorA));
            Assert.IsTrue(touchpoints.FixPoints.Contains(vectorB));
            Assert.IsTrue(touchpoints.FixPoints.Contains(vectorC));
            Assert.IsFalse(touchpoints.FixPoints.Contains(vectorD));
            Assert.IsFalse(touchpoints.FixPoints.Contains(vectorE));
            Assert.IsFalse(touchpoints.FixPoints.Contains(vectorF));
        }

        [Test]
        public void TestFindFixpointsRotated180()
        {
            // Arrange
            // these are the outer box..
            var vectorA = new Vector2d(4, 4);
            var vectorB = new Vector2d(4, 1);
            var vectorC = new Vector2d(1, 4);

            // ...those are inside
            var vectorD = new Vector2d(2, 1);
            var vectorE = new Vector2d(3, 2);
            var vectorF = new Vector2d(3, 4);

            List<Vector2d> vectors = new List<Vector2d> { vectorA, vectorB, vectorC, vectorD, vectorE, vectorF };

            // Act
            var touchpoints = new Touchpoints(vectors);

            // Assert
            Assert.AreEqual(touchpoints.FixPoints.Count, 3);
            Assert.IsTrue(touchpoints.FixPoints.Contains(vectorA));
            Assert.IsTrue(touchpoints.FixPoints.Contains(vectorB));
            Assert.IsTrue(touchpoints.FixPoints.Contains(vectorC));
            Assert.IsFalse(touchpoints.FixPoints.Contains(vectorD));
            Assert.IsFalse(touchpoints.FixPoints.Contains(vectorE));
            Assert.IsFalse(touchpoints.FixPoints.Contains(vectorF));
        }

        [TestCase(20, 50, 17, 53, 23, 53)]
        [TestCase(1, 1, 1, 4, 4, 1)]    
        [TestCase(1, 4, 1, 1, 4, 4)]
        [TestCase(4, 4, 1, 4, 4, 1)]
        [TestCase(4, 1, 4, 4, 1, 1)]
        [TestCase(10, 10, 10, 40, 40, 10)]
        public void TestGetOrigin(int ax, int ay, int bx, int by, int cx, int cy)
        {
            // Arrange
            var vectorA = new Vector2d(ax, ay);
            var vectorB = new Vector2d(bx, by);
            var vectorC = new Vector2d(cx, cy);
            var vectors = new List<Vector2d>() {vectorA, vectorB, vectorC};

            // Act + Assert
            Assert.AreEqual(new Touchpoints(new List<Vector2d>() { vectorA, vectorB, vectorC }).Origin, vectorA);
            Assert.AreEqual(new Touchpoints(new List<Vector2d>() { vectorA, vectorC, vectorB }).Origin, vectorA);
            Assert.AreEqual(new Touchpoints(new List<Vector2d>() { vectorB, vectorA, vectorC }).Origin, vectorA);
            Assert.AreEqual(new Touchpoints(new List<Vector2d>() { vectorB, vectorC, vectorA }).Origin, vectorA);
            Assert.AreEqual(new Touchpoints(new List<Vector2d>() { vectorC, vectorB, vectorA }).Origin, vectorA);
            Assert.AreEqual(new Touchpoints(new List<Vector2d>() { vectorC, vectorA, vectorB }).Origin, vectorA);  
        }

        [Test]
        public void ShouldTranslateByOrigin()
        {
            // Arrange
            var vectorA = new Vector2d(3, 3);
            var vectorB = new Vector2d(6, 3);
            var vectorC = new Vector2d(3, 6);
            var vectorD = new Vector2d(5, 6);
            var vectorE = new Vector2d(4, 4);
            var vectorF = new Vector2d(4, 6);

            var vectors = new List<Vector2d>() { vectorA, vectorB, vectorC, vectorD, vectorE, vectorF };

            // Act
            var touchpoints = new Touchpoints(vectors);

            touchpoints.MoveToOrigin();

            
            Assert.AreEqual(touchpoints.AllTouchPoints.Count, 6);
            Assert.AreEqual(touchpoints.FixPoints.Count, 3);
            Assert.IsTrue(touchpoints.Origin.Equals(new Vector2d(0, 0)));
            Assert.IsTrue(touchpoints.AllTouchPoints.Any(p => p.Equals(new Vector2d(0, 0))));
            Assert.IsTrue(touchpoints.AllTouchPoints.Any(p => p.Equals(new Vector2d(3, 0))));
            Assert.IsTrue(touchpoints.AllTouchPoints.Any(p => p.Equals(new Vector2d(0, 3))));
            Assert.IsTrue(touchpoints.AllTouchPoints.Any(p => p.Equals(new Vector2d(2, 3))));
            Assert.IsTrue(touchpoints.AllTouchPoints.Any(p => p.Equals(new Vector2d(1, 1))));
            Assert.IsTrue(touchpoints.AllTouchPoints.Any(p => p.Equals(new Vector2d(1, 3))));
        }

        [Test]
        public void ShouldTranslateByOriginComplex()
        {
            // Arrange
            var vectorA = new Vector2d(20, 50);
            var vectorB = new Vector2d(17, 53);
            var vectorC = new Vector2d(23, 53);
            var vectorD = new Vector2d(20, 52);
            var vectors = new List<Vector2d>() { vectorA, vectorB, vectorC, vectorD, };

            // Act
            var touchpoints = new Touchpoints(vectors);

            touchpoints.MoveToOrigin();
            
            Assert.AreEqual(touchpoints.AllTouchPoints.Count, 4);
            Assert.AreEqual(touchpoints.FixPoints.Count, 3);
            Assert.IsTrue(touchpoints.Origin.Equals(new Vector2d(0, 0)));
            Assert.IsTrue(touchpoints.AllTouchPoints.Any(p => p.Equals(new Vector2d(0, 0))));
            Assert.IsTrue(touchpoints.AllTouchPoints.Any(p => p.Equals(new Vector2d(-3, 3))));
            Assert.IsTrue(touchpoints.AllTouchPoints.Any(p => p.Equals(new Vector2d(3, 3))));
            Assert.IsTrue(touchpoints.AllTouchPoints.Any(p => p.Equals(new Vector2d(0, 2))));
        }
    }
}

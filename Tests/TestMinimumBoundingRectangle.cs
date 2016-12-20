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
    class TestMinimumBoundingRectangle
    {
        [Test]
        public void NaiveTest()
        {
            // Arrange
            // these are the outer box..
            var vectorA = new Vector2d(1, 1);
            var vectorB = new Vector2d(1, 4);
            var vectorC = new Vector2d(4, 1);

            // ...those are inside
            var vectorD = new Vector2d(1, 2);
            var vectorE = new Vector2d(1, 3);
            var vectorF = new Vector2d(2, 1);
            var vectorG = new Vector2d(2, 2);
            var vectorH = new Vector2d(2, 3);
            var vectorI = new Vector2d(2, 4);
            var vectorJ = new Vector2d(2, 4);
            var vectorK = new Vector2d(3, 1);
            var vectorL = new Vector2d(3, 2);
            var vectorM = new Vector2d(3, 3);
            var vectorN = new Vector2d(3, 4);
            var vectorO = new Vector2d(4, 2);
            var vectorP = new Vector2d(4, 3);

            Vector2d[] vectors = new Vector2d[] { vectorA, vectorB, vectorC, vectorD, vectorE, vectorF, vectorG, vectorH, vectorI, vectorJ, vectorK, vectorL, vectorM, vectorN, vectorO, vectorP };
            
            // Act
            var box = MinimalBoundingBox.Calculate(vectors);

            // Assert
            Assert.AreEqual(box.Points.Count, 4);
            
            Assert.Contains(vectorA, box.Points);
            Assert.Contains(vectorB, box.Points);
            Assert.Contains(vectorC, box.Points);

            // we expect this to contain a fourth point as the 4th corner of the box
            Assert.Contains(new Vector2d(4, 4), box.Points);
            
            // those are inside the box and should not be part of its border
            Assert.IsFalse(box.Points.Contains(vectorD));
            Assert.IsFalse(box.Points.Contains(vectorE));
            Assert.IsFalse(box.Points.Contains(vectorF));
            Assert.IsFalse(box.Points.Contains(vectorG));
            Assert.IsFalse(box.Points.Contains(vectorH));
            Assert.IsFalse(box.Points.Contains(vectorI));
            Assert.IsFalse(box.Points.Contains(vectorJ));
            Assert.IsFalse(box.Points.Contains(vectorK));
            Assert.IsFalse(box.Points.Contains(vectorL));
            Assert.IsFalse(box.Points.Contains(vectorM));
            Assert.IsFalse(box.Points.Contains(vectorN));
            Assert.IsFalse(box.Points.Contains(vectorO));
            Assert.IsFalse(box.Points.Contains(vectorP));
        }

        [Test]
        public void RotatedAndTranslated()
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
            var box = MinimalBoundingBox.Calculate(vectors);

            // Assert
            Assert.AreEqual(box.Points.Count, 4);

            Assert.Contains(vectorB, box.Points);
            Assert.Contains(vectorC, box.Points);
        }                      
    }
}

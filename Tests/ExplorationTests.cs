using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MathNet.Spatial;
using MathNet.Spatial.Euclidean;
using MathNet.Numerics;


namespace WpfApplication4.Tests
{
    [TestFixture]
    class ExplorationTests
    {
        [Test]
        public void CheckPointVectorConversion()
        {
            var point = new Point2D(100, 200);
            var vector = point.ToVector2D();

            Assert.AreEqual(vector.X, point.X);
            Assert.AreEqual(vector.Y, point.Y);
        }

        [Test]
        public void CheckPerpendicularity()
        {
            var vectorA = new Vector2D(100, 1);
            var vectorB = new Vector2D(0, 100);

            Console.Write(vectorA.AngleTo(vectorB).Degrees);
            Assert.IsTrue(vectorA.IsPerpendicularTo(vectorB, Trig.DegreeToRadian(1)));
        }

        [Test]
        public void CheckAlmostEqual()
        {
            double a = 145;
            double b = 140;
            double c = 150;

            Assert.IsTrue(Precision.AlmostEqual(a, b, 5.0001));
            Assert.IsTrue(Precision.AlmostEqual(a, c, 5.0001));
        }

        [Test]
        public void CheckRotation()
        {
            var angle = new MathNet.Spatial.Units.Angle(90, MathNet.Spatial.Units.AngleUnit.Degrees);

            Console.Write(new Vector2D(0, 1).Rotate(angle));

        }
    }
}

using MathNet.Spatial.Euclidean;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;

namespace TangibleTouch.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    class TouchcodeNativeAPITests
    {
        private List<List<Point2D>> _samples0x10 = new List<List<Point2D>> {
                new List<Point2D> {
                    new Point2D(552, 647),
                    new Point2D(363, 572),
                    new Point2D(467, 578),  
                    new Point2D(423, 707)
                },
                new List<Point2D> {
                    new Point2D(382, 429),
                    new Point2D(464, 244),
                    new Point2D(452, 345),  
                    new Point2D(325, 294)
                },
                new List<Point2D> {
                    new Point2D(1533, 244),
                    new Point2D(1540, 447),
                    new Point2D(1500, 346),  
                    new Point2D(1641, 334)
                },
                new List<Point2D> {
                    new Point2D(199, 589),
                    new Point2D(405, 620),
                    new Point2D(302, 637),  
                    new Point2D(320, 505)
                }
            };

        private List<List<Point2D>> _samples0x80 = new List<List<Point2D>> {
                new List<Point2D> {
                    new Point2D(1643, 614),
                    new Point2D(1450, 649),
                    new Point2D(1555, 663),  
                    new Point2D(1568, 731)
                },
                new List<Point2D> {
                    new Point2D(577, 629),
                    new Point2D(379, 577),
                    new Point2D(471, 631),  
                    new Point2D(453, 701)
                },
                new List<Point2D> {
                    new Point2D(1486, 68),
                    new Point2D(1638, 213),
                    new Point2D(1581, 119),  
                    new Point2D(1628, 69)
                },
                new List<Point2D> {
                    new Point2D(1676, 651),
                    new Point2D(1530, 799),
                    new Point2D(1619, 750),  
                    new Point2D(1675, 791)
                },
                new List<Point2D> {
                    new Point2D(176, 469),
                    new Point2D(320, 324),
                    new Point2D(225, 373),  
                    new Point2D(175, 324)
                },
                new List<Point2D> {
                    new Point2D(725, 544),
                    new Point2D(860, 697),
                    new Point2D(819, 599),  
                    new Point2D(869, 557)
                },
                new List<Point2D> {
                    new Point2D(346, 509),
                    new Point2D(494, 368),
                    new Point2D(399, 415),  
                    new Point2D(346, 367)
                },
                new List<Point2D> {
                    new Point2D(769, 593),
                    new Point2D(750, 792),
                    new Point2D(793, 695),  
                    new Point2D(865, 701)
                },
                new List<Point2D> {
                    new Point2D(269, 202),
                    new Point2D(477, 205),
                    new Point2D(375, 164),  
                    new Point2D(382, 93)
                }
            };

        [Test]
        public void ShouldReturnDefaultTouchcodeForNull()
        {
            // Arrange
            List<Point2D> touchpoints = null;

            // System Under Test
            var api = new TouchcodeAPI();

            // Act
            var touchcode = api.Check(touchpoints);

            // Assert
            Assert.AreEqual(touchcode, Touchcode.None);
        }

        [Test]
        public void ShouldReturnDefaultTouchcodeForLessThanThreeTouchpoints()
        {
            // Arrange
            List<Point2D> touchpoints = new List<Point2D>();

            // System Under Test
            var api = new TouchcodeAPI();

            // Act
            var touchcode = api.Check(touchpoints);

            // Assert
            Assert.AreEqual(touchcode, Touchcode.None);

            touchpoints.Add(new Point2D(0, 0));
            touchcode = api.Check(touchpoints);
            Assert.AreEqual(touchcode, Touchcode.None);

            touchpoints.Add(new Point2D(0, 0));
            touchcode = api.Check(touchpoints);
            Assert.AreEqual(touchcode, Touchcode.None);
        }

        [Test]
        public void ShouldRecognizeTouchcode0x80()
        {
            // System Under Test
            var api = new TouchcodeAPI();

            // Act + Assert
            foreach (var sample in _samples0x80)
            {
                Assert.AreEqual(0x80, api.Check(sample).Value);
            }
        }

        [Test]
        public void ShouldRecognizeTouchcode0x10()
        {
            // System Under Test
            var api = new TouchcodeAPI();

            // Act + Assert
            foreach (var sample in _samples0x10)
            {
                Assert.AreEqual(0x10, api.Check(sample).Value);
            }
        }

        [Test]
        public void ShouldNotRecognizeInvalidTouchpoints()
        {
            // Arrange
            var samplesInvalid = new List<List<Point2D>> {
                new List<Point2D> {
                    new Point2D(303, 152),
                    new Point2D(379, 577),
                    new Point2D(368, 171),  
                    new Point2D(368, 285)
                },
                new List<Point2D> {
                    new Point2D(1473, 235),
                    new Point2D(1417, 328),
                    new Point2D(1563, 340),  
                    new Point2D(1624, 263)
                }
            };

            // System Under Test
            var api = new TouchcodeAPI();

            // Act + Assert
            foreach (var sample in samplesInvalid)
            {
                Assert.AreEqual(Touchcode.None, api.Check(sample));
            }
        }

        [Test]
        public void ShouldRecongizeEmptyTouchcode()
        {
            // Arrange
            var touchpoints = new List<Point2D>();
			var referenceSystem = new ReferenceSystem(new Point2D(0, 0), new Vector2D(3, 0), new Vector2D(0, 3));

            // System Under Test
            var api = new TouchcodeAPI();

            // Act
			var touchcode = referenceSystem.MapPointsToTouchcode(touchpoints).Value;

            // Assert
            Assert.AreEqual(touchcode, 0);
        }

        [Test]
        public void ShouldRecongizeChristmasTree()
        {
            // Arrange
            var touchpoints = new List<Point2D> {
                    new Point2D(1, 0),
                    new Point2D(2, 0),
                    new Point2D(0, 1),  
                    new Point2D(1, 1),
                    new Point2D(2, 1),
                    new Point2D(3, 1),
                    new Point2D(0, 2),
                    new Point2D(1, 2),
                    new Point2D(2, 2),
                    new Point2D(3, 2),
                    new Point2D(1, 3),
                    new Point2D(2, 3)
            };

            // System Under Test
			var referenceSystem = new ReferenceSystem(new Point2D(0, 0), new Vector2D(3, 0), new Vector2D(0, 3));

            // Act
			var touchcode = referenceSystem.MapPointsToTouchcode(touchpoints).Value;

            // Assert
            Assert.AreEqual(0xFFF, touchcode);
        }

        [Test]
        public void ShouldRecongizeTouchcode0x18()
        {
            // Arrange
            var touchpoints = new List<Point2D> {
                    new Point2D(1.1, 2.0),
                    new Point2D(1.8, 2.2)
            };

            // System Under Test
			var referenceSystem = new ReferenceSystem(new Point2D(0, 0), new Vector2D(3, 0), new Vector2D(0, 3));

            // Act
			var touchcode = referenceSystem.MapPointsToTouchcode(touchpoints).Value;

            // Assert
            Assert.AreEqual(touchcode, 0x18);
        }

        [Test]
        public void ShouldRecongizeTouchcode0x888()
        {
            // Arrange
            var touchpoints = new List<Point2D> {
                    new Point2D(1.1, 2.0),
                    new Point2D(1.1, 1.2),
                    new Point2D(2.0, 0.0)
            };

			// System Under Test
			var referenceSystem = new ReferenceSystem(new Point2D(0, 0), new Vector2D(3, 0), new Vector2D(0, 3));

			// Act
			var touchcode = referenceSystem.MapPointsToTouchcode(touchpoints).Value;

            // Assert
            Assert.AreEqual(touchcode, 0x888);
        }

        [Test]
        public void ShouldRecongizeTouchcode0x444()
        {
            // Arrange
            var touchpoints = new List<Point2D> {
                    new Point2D(0, 2),
                    new Point2D(0.1, 1.2),
                    new Point2D(1.0, 0.1)
            };

			// System Under Test
			var referenceSystem = new ReferenceSystem(new Point2D(0, 0), new Vector2D(3, 0), new Vector2D(0, 3));

			// Act
			var touchcode = referenceSystem.MapPointsToTouchcode(touchpoints).Value;

            // Assert
            Assert.AreEqual(touchcode, 0x444);
        }

        [TestCase(1, 1, 0x80)]
        [TestCase(0.9, 1.1, 0x80)]
        [TestCase(1.2, 0.8, 0x80)]
        [TestCase(1.3, 0.7, 0)]
        [TestCase(70, 100, 0)]
        public void ShouldRecongizeTouchcode0x80(double x, double y, int expectedTouchcode)
        {
            // Arrange
            var touchpoints = new List<Point2D> {
                new Point2D(x, y)
            };

			// System Under Test
			var referenceSystem = new ReferenceSystem(new Point2D(0, 0), new Vector2D(3, 0), new Vector2D(0, 3));

			// Act
			var touchcode = referenceSystem.MapPointsToTouchcode(touchpoints).Value;

            // Assert
            Assert.AreEqual(touchcode, expectedTouchcode);
        }

        [TestCase(1, 0, 1, 1080)]
        [TestCase(1, 2, 1, 1078)]
        [TestCase(100, 3, 100, 1077)]
        [TestCase(200, 1080, 200, 0)]
        [TestCase(300, 900, 300, 180)]
        public void ShouldMirrorCoordinatesAlongXAxis(double xBefore, double yBefore, double xAfter, double yAfter)
        {
            // Arrange
            int maxY = 1080;

            var touchpoints = new List<Point2D> {
                new Point2D(xBefore, yBefore)
            };

            // System Under Test
            var api = new TouchcodeAPI();

            // Act
            var mirroredPoints = api.MirrorX(touchpoints, maxY);

            // Assert
            Assert.AreEqual(mirroredPoints[0].X, xAfter);
            Assert.AreEqual(mirroredPoints[0].Y, yAfter);
        }

        [Test]
        public void ShouldGetReferenceSystemSimple()
        {
            // Arrange
            var touchpoints = new List<Point2D> {
                    new Point2D(0, 0),
                    new Point2D(3, 0),
                    new Point2D(0, 3),
                    new Point2D(1, 1),
                    new Point2D(2, 2),

            };

            // System Under Test
            var api = new TouchcodeAPI();

            // Act
            var referenceSystem = api.ExtractReferenceSystemFrom(touchpoints);

            // Assert
            Assert.AreEqual(touchpoints[0], referenceSystem.Origin);
            Assert.AreEqual(touchpoints[1].ToVector2D(), referenceSystem.Vx);
            Assert.AreEqual(touchpoints[2].ToVector2D(), referenceSystem.Vy);
        }

        [Test]
        public void ShouldGetReferenceSystemDifferentOrder()
        {
            // Arrange
            var touchpoints = new List<Point2D> {
                new Point2D(1, 1),    
                new Point2D(0, 3),
                new Point2D(3, 0),
                new Point2D(0, 0)
            };

            var origin = touchpoints[3];
            var expectedVx = touchpoints[2] - origin;
            var expectedVy = touchpoints[1] - origin;

            // System Under Test
            var api = new TouchcodeAPI();

            // Act
            var referenceSystem = api.ExtractReferenceSystemFrom(touchpoints);

            // Assert
            Assert.AreEqual(touchpoints[3], referenceSystem.Origin);
			Assert.AreEqual(expectedVx, referenceSystem.Vx);
            Assert.AreEqual(expectedVy, referenceSystem.Vy);
        }

        [Test]
        public void ShouldGetReferenceSystemRotated45Degrees()
        {
            // Arrange
            var touchpoints = new List<Point2D> {
                new Point2D(0, 0),    
                new Point2D(3, 0),
                new Point2D(2, 1),
                new Point2D(3, 3)
            };

            // Arrange
            var origin = touchpoints[1];
            var expectedVx = touchpoints[3] - origin;
            var expectedVy = touchpoints[0] - origin;

            // System Under Test
            var api = new TouchcodeAPI();

            // Act
            var referenceSystem = api.ExtractReferenceSystemFrom(touchpoints);

            // Assert
            Assert.AreEqual(origin, referenceSystem.Origin);
            Assert.AreEqual(expectedVx, referenceSystem.Vx);
            Assert.AreEqual(expectedVy, referenceSystem.Vy);
        }

        [Test]
        public void ShouldGetReferenceSystemRotated90Degrees()
        {
            // Arrange
            var touchpoints = new List<Point2D> {
                new Point2D(0, 3),    
                new Point2D(3, 0),
                new Point2D(3, 3),
                new Point2D(2, 2),
                new Point2D(1, 1),
                new Point2D(0, 1)
            };

            // Arrange
            var origin = touchpoints[2];
            var expectedVx = touchpoints[0] - origin;
            var expectedVy = touchpoints[1] - origin;

            // System Under Test
            var api = new TouchcodeAPI();

            // Act
            var referenceSystem = api.ExtractReferenceSystemFrom(touchpoints);

            // Assert
            Assert.AreEqual(origin, referenceSystem.Origin);
            Assert.AreEqual(expectedVx, referenceSystem.Vx);
            Assert.AreEqual(expectedVy, referenceSystem.Vy);
        }

        [Test]
        public void ShouldGetReferenceSystemRotated135Degrees()
        {
            // Arrange
            var touchpoints = new List<Point2D> {
                new Point2D(0, 0),    
                new Point2D(0, 3),
                new Point2D(3, 3),
                new Point2D(1, 2),
                new Point2D(1, 1)
            };

            // Arrange
            var origin = touchpoints[1];
            var expectedVx = touchpoints[0] - origin;
            var expectedVy = touchpoints[2] - origin;

            // System Under Test
            var api = new TouchcodeAPI();

            // Act
            var referenceSystem = api.ExtractReferenceSystemFrom(touchpoints);

            // Assert
            Assert.AreEqual(origin, referenceSystem.Origin);
            Assert.AreEqual(expectedVx, referenceSystem.Vx);
            Assert.AreEqual(expectedVy, referenceSystem.Vy);
        }

        [Test]
        public void ShouldGetReferenceSystemForSamples0x80()
        {
            // System Under Test
            var api = new TouchcodeAPI();

            foreach(var sample in _samples0x80)
            {
                // Arrange
                var origin = sample[3];
                var expectedVx = sample[1] - origin;
                var expectedVy = sample[0] - origin;
                
                // Act
                var referenceSystem = api.ExtractReferenceSystemFrom(sample);

                // Assert
                Assert.AreEqual(origin, referenceSystem.Origin);
                Assert.AreEqual(expectedVx, referenceSystem.Vx);
                Assert.AreEqual(expectedVy, referenceSystem.Vy);
            }
        }

        [Test]
        public void ShouldGetReferenceSystemForSamples0x10()
        {
            // System Under Test
            var api = new TouchcodeAPI();

            foreach (var sample in _samples0x10)
            {
                // Arrange
                var origin = sample[3];
                var expectedVx = sample[1] - origin;
                var expectedVy = sample[0] - origin;

                // Act
                var referenceSystem = api.ExtractReferenceSystemFrom(sample);

                // Assert
                Assert.AreEqual(origin, referenceSystem.Origin);
                Assert.AreEqual(expectedVx, referenceSystem.Vx);
                Assert.AreEqual(expectedVy, referenceSystem.Vy);
            }
        }

        [TestCase(1, 0, 0, 1, false)]
        [TestCase(1, 1, 1, 1, true)]
        [TestCase(1, 1, 1, 0, true)]
        [TestCase(1, 1, 1, -0.9, true)]
        [TestCase(1, 1, -1, -1, false)]
        [TestCase(-1, -1, -1, 0.9, true)]
        public void ShouldCheckIfTwoVectorsHaveTheSameOrientation(double x1, double y1, double x2, double y2, bool sameOrientation)
        {
            // Arrange
            Vector2D a = new Vector2D(x1, y1);
            Vector2D b = new Vector2D(x2, y2);

            Assert.AreEqual(sameOrientation, a.HasSameOrientationAs(b));
        }

        [Test]
        public void ShouldRecognizeTouchcode0x001()
        {
            // Arrange
            List<Point2D> points = new List<Point2D> {
                new Point2D(306, 501),
                new Point2D(300, 647),
                new Point2D(445, 660),
                new Point2D(362, 507)
            };

            // System Under Test
            var api = new TouchcodeAPI();

            // Act
            var touchcode = api.Check(points);

            // Assert
            Assert.AreEqual(0x001, touchcode.Value);
        }
    }
}

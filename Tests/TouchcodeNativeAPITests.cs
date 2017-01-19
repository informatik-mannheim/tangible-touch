using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApplication4.Touchcode;

namespace WpfApplication4.Tests
{
    [TestFixture, RequiresSTA]
    class TouchcodeNativeAPITests
    {
        [Test]
        public void ShouldReturnDefaultTouchcodeForLessThanThreeTouchpoints()
        {
            // Arrange
            List<TouchPoint> touchpoints = new List<TouchPoint>();

            // System Under Test
            var api = new TouchcodeNativeAPI();

            // Act
            var touchcode = api.Check(touchpoints);

            // Assert
            Assert.AreEqual(touchcode, -1);

            touchpoints.Add(new FakeTouchPoint(0, 0));
            touchcode = api.Check(touchpoints);
            Assert.AreEqual(touchcode, -1);

            touchpoints.Add(new FakeTouchPoint(0, 0));
            touchcode = api.Check(touchpoints);
            Assert.AreEqual(touchcode, -1);
        }
    }
}

using System.Windows;
using System.Windows.Input;

namespace WpfApplication4.Touchcode
{
    class FakeTouchPoint : TouchPoint
    {
        public FakeTouchPoint(double x, double y)
            : base(new IHaveNoTouchDevice(666), new System.Windows.Point(x, y), new Rect(), new TouchAction())
        {

        }
    }
}

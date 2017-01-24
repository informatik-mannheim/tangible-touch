using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication4
{
    public class Touchcode
    {
        public static Touchcode None = new Touchcode(int.MinValue, 0, new Point2D(), new Point2D(), new Point2D());

        public Touchcode(int value, double angle, Point2D o, Point2D x, Point2D y)
        {
            Value = value;
            Angle = angle;
            X = x;
            Y = y;
            O = o;
        }

        public int Value { get; private set; }
        public double Angle { get; private set; }
        public Point2D X { get; private set; }
        public Point2D Y { get; private set; }
        public Point2D O { get; private set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

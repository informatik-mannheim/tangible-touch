using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication4.Geometry;
using WpfApplication4.Geometry.Elements;

namespace WpfApplication4.Geometry
{
    public class Touchcode
    {
        public int Id { get; protected set; }

        public List<Vector2d> FixPoints { get; private set; }

        public static Touchcode From(Vector2d[] touchPoints)
        {
            if (touchPoints.Count() < 3)
            {
                return new InvalidTouchcode();
            }

            
            var box = MinimalBoundingBox.Calculate(touchPoints);
            
            var known = new List<Vector2d>();
            var unknown = new List<Vector2d>();

            foreach (var point in box.Points)
            {
                if (!touchPoints.Contains(point))
                {
                    unknown.Add(point);
                }
                else
                {
                    known.Add(point);
                }
            }

            if (known.Count() == 3)
            {
                return new Touchcode(known);
            }
            else
            {
                var a = unknown[0];
                var b = unknown[1];
                var c = new Vector2d((a.X + b.X) / 2, (a.Y + b.Y) / 2);
                if (touchPoints.Contains(c))
                {
                    return new Touchcode(new List<Vector2d>() { known[0], known[1], c });
                }
                else
                {
                    return new InvalidTouchcode();
                }
            }
        }

        protected Touchcode(List<Vector2d> fixPoints, List<Vector2d> touchPoints)
        {
            FixPoints = fixPoints;

            var id = 1000000;
            Id = 0;
        }
    }

    public class InvalidTouchcode : Touchcode
    {
        public InvalidTouchcode()
            : base(null)
        {
            Id = int.MinValue;
        }
    }
}

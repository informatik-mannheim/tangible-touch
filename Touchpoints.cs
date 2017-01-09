using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication4.Geometry.Elements;
using WpfApplication4.Geometry;

namespace WpfApplication4
{
    public class Touchpoints
    {
        public IList<Vector2d> AllTouchPoints {get; private set;}
        public List<Vector2d> FixPoints { get; private set; }
        public Vector2d Origin { get; private set; }

        public Touchpoints(IList<Vector2d> touchPoints)
        {
            AllTouchPoints = touchPoints;
            FixPoints = FindFixpoints();
            Origin = CalculateOrigin(FixPoints[0], FixPoints[1], FixPoints[2]);
        }

        private Vector2d CalculateOrigin(Vector2d a, Vector2d b, Vector2d c)
        {
            if (a.Distance(b) == a.Distance(c))
            {
                return a;
            }
            else if (a.Distance(b) > a.Distance(c))
            {
                return c;
            }

            return b;
        }

        public void MoveToOrigin()
        {
            List<Vector2d> translatedVectors = new List<Vector2d>();

            foreach (var p in AllTouchPoints)
            {
                translatedVectors.Add(p.Subtract(Origin));
            }

            AllTouchPoints = translatedVectors;
            FixPoints = FindFixpoints();
            Origin = CalculateOrigin(FixPoints[0], FixPoints[1], FixPoints[2]);
        }

        private List<Vector2d> FindFixpoints()
        {
            if (AllTouchPoints.Count() < 3)
            {
                throw new InvalidTouchPointsException();
            }

            var box = MinimalBoundingBox.Calculate(AllTouchPoints.ToArray<Vector2d>());

            var known = new List<Vector2d>();
            var unknown = new List<Vector2d>();

            // the minimal bounding box produces results like 0.999999965
            foreach(var p in box.Points) 
            {
                p.X = Math.Round(p.X, 3);
                p.Y = Math.Round(p.Y, 3);
            }

            
            foreach (var point in box.Points)
            {
                if (!AllTouchPoints.Any(p => p.Equals(point)))
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
                return known;
            }
            else
            {
                var a = unknown[0];
                var b = unknown[1];
                var c = new Vector2d((a.X + b.X) / 2, (a.Y + b.Y) / 2);

                if (AllTouchPoints.Contains(c))
                {
                    return new List<Vector2d> {known[0], known[1], c };
                }
                else
                {
                    throw new InvalidTouchPointsException();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Input;
using MathNet.Numerics;
using MathNet.Spatial;
using MathNet.Spatial.Euclidean;
using System.Linq;
using System.Text;

namespace WpfApplication4.Touchcode
{
    public class TouchcodeAPI
    {
        private int NO_TOUCHCODE = -1;

        private Dictionary<Point2D, int> _touchpointMap = new Dictionary<Point2D, int> {
            { new Point2D(1, 3), 0x001 },
            { new Point2D(2, 3), 0x002 },
            { new Point2D(0, 2), 0x004 },
            { new Point2D(1, 2), 0x008 },
            { new Point2D(2, 2), 0x010 },
            { new Point2D(3, 2), 0x020 },
            { new Point2D(0, 1), 0x040 },
            { new Point2D(1, 1), 0x080 },
            { new Point2D(2, 1), 0x100 },
            { new Point2D(3, 1), 0x200 },
            { new Point2D(1, 0), 0x400 },
            { new Point2D(2, 0), 0x800 },
        };

        public int Check(IList<TouchPoint> touchPoints, bool xMirror = true, int maxY = 1080)
        {
            return Check(touchPoints.Select(point => new Point2D(point.Position.X, point.Position.Y)).ToList(), xMirror, maxY);
        }

        public int Check(IList<Point2D> touchpoints, bool xMirror = true, int maxY = 1080)
        {
            if (touchpoints == null || touchpoints.Count < 3)
            {
                return NO_TOUCHCODE;
            }

            if (xMirror)
            {
                touchpoints = MirrorX(touchpoints);
            }

            var referenceSystem = GetReferenceSystem(touchpoints);

            if(referenceSystem == null)
            {
                return NO_TOUCHCODE;
            }

            return MapPointsToTouchcode(touchpoints.Select(point => Normalize(referenceSystem, point)));
        }

        private Point2D Normalize(Tuple<Point2D, Point2D, Point2D> referenceSystem, Point2D point)
        {
            var vx = referenceSystem.Item2 - referenceSystem.Item1;
            var vy = referenceSystem.Item3 - referenceSystem.Item1;
            var so = point - referenceSystem.Item1;

            vx = (vx / vx.Length) / vx.Length * 3;
            vy = (vy / vy.Length) / vy.Length * 3;

            var xcor = vx.DotProduct(so);
            var ycor = vy.DotProduct(so);

            return new Point2D(Math.Round(xcor, 1), Math.Round(ycor, 1));
        }

        public Tuple<Point2D, Point2D, Point2D> GetReferenceSystem(IList<Point2D> touchPoints)
        {
            double threshold = 0.10;

            var vectors = touchPoints.Combinations(2);

            var distances = vectors
                .Select(points => new Tuple<IList<Point2D>, double>(points.ToList(), points.ToList()[0].DistanceTo(points.ToList()[1])));

            var longestDistance = distances.OrderByDescending(t => t.Item2).FirstOrDefault();
            var v1 = longestDistance.Item1[0];
            var v2 = longestDistance.Item1[1];

            IList<Point2D> candidates = new List<Point2D>();

            foreach (var distance in distances)
            {
                if (Precision.AlmostEqual(longestDistance.Item2 / Constants.Sqrt2, distance.Item2, (longestDistance.Item2 / Constants.Sqrt2) * threshold))
                {
                    if (distance.Item1[0] == v1 || distance.Item1[0] == v2)
                    {
                        candidates.Add(distance.Item1[1]);
                    }
                    else if (distance.Item1[1] == v1 || distance.Item1[1] == v2)
                    {
                        candidates.Add(distance.Item1[0]);
                    }
                }
            }

            Point2D origin;

            try
            {
                origin = candidates.GroupBy(c => c.GetHashCode()).Where(g => g.Count() == 2).Select(g => g).FirstOrDefault().FirstOrDefault();
                return FindVxVyIn(new Tuple<Point2D, Point2D, Point2D>(origin, v1, v2));
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Tuple<Point2D, Point2D, Point2D> FindVxVyIn(Tuple<Point2D, Point2D, Point2D> referenceSystem)
        {
            var positiveXAxis = new Vector2D(1, 0);
            var positiveYAxis = new Vector2D(0, 1);
            var realOrigin = new Point2D(0, 0);

            var origin = referenceSystem.Item1;
            
            var translationVector = realOrigin - origin;
            
            var v1 = (referenceSystem.Item2 + translationVector).ToVector2D();
            var v2 = (referenceSystem.Item3 + translationVector).ToVector2D();

            var angle = v1.SignedAngleTo(positiveYAxis, false, false);

            v1 = v1.Rotate(angle);
            v2 = v2.Rotate(angle);

            if(v2.HasSameOrientationAs(positiveXAxis))
            {
                return new Tuple<Point2D, Point2D, Point2D>(origin, referenceSystem.Item3, referenceSystem.Item2);
            }
            else
            {
                return new Tuple<Point2D, Point2D, Point2D>(origin, referenceSystem.Item2, referenceSystem.Item3);
            }
        }

        public int MapPointsToTouchcode(IEnumerable<Point2D> touchPoints)
        {
            var threshold = 0.2001;
            var touchcode = 0;
            
            _touchpointMap.ToList().ForEach(map => touchcode |= touchPoints.Any(tp => tp.AlmostEqual(map.Key, threshold)) ? map.Value : 0);

            return touchcode;
        }

        public IList<Point2D> MirrorX(IEnumerable<Point2D> touchPoints, int maxY = 1080)
        {
            return touchPoints.Select(point => new Point2D(point.X, maxY - point.Y)).ToList();
        }

        public string Serialize(List<TouchPoint> touchpoints)
        {
            StringBuilder builder = new StringBuilder("[");

            for (int i = 0; i < touchpoints.Count; i++)
            {
                var tp = touchpoints[i];
                builder.AppendFormat("({0},{1}){2}", tp.Position.X, tp.Position.Y, i == touchpoints.Count - 1 ? "" : ",");
            }

            return builder.Append("]").ToString();
        }
    }

    internal static class ExtensionMethods
    {
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] { new T[0] } :
              elements.SelectMany((e, i) =>
                elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
        }

        public static bool AlmostEqual(this Point2D thisPoint, Point2D thatPoint, double threshold)
        {
            return Precision.AlmostEqual(thisPoint.X, thatPoint.X, threshold) && Precision.AlmostEqual(thisPoint.Y, thatPoint.Y, threshold);
        }

        public static bool HasSameOrientationAs(this Vector2D thisVector, Vector2D otherVector)
        {
            return thisVector.DotProduct(otherVector) > 0;
        }
    }
}

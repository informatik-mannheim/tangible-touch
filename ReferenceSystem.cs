using MathNet.Spatial.Euclidean;
using System.Collections.Generic;
using System.Linq;

namespace TangibleTouch
{
	public class ReferenceSystem
	{
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


		/// <summary>
		/// Gets the origin of the reference system in screen coordinates.
		/// </summary>
		public Point2D Origin { get; protected set; }

		/// <summary>
		/// Gets the x-axis of the reference system
		/// </summary>
		public Vector2D Vx { get; protected set; }

		/// <summary>
		/// Gets the y-axis of the reference system
		/// </summary>
		public Vector2D Vy { get; protected set; }

		/// <summary>
		/// Angle between the reference system and the positive y-axis of the screen.
		/// </summary>
		public double Angle { get; protected set; }

		/// <summary>
		/// Holds information about the reference coordinates (origin, vx, vy) and maps arbitrary screen coordinates to touchcodes.
		/// </summary>
		/// <param name="origin">The origin of the reference system in screen coordinates</param>
		/// <param name="vx">The x-axis of the reference system</param>
		/// <param name="vy">The y-axis of the reference system</param>
		public ReferenceSystem(Point2D origin, Vector2D vx, Vector2D vy)
		{
			Origin = origin;
			Vx = vx;
			Vy = vy;
			Angle = Vy.SignedAngleTo(new Vector2D(0, 1), false, false).Degrees;
		}

		/// <summary>
		/// Maps a list of TouchPoints in screen coordinates to the reference system coordinates and extracts a Touchcode.
		/// </summary>
		/// <param name="touchPoints">TouchPoints in screen coordinates</param>
		/// <returns>A <see cref="Touchcode"/>instance</returns>
		public virtual Touchcode MapPointsToTouchcode(IEnumerable<Point2D> touchPoints)
		{
			var threshold = 0.2001;
			var touchcode = 0;

			_touchpointMap
				.ToList()
				.ForEach(map => touchcode |= touchPoints.Any(tp => Normalize(tp).AlmostEqual(map.Key, threshold)) ? map.Value : 0);

			return new Touchcode(touchcode, this.Angle, this.Origin);
		}

		private Point2D Normalize(Point2D point)
		{
			var oPoint = point - Origin;

			var xcor = Vx.Normalize().DotProduct(oPoint / Vx.Length) * 3;
			var ycor = Vy.Normalize().DotProduct(oPoint / Vy.Length) * 3;

			return new Point2D(xcor, ycor);
		}
	}
}

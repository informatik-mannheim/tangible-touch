using MathNet.Spatial.Euclidean;
using System.Collections.Generic;

namespace TangibleTouch
{
	public class InvalidReferenceSystem : ReferenceSystem
	{
		public InvalidReferenceSystem()
			: base(new Point2D(-1, -1), new Vector2D(-1, -1), new Vector2D(-1, -1))
		{

		}

		public override Touchcode MapPointsToTouchcode(IEnumerable<Point2D> touchPoints)
		{
			return Touchcode.None;
		}
	}
}

using MathNet.Spatial.Euclidean;
using System;

namespace TangibleTouch
{
	/// <summary>
	/// Holds information about a recognized Touchcode.
	/// </summary>
	public class Touchcode
	{
		/// <summary>
		/// 
		/// </summary>
		public static Touchcode None = new Touchcode(int.MinValue, 0, new Point2D());

		/// <summary>
		/// The touchcode value as a 12-bit number, ranging between 0 (no bit set) to 4095 (all 12 bits set). 
		/// </summary>
		public int Value { get; private set; }

		/// <summary>
		/// The angle between the recognized touchcode and the Y-Axis of the screen, growing clockwise.
		/// </summary>
		public double Angle { get; private set; }

		/// <summary>
		/// The origin of the Touchcode in absolute screen coordinates.
		/// </summary>
		public Point2D Origin { get; private set; }

		/// <summary>
		/// Creates a Touchcode instance holding the Touchcode value and 
		/// spatial information related to the screen it was recognized on.
		/// </summary>
		public Touchcode(int value, double angle, Point2D origin)
		{
			Value = value;
			Angle = angle;
			Origin = origin;
		}

		/// <summary>
		/// Creates a formatted string with the Touchcode value in hex and the angle of the Touchcode 
		/// or '[None]' if the Touchcode is Touchcode.None.
		/// </summary>
		public override string ToString()
		{
			return this == None ? "[None]" : String.Format("0x{0:X} ({1:0.00}°)", Value, Angle);
		}
	}
}

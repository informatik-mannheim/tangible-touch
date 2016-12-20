using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace WpfApplication4.Geometry.Elements
{
	public class Polygon2d
	{
		public List<Vector2d> Points { get; set; } 

		public Polygon2d ()
		{
            Points = new List<Vector2d>();
		}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in Points)
            {
                sb.Append(item.ToString());
            }

            return sb.ToString();
        }
	}
}
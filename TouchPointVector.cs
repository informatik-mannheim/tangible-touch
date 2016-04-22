using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApplication4
{
    public class TouchPointVector : Vector
    {
        private int _id;
        private Vector _vector;

        public TouchPointVector(int id, double x, double y)
        {
            _vector = new Vector(x, y);
            Id = id;
        }

        public int Id { get; private set;}
    }
}

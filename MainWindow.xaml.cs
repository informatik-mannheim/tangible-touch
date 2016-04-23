using System;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using WpfApplication4;


namespace WpfTouchFrameSample
{
    public partial class MainWindow : Window
    {
        private int countTouches = 0;
        private int countDistances = 0;
        private double[,] position = new double[8, 2];

        // private ArrayList vectorList = new ArrayList();
        // map with all point objects
        private Dictionary<int, Point> pointMap = new Dictionary<int, Point>();
        // map with all distances between the points
        private Dictionary<int, double> distanceMap = new Dictionary<int, double>();


        public MainWindow()
        {
            InitializeComponent();
        }

        public delegate void UpdateTextCallback(string text);

        TouchPoint _touchPoint;

        void OnTouchDown(object sender, TouchEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            element.CaptureTouch(e.TouchDevice);
            _touchPoint = e.TouchDevice.GetTouchPoint(this.canvas1);

            // retrieve coordination of single point
            Point point = new Point(_touchPoint.Position.X, _touchPoint.Position.Y);

            // add point to map
            pointMap.Add(countTouches, point);

            // only calculate distance, if there is more than 1 touch point
            if (countTouches > 0)
            {
                // get one touch point
                // then calculate distance with the other touch points
                for (int i = 0; i < countTouches; i++)
                {
                    for (int j = i + 1; j < countTouches; j++)
                    {
                        // add distance to map
                        distanceMap.Add(countDistances, calcDistance(pointMap[i], pointMap[j]));

                        // count touch point
                        countTouches++;
                    }
                }
            }

            Nummer.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), countTouches.ToString());

            xy.AppendText(countTouches.ToString() + " X " + _touchPoint.Position.X.ToString() + " Y " + _touchPoint.Position.Y.ToString() + "\n");

            e.Handled = true;
        }

        void OnTouchUp(object sender, TouchEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;

            if (el == null) return;

            TouchPoint tp = e.GetTouchPoint(el);
            Rect bounds = new Rect(new Point(0, 0), el.RenderSize);


            foreach (var key in pointMap)
            {
                // key.Value is the actual value
                // here Point object
                if (key.Value.Equals(tp))
                {
                    // key.Key is the actual key
                    // remove touch point
                    distanceMap.Remove(key.Key);
                }
            }

            if (bounds.Contains(tp.Position))
            {
                countTouches--;
                Nummer.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), countTouches.ToString());
            }

            el.ReleaseTouchCapture(e.TouchDevice);

            e.Handled = true;
        }

        private void UpdateText(string message)
        {
            // hier evtl. den calculator aufrufen
            Nummer.Document.Blocks.Clear();
            Nummer.AppendText(message);
            // xy.Document.Blocks.Clear();
        }

        // Function calculates distance between two touch points
        // returns {double} distance
        private double calcDistance(Point p1, Point p2)
        {
            countDistances++;
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}
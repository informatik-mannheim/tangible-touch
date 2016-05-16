using System;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using WpfApplication4;
using System.Windows.Forms;
using System.Drawing;

namespace WpfTouchFrameSample
{
    public partial class MainWindow : Window
    {
        private int countTouches = 0;
        private int countDistances = 0;
        private int threshold = 5;
        // private double[,] position = new double[8, 2];
        // private ArrayList vectorList = new ArrayList();

        // map with all point objects
        private Dictionary<int, TouchPoint> touchPointMap = new Dictionary<int, TouchPoint>();
        private List<TouchPoint> touchPointList = new List<TouchPoint>();

        // map with all distances between the points
        private Dictionary<int, double> distanceMap = new Dictionary<int, double>();
        private List<Double> distanceList = new List<Double>();

        public delegate void UpdateTextCallback(string text);
        TouchPoint _touchPoint;

        public MainWindow()
        {
            InitializeComponent();
        }

        void OnTouchDown(object sender, TouchEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            // return if no touch element
            if (element == null) return;

            element.CaptureTouch(e.TouchDevice);
            _touchPoint = e.TouchDevice.GetTouchPoint(this.grid1);

            // update on touch down
            updateOnTouchDown(_touchPoint);

            e.Handled = true;
        }

        void OnTouchUp(object sender, TouchEventArgs e)
        {
            xaml_number_of_touchpoints.Document.Blocks.Clear();
            xaml_xy_coordinates.Document.Blocks.Clear();
            xaml_distances.Document.Blocks.Clear();

            FrameworkElement element = sender as FrameworkElement;
            // return if there is no element
            if (element == null) return;

            // get touch point
            TouchPoint touchPoint = e.GetTouchPoint(element);

            Rect bounds = new Rect(new System.Windows.Point(0, 0), element.RenderSize);

            updateOnTouchUp(touchPoint);

            element.ReleaseTouchCapture(e.TouchDevice);
            e.Handled = true;
        }

        void OnTouchMove(object sender, TouchEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            element.CaptureTouch(e.TouchDevice);
            _touchPoint = e.TouchDevice.GetTouchPoint(this.grid1);
            Console.WriteLine("test");

        }
        private void updateText()
        {
            xaml_xy_coordinates.Document.Blocks.Clear();
            

            int cTouchPoints = 0;

            foreach (TouchPoint element in touchPointList)
            {
                cTouchPoints++;
                xaml_number_of_touchpoints.Document.Blocks.Clear();

                xaml_number_of_touchpoints.AppendText(touchPointList.Count.ToString());
                xaml_xy_coordinates.AppendText("\n" + "Point: " + cTouchPoints + "\n" + " X: " + element.Position.X.ToString() + "\n" + " Y: " + element.Position.Y.ToString());

                xaml_distances.Document.Blocks.Clear();
                foreach (Double distance in distanceList)
                {
                    xaml_distances.AppendText("\n" + "Distance between the Points " + distance);
                }
                Console.WriteLine(distanceList.Count);
            }
        }

        // Function calculates distance between two touch points
        // returns {double} distance
        private void calcDistance()
        {
            // calculating the distances new with the touch points which left...
            distanceList = new List<Double>();

            if (countTouches >= 2)
            {
                for (int i = 0; i < touchPointList.Count; i++)
                {
                    // get first touchpoint
                    TouchPoint p1 = touchPointList[i];
                    for (int j = i + 1; j < touchPointList.Count; j++)
                    {
                        // get second touchpoint
                        TouchPoint p2 = touchPointList[j];
                        // calulate distance
                        Double distance = Math.Sqrt(Math.Pow(p2.Position.X - p1.Position.X, 2) + Math.Pow(p2.Position.Y - p1.Position.Y, 2));
                        if (!(distanceList.Contains(distance)))
                        {
                            // add distance value to map
                            distanceList.Add(distance);
                            // increase number of distances
                            countDistances++;
                            xaml_distances.AppendText("\n" + "Distance between the Points " + distance);
                        }
                    }
                }
            }

        }

        private void updateOnTouchDown(TouchPoint touchPoint)
        {
            // increase number of touch points
            countTouches++;
            Console.WriteLine("OnTouchDown..." + countTouches);

            // add new touchpoint to map
            touchPointList.Add(touchPoint);

            // calculate distance with minimum 2 touch points
            calcDistance();

            updateText();
        }

        private void updateOnTouchUp(TouchPoint touchPoint)
        {
            // find touch point and remove it
            foreach (TouchPoint element in touchPointList)
            {
                if (element.Position.X - touchPoint.Position.X <= threshold ||
                    element.Position.Y - touchPoint.Position.Y <= threshold ||
                    touchPoint.Position.X - element.Position.X <= threshold ||
                    touchPoint.Position.Y - element.Position.Y <= threshold)
                {
                    touchPointList.Remove(element);
                    // decrease number of touch points
                    countTouches--;
                    Console.WriteLine("OnTouchUp..." + countTouches);


                    // calculate distance with minimum 2 touch points
                    calcDistance();

                    updateText();

                    // return because the foreach won't recognize the updated number of elements
                    return;
                }
            }
        }
    }
}
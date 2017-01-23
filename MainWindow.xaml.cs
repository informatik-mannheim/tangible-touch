using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using WpfApplication4;
using WpfApplication4.Touchcode;

namespace WpfTouchFrameSample
{
    public partial class MainWindow : Window
    {
        private Object thisLock = new Object();

        private int _countTouches = 0;
        private int _countDistances = 0;
        private int _threshold = 15;
        private int _currentTouchcode = -1;

        private TouchcodeAPI _touchcodeAPI;

        private Dictionary<int, TouchPoint> touchPointMap = new Dictionary<int, TouchPoint>();
        private List<TouchPoint> touchPointList = new List<TouchPoint>();

        private Dictionary<int, double> distanceMap = new Dictionary<int, double>();
        private List<Double> distanceList = new List<Double>();

        public delegate void UpdateTextCallback(string text);

        public MainWindow()
        {
            InitializeComponent();

            var window = Window.GetWindow(this);
            window.KeyDown += OnKeyDown;

            _touchcodeAPI = new TouchcodeAPI();
        }


        void OnTouchDown(object sender, TouchEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            if (element == null) { return; }

            element.CaptureTouch(e.TouchDevice);
            var touchPoint = new MyTouchpoint(e.TouchDevice.GetTouchPoint(this.grid1));

            updateOnTouchDown(touchPoint);
            e.Handled = true;
        }

        void OnTouchUp(object sender, TouchEventArgs e)
        {
            xaml_number_of_touchpoints.Document.Blocks.Clear();
            xaml_xy_coordinates.Document.Blocks.Clear();
            xaml_distances.Document.Blocks.Clear();

            FrameworkElement element = sender as FrameworkElement;

            if (element == null) { return; }

            var touchPoint = new MyTouchpoint(e.GetTouchPoint(element));

            Rect bounds = new Rect(new System.Windows.Point(0, 0), element.RenderSize);

            updateOnTouchUp(touchPoint);

            element.ReleaseTouchCapture(e.TouchDevice);
            e.Handled = true;
        }

        private void updateOnTouchDown(MyTouchpoint touchPoint)
        {
            lock (thisLock)
            {
                Console.WriteLine("OnTouchDown [" + touchPoint.ID.ToString() + "]");

                // add new touchpoint to map
                touchPointList.Add(touchPoint);

                // check touchcode

                _currentTouchcode = _touchcodeAPI.Check(touchPointList);

                Console.WriteLine("OnTouchDown [" + touchPointList.Count + "]");

                // calculate distance with minimum 2 touch points
                calcDistance();

                updateText();
            }
        }

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key.ToString().Equals("S"))
            {
                Console.WriteLine(_touchcodeAPI.Serialize(touchPointList));
            }
            else if (e.Key.ToString().Equals("C"))
            {
                touchPointList.Clear();
                updateText();
                xaml_number_of_touchpoints.Document.Blocks.Clear();
            }
        }

        private void updateOnTouchUp(MyTouchpoint touchPointToRemove)
        {
            lock (thisLock)
            {
                bool found = false;
                // find touch point and remove it
                foreach (MyTouchpoint touchPoint in touchPointList)
                {
                    var deviationX = Math.Abs(touchPoint.Position.X - touchPointToRemove.Position.X);
                    var deviationY = Math.Abs(touchPoint.Position.Y - touchPointToRemove.Position.Y);

                    if (Math.Floor(((deviationX + deviationY) / 2)) <= _threshold)
                    {
                        Console.WriteLine("OnTouchUp [" + touchPoint.ID.ToString() + "]");

                        // decrease number of touch points
                        touchPointList.Remove(touchPoint);

                        _currentTouchcode = _touchcodeAPI.Check(touchPointList);

                        Console.WriteLine("OnTouchUp [" + touchPointList.Count + "]");

                        // calculate distance with minimum 2 touch points
                        calcDistance();

                        updateText();

                        // return because the foreach won't recognize the updated number of elements
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Console.WriteLine(String.Format("Couldn't find touchpoint x: {0} y: {1}", touchPointToRemove.Position.X, touchPointToRemove.Position.Y));

                }
            }
        }

        private void updateText()
        {
            xaml_xy_coordinates.Document.Blocks.Clear();

            xaml_xy_coordinates.AppendText(string.Format("Current Touchcode is: {0}\n\n", _currentTouchcode));

            foreach (MyTouchpoint touchPoint in touchPointList)
            {
                xaml_number_of_touchpoints.Document.Blocks.Clear();

                xaml_number_of_touchpoints.AppendText(touchPointList.Count.ToString());
                xaml_xy_coordinates.AppendText(touchPoint.ToString());
                

                xaml_distances.Document.Blocks.Clear();

                foreach (Double distance in distanceList)
                {
                    xaml_distances.AppendText("\n" + "Distance between the Points " + distance);
                }
            }
        }

        private void calcDistance()
        {
            // calculating the distances new with the touch points which left...
            distanceList = new List<Double>();

            if (_countTouches >= 2)
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
                            _countDistances++;
                            xaml_distances.AppendText("\n" + "Distance between the Points " + distance);
                        }
                    }
                }
            }

        }

        public class MyTouchpoint : TouchPoint
        {
            public DateTime Timestamp { get; private set; }

            public MyTouchpoint(TouchPoint touchPoint)
                : base(touchPoint.TouchDevice, touchPoint.Position, touchPoint.Bounds, touchPoint.Action)
            {
                Timestamp = DateTime.Now;
                ID = Guid.NewGuid();
            }

            public override string ToString()
            {
                return "\n" + "Point: " + ID + "\n(" + Timestamp.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + ")\n" + " X: " + Position.X.ToString() + "\n" + " Y: " + Position.Y.ToString();
            }

            public Guid ID
            {
                get;
                private set;
            }
        }
    }
}
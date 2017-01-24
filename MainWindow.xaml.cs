using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using WpfApplication4;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using MathNet.Spatial.Euclidean;
using System.Windows.Media.Animation;

namespace WpfTouchFrameSample
{
    public partial class MainWindow : Window
    {
        private Object thisLock = new Object();

        private int _threshold = 15;

        private List<TouchPoint> touchPointList = new List<TouchPoint>();

        private Rectangle _currentRectangle = new Rectangle();
        private Touchcode _currentTouchcode = Touchcode.None;
        private TouchcodeAPI _touchcodeAPI;
        
        public delegate void UpdateTextCallback(string text);

        public MainWindow()
        {
            InitializeComponent();

            var window = Window.GetWindow(this);
            window.KeyDown += OnKeyDown;

            _touchcodeAPI = new TouchcodeAPI();

            updateText();
        }

        private void Flash(int milliseconds)
        {
            var animation = new DoubleAnimation
            {
                AutoReverse = true,
                From = 1,
                To = 0,
                Duration = new TimeSpan(0, 0, 0, 0, milliseconds)
            };

            Storyboard.SetTargetName(animation, grid.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Shape.OpacityProperty));
            Storyboard flashStoryboard = new Storyboard();
            flashStoryboard.Children.Add(animation);
            flashStoryboard.Begin(grid);
        }

        private Rectangle DrawRect(double width, double height, Point2D origin, double rotationAngle)
        {
            var rect = new Rectangle() {
                Width = width,
                Height = height,
                Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                RenderTransform = new RotateTransform(rotationAngle, Width / 2, Height / 2),
                
            };
           
            Canvas.SetLeft(rect, origin.X);
            Canvas.SetTop(rect, origin.Y);

            canvas.Children.Add(rect);

            return rect;
        }

        void OnTouchDown(object sender, TouchEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            if (element == null) { return; }

            element.CaptureTouch(e.TouchDevice);
            var touchPoint = new MyTouchpoint(e.TouchDevice.GetTouchPoint(this.grid));

            updateOnTouchDown(touchPoint);
            e.Handled = true;
        }

        void OnTouchUp(object sender, TouchEventArgs e)
        {
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

                updateText();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Equals("S"))
            {
                Flash(150);
                Console.WriteLine(_touchcodeAPI.Serialize(touchPointList));
            }
            else if (e.Key.ToString().Equals("C"))
            {
                touchPointList.Clear();
                updateText();
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
            xaml_touchpoints.Clear();
            xaml_touchcode_value.Clear();

            xaml_touchcode_value.AppendText(_currentTouchcode.ToString());
            xaml_touchpoints.AppendText(String.Format("{0} TouchPoints @ {1}", touchPointList.Count, _touchcodeAPI.Serialize(touchPointList)));
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
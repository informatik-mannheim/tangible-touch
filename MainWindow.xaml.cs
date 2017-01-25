using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TangibleTouch;
using System.Linq;
using Path = System.IO.Path;

namespace WpfTouchFrameSample
{
    public partial class MainWindow : Window
    {
        private List<TouchPoint> _touchPointList = new List<TouchPoint>();

        private Touchcode _currentTouchcode;
        private TouchcodeAPI _touchcodeAPI;

        public MainWindow()
        {
            InitializeComponent();

            _touchcodeAPI = new TouchcodeAPI();
            _currentTouchcode = Touchcode.None;

            updateText();
        }

        private void updateText()
        {
            xaml_touchpoints.Text = String.Format("{0} TouchPoints @ {1}", _touchPointList.Count, _touchcodeAPI.Serialize(_touchPointList));
            xaml_touchcode_value.Text = _currentTouchcode.ToString();
        }

        void OnTouchDown(object sender, TouchEventArgs e)
        {
            grid.CaptureTouch(e.TouchDevice);

            _touchPointList.Add(e.TouchDevice.GetTouchPoint(grid));

            _currentTouchcode = _touchcodeAPI.Check(_touchPointList);

            updateText();
        }

        void OnTouchUp(object sender, TouchEventArgs e)
        {
            var touchpoint = e.GetTouchPoint(grid);

            _touchPointList.RemoveAll(p => p.TouchDevice == touchpoint.TouchDevice);
            _currentTouchcode = _touchcodeAPI.Check(_touchPointList);

            updateText();

            grid.ReleaseTouchCapture(e.TouchDevice);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Equals("S"))
            {
                Flash(150);
                WriteSampleToTempLogFile();
            }
        }

        private void WriteSampleToTempLogFile()
        {
            using (StreamWriter file = new StreamWriter(String.Format(@"{0}/touchcode_log.txt", Path.GetTempPath()), true))
            {
                file.WriteLine(_touchcodeAPI.Serialize(_touchPointList));
            }
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
    }
}
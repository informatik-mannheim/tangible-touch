using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using WpfApplication4;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Linq;
using MathNet.Spatial.Euclidean;
using System.Windows.Media.Animation;

namespace WpfTouchFrameSample
{
    public partial class MainWindow : Window
    {
        private List<TouchPoint> _touchPointList = new List<TouchPoint>();

        private Touchcode _currentTouchcode = Touchcode.None;
        private TouchcodeAPI _touchcodeAPI;
        
        public MainWindow()
        {
            InitializeComponent();

            Window.GetWindow(this).KeyDown += OnKeyDown;

            _touchcodeAPI = new TouchcodeAPI();

            updateText();
        }


        void OnTouchDown(object sender, TouchEventArgs e)
        {
            grid.CaptureTouch(e.TouchDevice);

            _touchPointList.Add(e.TouchDevice.GetTouchPoint(grid));
            
            _currentTouchcode = _touchcodeAPI.Check(_touchPointList);
            
            updateText();

            e.Handled = true;
        }

        void OnTouchUp(object sender, TouchEventArgs e)
        {
            var touchpoint = e.GetTouchPoint(grid);
            _touchPointList.RemoveAll(p => p.TouchDevice == touchpoint.TouchDevice);

            _currentTouchcode = _touchcodeAPI.Check(_touchPointList);

            updateText();
            
            grid.ReleaseTouchCapture(e.TouchDevice);
            e.Handled = true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Equals("S"))
            {
                Flash(150);
                Console.WriteLine(_touchcodeAPI.Serialize(_touchPointList));
            }

            e.Handled = true;
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

        private void updateText()
        {
            xaml_touchpoints.Clear();
            xaml_touchcode_value.Clear();

            xaml_touchpoints.AppendText(String.Format("{0} TouchPoints @ {1}", _touchPointList.Count, _touchcodeAPI.Serialize(_touchPointList)));
            xaml_touchcode_value.AppendText(_currentTouchcode.ToString());
        }
    }
}
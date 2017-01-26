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
using System.Windows.Media;
using System.Windows.Controls;

namespace WpfTouchFrameSample
{
	public partial class MainWindow : Window
	{
		private List<TouchPoint> _touchPointList = new List<TouchPoint>();

		private Touchcode _currentTouchcode;
		private TouchcodeAPI _touchcodeAPI;

		private Polygon _polygon;
		private Canvas _canvas;

		public MainWindow()
		{
			InitializeComponent();

			_touchcodeAPI = new TouchcodeAPI();
			_currentTouchcode = Touchcode.None;

			Redraw();

			_canvas = CreateTouchcodeVisualization();
			canvas.Children.Add(_canvas);
		}

		private void RenderTouchcodeVisualization()
		{
			if (_currentTouchcode != Touchcode.None)
			{
				for (int i = 0; i < 12; i++)
				{
					_canvas.Children[i].Visibility = (_currentTouchcode.Value & 1 << i) == 1 << i ? Visibility.Visible : Visibility.Hidden;
				}

				_canvas.RenderTransform = new RotateTransform(_currentTouchcode.Angle, 1800, 150);
			}
		}

		private void Redraw()
		{
			RenderTouchcodeVisualization();
			xaml_touchpoints.Text = String.Format("{0} TouchPoints @ {1}", _touchPointList.Count, _touchcodeAPI.Serialize(_touchPointList));
			xaml_touchcode_value.Text = _currentTouchcode.ToString();
		}

		void OnTouchDown(object sender, TouchEventArgs e)
		{
			grid.CaptureTouch(e.TouchDevice);

			_touchPointList.Add(e.TouchDevice.GetTouchPoint(grid));
			_touchPointList.OrderBy(t => t.TouchDevice);

			_currentTouchcode = _touchcodeAPI.Check(_touchPointList);
			
			Redraw();
		}

		void OnTouchMove(object sender, TouchEventArgs e)
		{
			var p = _touchPointList.Where(tp => tp.TouchDevice.Equals(e.TouchDevice)).First();
			
			_touchPointList.Remove(p);

			_touchPointList.Add(e.GetTouchPoint(grid));
			_touchPointList.OrderBy(t => t.TouchDevice);

			_currentTouchcode = _touchcodeAPI.Check(_touchPointList);

			Redraw();
		}

		void OnTouchUp(object sender, TouchEventArgs e)
		{
			var touchpoint = e.GetTouchPoint(grid);

			_touchPointList.RemoveAll(p => p.TouchDevice == touchpoint.TouchDevice);
			_touchPointList.OrderBy(t => t.TouchDevice);
			_currentTouchcode = _touchcodeAPI.Check(_touchPointList);

			grid.ReleaseTouchCapture(e.TouchDevice);

			Redraw();
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

		private Canvas CreateTouchcodeVisualization()
		{
			Canvas canvas = new Canvas();
			
			canvas.Children.Add(GetPointAt(new Point(1750 + 33.3, 100)));
			canvas.Children.Add(GetPointAt(new Point(1750 + 66.6, 100)));
			canvas.Children.Add(GetPointAt(new Point(1750, 100 + 33.3)));
			canvas.Children.Add(GetPointAt(new Point(1750 + 33.3, 100 + 33.3)));
			canvas.Children.Add(GetPointAt(new Point(1750 + 66.6, 100 + 33.3)));
			canvas.Children.Add(GetPointAt(new Point(1750 + 100, 100 + 33.3)));
			canvas.Children.Add(GetPointAt(new Point(1750, 100 + 66.6)));
			canvas.Children.Add(GetPointAt(new Point(1750 + 33.3, 100 + 66.6)));
			canvas.Children.Add(GetPointAt(new Point(1750 + 66.6, 100 + 66.6)));
			canvas.Children.Add(GetPointAt(new Point(1750 + 100, 100 + 66.6)));
			canvas.Children.Add(GetPointAt(new Point(1750 + 33.3, 100 + 100)));
			canvas.Children.Add(GetPointAt(new Point(1750 + 66.6, 100 + 100)));
			canvas.Children.Add(DrawLine(new Point(1750, 100), new Point(1750, 200)));
			canvas.Children.Add(DrawLine(new Point(1750, 200), new Point(1850, 200)));

			return canvas;
		}

		private Polygon DrawLine(Point from, Point to)
		{
			var polygon = new Polygon();
			polygon.Stroke = System.Windows.Media.Brushes.Black;
			polygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
			polygon.StrokeThickness = 2;
			polygon.HorizontalAlignment = HorizontalAlignment.Left;
			polygon.VerticalAlignment = VerticalAlignment.Center;
			polygon.Points.Add(from);
			polygon.Points.Add(to);
			return polygon;
		}

		private Polygon GetPointAt(Point point)
		{
			var polygon = new Polygon();
			polygon.Visibility = Visibility.Hidden;
			polygon.Stroke = System.Windows.Media.Brushes.Crimson;
			polygon.Fill = System.Windows.Media.Brushes.Red;
			polygon.StrokeThickness = 5;
			polygon.HorizontalAlignment = HorizontalAlignment.Left;
			polygon.VerticalAlignment = VerticalAlignment.Center;
			polygon.Points.Add(point);
			polygon.Points.Add(point);
			return polygon;
		}
	}
}
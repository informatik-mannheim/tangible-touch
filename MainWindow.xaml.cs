using System;
using System.Windows;
using System.Windows.Input;

namespace WpfTouchFrameSample
{
    public partial class MainWindow : Window
    {
        private int touches = 0;

        public MainWindow()
        {
            InitializeComponent();
         }

        public delegate void UpdateTextCallback(string text);

        void OnTouchDown(object sender, TouchEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            
            if (el == null) return;
            
            el.CaptureTouch(e.TouchDevice);

            touches++;
            Nummer.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), touches.ToString());

            e.Handled = true;
        }

        void OnTouchUp(object sender, TouchEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;

            if (el == null) return;

            TouchPoint tp = e.GetTouchPoint(el);

            Rect bounds = new Rect(new Point(0, 0), el.RenderSize);

            if (bounds.Contains(tp.Position))
            {
                touches--;
                Nummer.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), touches.ToString());
            }

            el.ReleaseTouchCapture(e.TouchDevice);

            e.Handled = true;
        }

        private void UpdateText(string message)
        {
            Nummer.Document.Blocks.Clear();
            Nummer.AppendText(message);
        }
    }
}
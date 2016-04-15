using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Collections;

namespace WpfTouchFrameSample
{
    public partial class MainWindow : Window
    {
        private int touches = 0;

        private object _lockobj = new Object();

        public MainWindow()
        {
            InitializeComponent();
         }

        public delegate void UpdateTextCallback(string text);

        private void UpdateText(string message)
        {
            Nummer.Document.Blocks.Clear();
            Nummer.AppendText(message);
        }

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
    }
}
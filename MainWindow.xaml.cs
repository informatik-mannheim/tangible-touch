using System;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using WpfApplication4;

namespace WpfTouchFrameSample
{
    public partial class MainWindow : Window
    {
        private int touches = 0;
        double[,] position = new double[8,2];
        ArrayList vectorList = new ArrayList();
      
        public MainWindow()
        {
            InitializeComponent();
         }

        public delegate void UpdateTextCallback(string text);

        TouchPoint _touchPoint;

        void OnTouchDown(object sender, TouchEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            
            if (el == null) return;
            
            el.CaptureTouch(e.TouchDevice);
            _touchPoint = e.TouchDevice.GetTouchPoint(this.canvas1);

            touches++;

            
            vectorList.Add(new Vector(_touchPoint.Position.X, _touchPoint.Position.Y));
            /*
            position[touches, 0] = _touchPoint.Position.X;
            position[touches, 1] = _touchPoint.Position.Y;
            touches++;
            */
            calculator(position);
        
            Nummer.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), touches.ToString());

            xy.AppendText(touches.ToString() + " X " + _touchPoint.Position.X.ToString() + " Y " + _touchPoint.Position.Y.ToString() +"\n");

            e.Handled = true;
        }

        void calculator(double[,] position )
        {
            int ix=0;
            int iy=0;
            int ret=0;
            Array dis = new double[8,2];

            double[,] leer; 
            if ( position[1, 0] == null) return;

            while ( ret <=8 )
            {
                dis = (position[ix] - position[ix++]);
            }


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
            // hier evtl. den calculator aufrufen
            Nummer.Document.Blocks.Clear();
            Nummer.AppendText(message);
           // xy.Document.Blocks.Clear();
        }
    }
}
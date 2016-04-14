using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections;
using UnityEngine;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;



namespace WpfTouchFrameSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Variables for tracking the position of two points.
        ArrayList ids;
        ArrayList list = new ArrayList();

  
        public MainWindow()
        {
            //TESTTESTESTESTE
            InitializeComponent();
            System.Windows.Input.Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);


            System.Threading.Thread printingThread = new System.Threading.Thread(new ThreadStart(printCount));
            ids = ArrayList.Synchronized(list);
            printingThread.Start();
         }

        public delegate void UpdateTextCallback(string text);

        private void UpdateText(string message)
        {
            Nummer.Document.Blocks.Clear();
            Nummer.AppendText(message);
        }

        public void printCount()
        {
            for (int i = 0; i <= 1000; i++)
            {
                Thread.Sleep(400);
                Nummer.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), ids.Count.ToString());
                Console.WriteLine(ids.Count.ToString());
                foreach (int s in ids)
                {
                    Console.WriteLine(s);
                }
            }
        }

        public void addTP(int tpId)
        {
            if (tpId != -1)
            {
                if (!ids.Contains(tpId))
                {
                    ids.Add(tpId);
                }
            }

        }

        public void removeTP(int tpId)
        {
            if (tpId != -1)
            {
                ids.Remove(tpId);
            }
        }

        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {

            TouchPointCollection _touchPoints;
            _touchPoints = e.GetTouchPoints(this.canvas1);
            int tpId=-1;

            foreach (TouchPoint tp in _touchPoints)
            {
                tpId = tp.TouchDevice.Id;
                if (tp.Action == TouchAction.Down)
                {
                    
                    addTP(tpId);
                    Console.WriteLine("id: " + tpId);
                }

                if (tp.Action == TouchAction.Up)
                {
                    removeTP(tpId);
                }
            }

            



            
            
            /*
           int pointsNumber = e.GetTouchPoints(this.canvas1).Count; 
                
            if (this.canvas1 != null)
            {
                foreach (TouchPoint _touchPoint in e.GetTouchPoints(this.canvas1))
                {
                    i++;
                  if (_touchPoint.Action == TouchAction.Down)
                    {
                        // Clear the canvas and capture the touch to it.
                        

                        this.canvas1.Children.Clear();
                        _touchPoint.TouchDevice.Capture(this.canvas1);

                     }

                    else if (_touchPoint.Action == TouchAction.Move && e.GetPrimaryTouchPoint(this.canvas1) != null)
                    {
                      //  id[i] = e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id;

                        // This is the first (primary) touch point. Just record its position.
                        if (_touchPoint.TouchDevice.Id == e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id)
                        {
                            pt1.X = _touchPoint.Position.X;
                            pt1.Y = _touchPoint.Position.Y;
                          
                            Nummer.Text = "1"; 
                        }

                        // This is not the first touch point. Draw a line from the first point to this one.
                        else if (_touchPoint.TouchDevice.Id != e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id)
                        {
                            pt2.X = _touchPoint.Position.X;
                            pt2.Y = _touchPoint.Position.Y;

                           // id[i] = e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id;
                          
                            Nummer.Text = "2";
                        }

                        else if (id[i] != e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id)
                        {
                            P[i].X = _touchPoint.Position.X;
                            P[i].Y = _touchPoint.Position.Y;
                            id[i] = e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id;
                            i++;
                            Nummer.Text = "3";
                        }

                        else if (id[i] != e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id)
                        {
                            P[i].X = _touchPoint.Position.X;
                            P[i].Y = _touchPoint.Position.Y;
                            id[i] = e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id;
                            i++;
                            Nummer.Text = "4";
                        }

                        else if ( id[i] != e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id)
                        {
                            P[i].X = _touchPoint.Position.X;
                            P[i].Y = _touchPoint.Position.Y;
                            id[i] = e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id;
                            i++;
                            Nummer.Text = "5";
                        }

                        else if (id[i] != e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id)
                        {
                            P[i].X = _touchPoint.Position.X;
                            P[i].Y = _touchPoint.Position.Y;
                            id[i] = e.GetPrimaryTouchPoint(this.canvas1).TouchDevice.Id;
                            i++;
                            Nummer.Text = "6";
                    }

                    else if (_touchPoint.Action == TouchAction.Up)
                    {
                        // If this touch is captured to the canvas, release it.
                        if (_touchPoint.TouchDevice.Captured == this.canvas1)
                        {
                            this.canvas1.ReleaseTouchCapture(_touchPoint.TouchDevice);
                            Nummer.Text = "0";
                        }
                    }


                }
                Console.WriteLine("Points: " + i);
            }
            */
        }
    }
}
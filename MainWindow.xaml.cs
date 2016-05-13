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
        private int countTouches = 1;
        private int countDistances = 0;
        // private double[,] position = new double[8, 2];
        // private ArrayList vectorList = new ArrayList();

        // map with all point objects
        // private Dictionary<int, Point> pointMap = new Dictionary<int, Point>();

        private Dictionary<int, TouchPoint> touchPointMap = new Dictionary<int, TouchPoint>();

        // map with all distances between the points
        private Dictionary<Array, double> distanceMap = new Dictionary<Array, double>();

        public delegate void UpdateTextCallback(string text);
        TouchPoint _touchPoint;

        public MainWindow()
        {
            InitializeComponent();
        }

        void OnTouchDown(object sender, TouchEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            element.CaptureTouch(e.TouchDevice);
            _touchPoint = e.TouchDevice.GetTouchPoint(this.grid1);


            // retrieve coordination of single point
            //Point point = new Point(_touchPoint.Position.X, _touchPoint.Position.Y);

            // add point to map
            //pointMap.Add(countTouches, point);
            touchPointMap.Add(countTouches, _touchPoint);
            // BUG an dieser Stelle !!! 
            // Exception: Ein Element mit dem gleichen Schlüssel wurde bereits hinzugefügt.

            // only calculate distance, if there is more than 1 touch point
            if (countTouches > 1)
            {
                Console.WriteLine("Detected Points: " + countTouches);
                // get one touch point
                // then calculate distance with the other touch points
                // create maps for touch points and distances
                for (int i = 1; i < countTouches; i++)
                {
                    // LOG distances
                    //Console.WriteLine("point " + countTouches + "i :" + i);

                    for (int j = i + 1; j <= countTouches; j++)
                    {
                        // LOG distances
                        //Console.WriteLine("i: " + i + "j: " + j + "\n");

                        // declare single-dimensional array for index i, j
                        int[] aIndex = new int[2];

                        // save index i, j as values
                        // later compare with keys of touch point map
                        aIndex[0] = i;
                        aIndex[1] = j;
                
                        double distance = calcDistance(touchPointMap[i], touchPointMap[j]);   
                       //Console.WriteLine("distance: " + distance);

                        // check if distance is in distanceMap
                        if (distanceMap.ContainsValue(distance))
                        {
                            Console.WriteLine("Die Distanz ist bereits in distanceMap enthalten");
                        }
                        else
                        {
                            // if it is not, add to distanceMap 
                            Console.WriteLine("aIndex UND distance in distanceMap hinzugefügt: " + aIndex + "    " + distance);
                            //xy.AppendText("distance " + distance + "\n");
                            xamlDistances.AppendText("\n" + "Distance between the Points " + distance);
                       
                            // add distance to map
                            distanceMap.Add(aIndex, distance);
                        }
                    }
                }
                // here to continue... 

            }
            else
            {
                // only one touch point...
            }

            Console.WriteLine("OnTouchDown..." + countTouches);
            Number.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), countTouches.ToString());
            xy.AppendText("\n" + "Point: " + countTouches.ToString() + "\n" + " X: " + _touchPoint.Position.X.ToString() + "\n" + " Y: " + _touchPoint.Position.Y.ToString());

            // count touch point, after calculating distance
            countTouches++;

      
            Console.WriteLine("OnTouchDown...");

            int counter = 1;
            foreach (var key in distanceMap)
            {

               // Console.WriteLine("Distance " + counter + " between " + "\n" + _touchPoint.Position.X.ToString() + " & " + _touchPoint.Position.Y.ToString()  + ": " + distanceMap[key.Key] + "\n");
                counter++;
            }
            e.Handled = true;
        }

        void OnTouchUp(object sender, TouchEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;

            if (el == null) return;

            // get touch point
            TouchPoint touchPoint = e.GetTouchPoint(el);

            // get x and y coordinates of touch point
            System.Windows.Point point = new System.Windows.Point(touchPoint.Position.X, touchPoint.Position.Y);

            Rect bounds = new Rect(new System.Windows.Point(0, 0), el.RenderSize);

            // remove touch point from point map
            foreach (var key in touchPointMap)
            {
               if (touchPointMap.ContainsKey(key.Key))
                {
                    if (touchPointMap[key.Key].Position.Equals(touchPoint.Position))
                    {
                        touchPointMap.Remove(key.Key);
                        countTouches--;

                        // remove distance from distance map
                        // {array} aKey: array with indexes of touch points
                        foreach (var aKey in distanceMap)
                        {
                            if (distanceMap.ContainsKey(aKey.Key))
                            {
                                for (int i = 0; i < aKey.Key.Length; i++)
                                {
                                    if (key.Key == (int)aKey.Key.GetValue(i))
                                    {
                                        distanceMap.Remove(aKey.Key);
                                        //Console.WriteLine("OnTouchUP..." + aKey);
                                        Number.Document.Blocks.Clear();
                                        xy.Document.Blocks.Clear();
                                        xamlDistances.Document.Blocks.Clear();
                                       
                                        return;

                                    }
                                } 
                            }
                        }return;
                    }
                }
            }

            if (bounds.Contains(touchPoint.Position))
            {
                //countTouches--;
                Console.WriteLine("OnTouchUP..." + countTouches);
                Number.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), countTouches.ToString());
            }

            /*Console.WriteLine("OnTouchUp...");

            int counter = 1;
            foreach (var key in distanceMap)
            {
                Console.WriteLine("Distance " + counter + ": " + distanceMap[key.Key]);
                counter++;
            }
            */
            el.ReleaseTouchCapture(e.TouchDevice);

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
        private void UpdateText(string message)
        {
            Number.Document.Blocks.Clear();
            Number.AppendText(message);
            //xy.Document.Blocks.Clear();
            //xy.AppendText(message);
            //xamlDistances.Document.Blocks.Clear();
            //xamlDistances.AppendText(message);

        }

        // Function calculates distance between two touch points
        // returns {double} distance
        private double calcDistance(TouchPoint p1, TouchPoint p2)
        {
            countDistances++;
           // System.Drawing.Graphics screen = new System.Drawing.Graphics();
           // double dpc = 82 / 2.54; //Dots Per Centimeter
            return Math.Sqrt(Math.Pow(p2.Position.X - p1.Position.X, 2) + Math.Pow(p2.Position.Y - p1.Position.Y, 2));
        }
    }
}
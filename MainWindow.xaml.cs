using System;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using WpfApplication4;


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
        private Dictionary<int, double> distanceMap = new Dictionary<int, double>();

        // map with index of distances
        private Dictionary<int, Array> mIndex = new Dictionary<int, Array>();

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
            _touchPoint = e.TouchDevice.GetTouchPoint(this.canvas1);

            // retrieve coordination of single point
            //Point point = new Point(_touchPoint.Position.X, _touchPoint.Position.Y);

            // add point to map
            //pointMap.Add(countTouches, point);
            touchPointMap.Add(countTouches, _touchPoint);

            // only calculate distance, if there is more than 1 touch point
            if (countTouches > 1)
            {
                Console.WriteLine("Point " + countTouches);
                // get one touch point
                // then calculate distance with the other touch points
                // create maps for touch points and distances
                for (int i = 1; i < countTouches; i++)
                {
                    // LOG distances
                    Console.WriteLine("point " + countTouches + "i :" + i);

                    for (int j = i + 1; j <= countTouches; j++)
                    {
                        // LOG distances
                        Console.WriteLine("i :" + i + "j :" + j);
                        // declare single-dimensional array for index i, j
                        int[] aIndex = new int[2];

                        // save index i, j as values
                        // later compare with keys of touch point map
                        aIndex[0] = i;
                        aIndex[1] = j;

                        // add distance to map
                        // {int} HashCode of aIndex as key
                        distanceMap.Add(aIndex.GetHashCode(), calcDistance(touchPointMap[i], touchPointMap[j]));

                        // add {array} aIndex to map 
                        // {int} countDistances as key
                        mIndex.Add(countDistances, aIndex);
                    }
                }
                // here to continue... 

            }
            else
            {
                // only one touch point...
            }

            Console.WriteLine("OnTouchDown..." + countTouches);
            Nummer.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), countTouches.ToString());

            // count touch point, after calculating distance
            countTouches++;

            xy.AppendText(countTouches.ToString() + " X " + _touchPoint.Position.X.ToString() + " Y " + _touchPoint.Position.Y.ToString() + "\n");

            Console.WriteLine("OnTouchDown...");

            int counter = 1;
            foreach (var key in distanceMap)
            {

                Console.WriteLine("Distance " + counter + ": " + distanceMap[key.Key]);
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

            Rect bounds = new Rect(new Point(0, 0), el.RenderSize);

            // remove touch point from point map
            // {int} key is the specific countTouches number of the touch point, which is saved in the map
            foreach (var key in touchPointMap)
            {
                Console.WriteLine("Teste 1..." + key);
                if (touchPointMap.ContainsKey(key.Key))
                {
                    Console.WriteLine("Teste 2...");
                    if (touchPointMap[key.Key].Position.Equals(touchPoint.Position))
                    {
                        Console.WriteLine("Teste 3...");
                        touchPointMap.Remove(key.Key);
                        countTouches--;

                        // remove distance from distance map
                        // {int} hashcode as the key of the distance map
                        foreach (var hashCode in distanceMap)
                        {
                            if (distanceMap.ContainsKey(hashCode.Key))
                            {
                                foreach (var iKey in mIndex)
                                {
                                    // check if hashCode {key} equals the hashCode of the {array} in mIndex[iKey.Key]
                                    if (hashCode.Key.Equals(mIndex[iKey.Key].GetHashCode()))
                                    {
                                        // loop through the {array} in mIndex[iKey.Key] which is 2 elements long
                                        for (int i = 0; i < mIndex[iKey.Key].Length; i++)
                                        {
                                            // get the element of the {array} in mIndex[iKey.Key]
                                            // which is the saved index i,j in aIndex above
                                            int element = (int)mIndex[iKey.Key].GetValue(i);

                                            // if the {int} key  (the specific countTouches number of the touch point) equals the index in the array
                                            // remove the distance
                                            if (key.Key == element)
                                            {
                                                distanceMap.Remove(hashCode.Key);
                                                countDistances--;
                                            }
                                        }
                                    }
                                }

                                /*
                                for (int i = 0; i < aKey.Key.Length; i++)
                                {
                                    if (key.Key == (int)aKey.Key.GetValue(i))
                                    {
                                        distanceMap.Remove(aKey.Key);

                                        if (distanceMap.Count == 0) 
                                        {
                                            return;
                                        }

                                    }
                                }*/
                            }
                        }
                    }
                }
            }

            if (bounds.Contains(touchPoint.Position))
            {
                //countTouches--;
                Console.WriteLine("OnTouchUP..." + countTouches);
                Nummer.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), countTouches.ToString());
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
            _touchPoint = e.TouchDevice.GetTouchPoint(this.canvas1);
            Console.WriteLine("test");

        }
        private void UpdateText(string message)
        {
            // hier evtl. den calculator aufrufen
            Nummer.Document.Blocks.Clear();
            Nummer.AppendText(message);
            // xy.Document.Blocks.Clear();
        }

        // Function calculates distance between two touch points
        // returns {double} distance
        private double calcDistance(TouchPoint p1, TouchPoint p2)
        {
            countDistances++;
            return Math.Sqrt(Math.Pow(p2.Position.X - p1.Position.X, 2) + Math.Pow(p2.Position.Y - p1.Position.Y, 2));
        }
    }
}
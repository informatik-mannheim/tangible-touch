using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;


namespace WpfApplication4.Touchcode
{
    class TouchcodePythonAPI : ITouchcodeAPI
    {
        public string _example = "[(0,0),(0,3),(1,3),(2,3),(0,2),(1,2),(2,2),(3,2),(0,1),(1,1),(2,1),(3,1),(1,0),(2,0),(3,0)]";

        public int Check(List<TouchPoint> touchpoints)
        {
            if (touchpoints == null || touchpoints.Count < 3)
            {
                return -1;
            }

            var basePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var scriptPath = Path.GetFullPath(Path.Combine(basePath, @"..\..\python\touchcode_cli.py"));

            return RunScript(scriptPath, Serialize(touchpoints));
        }

        private static int RunScript(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Python35\\python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.CreateNoWindow = true;

            try
            {
                Process process = Process.Start(start);
                process.WaitForExit();
                return process.ExitCode;
            }
            catch (Exception ex)
            {
                throw new TouchcodeSubprocessException(ex);
            }
        }

        public string Serialize(List<TouchPoint> touchpoints)
        {
            StringBuilder builder = new StringBuilder("[");

            for (int i = 0; i < touchpoints.Count; i++)
            {
                var tp = touchpoints[i];
                builder.AppendFormat("({0},{1}){2}", tp.Position.X, tp.Position.Y, i == touchpoints.Count - 1 ? "" : ",");
            }

            return builder.Append("]").ToString();
        }

        class TouchcodeSubprocessException : Exception
        {
            public TouchcodeSubprocessException(Exception ex)
                : base("Can not launch the touchcode subprocess", ex)
            { }

            public TouchcodeSubprocessException(String reason)
                : base(string.Format("Can not launch the touchcode subprocess [{0}]"))
            { }
        }

        public void CheckIfTouchcodeAPIWorks()
        {
            try
            {
                var tps = new List<TouchPoint>();

                tps.Add(new TouchPoint(new IHaveNoTouchDevice(666), new System.Windows.Point(0, 0), new Rect(), new TouchAction()));
                tps.Add(new TouchPoint(new IHaveNoTouchDevice(666), new System.Windows.Point(0, 3), new Rect(), new TouchAction()));
                tps.Add(new TouchPoint(new IHaveNoTouchDevice(666), new System.Windows.Point(3, 0), new Rect(), new TouchAction()));
                tps.Add(new TouchPoint(new IHaveNoTouchDevice(666), new System.Windows.Point(2, 2), new Rect(), new TouchAction()));

                Console.WriteLine(string.Format("Touchcode API is {0}", Check(tps) == 16 ? "working" : "NOT working"));
            }
            catch(TouchcodeSubprocessException ex)
            {
                Console.WriteLine("Touchcode API is NOT working");
            }

            
        }

        class IHaveNoTouchDevice : TouchDevice
        {
            public IHaveNoTouchDevice(int deviceId)
                : base(deviceId)
            { }

            public override TouchPointCollection GetIntermediateTouchPoints(IInputElement relativeTo)
            {
                throw new NotImplementedException();
            }

            public override TouchPoint GetTouchPoint(IInputElement relativeTo)
            {
                throw new NotImplementedException();
            }
        }

    }
}

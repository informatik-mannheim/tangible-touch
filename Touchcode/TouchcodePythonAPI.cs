using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;


namespace WpfApplication4.Touchcode
{
    class TouchcodePythonAPI
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

                tps.Add(new FakeTouchPoint(0, 0));
                tps.Add(new FakeTouchPoint(0, 3));
                tps.Add(new FakeTouchPoint(3, 0));
                tps.Add(new FakeTouchPoint(2, 2));

                Console.WriteLine(string.Format("Touchcode API is {0}", Check(tps) == 16 ? "working" : "NOT working"));
            }
            catch (TouchcodeSubprocessException)
            {
                Console.WriteLine("Touchcode API is NOT working");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;


namespace WpfApplication4
{
    class TouchcodeAPI
    {
        public static int Parse()
        {
            return RunScript("c:\\Users\\horst\\code\\tangibletouch\\mirror.py", "hallowelt");
        }

        private static int RunScript(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Program Files\\Python35\\python.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

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


        class TouchcodeSubprocessException : Exception
        {
            public TouchcodeSubprocessException(Exception ex)
                : base("Can not launch the touchcode subprocess", ex)
            { }

            public TouchcodeSubprocessException(String reason)
                : base(string.Format("Can not launch the touchcode subprocess [{0}]"))
            { }
        }
    }
}

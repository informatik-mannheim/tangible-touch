using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfApplication4.Touchcode
{
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

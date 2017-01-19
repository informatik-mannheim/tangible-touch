using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WpfApplication4
{
    interface ITouchcodeAPI
    {
        void CheckIfTouchcodeAPIWorks();
        int Check(List<TouchPoint> touchpoints);
        string Serialize(List<TouchPoint> touchpoints);
    }
}

using System;
using System.Collections.Generic;

namespace WpfApplication4.Touchcode
{
    class TouchcodeNativeAPI : ITouchcodeAPI
    {
        public void CheckIfTouchcodeAPIWorks()
        {
            throw new NotImplementedException();
        }

        public int Check(List<System.Windows.Input.TouchPoint> touchpoints)
        {
            if(touchpoints == null || touchpoints.Count < 3)
            {
                return -1;
            }

            return -1;
        }

        public string Serialize(List<System.Windows.Input.TouchPoint> touchpoints)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class TimeStampCalculator {
        public static DateTime GetTimeStamp() {
            TimeManager timeManager = ManagerBehaviour<TimeManager>.instance;
            return new DateTime(timeManager.today.Year, timeManager.today.Month, timeManager.today.Day);
        }
    }
}

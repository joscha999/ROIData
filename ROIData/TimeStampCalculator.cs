using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class TimeStampCalculator {
        public static Date GetTimeStamp() {
            TimeManager timeManager = ManagerBehaviour<TimeManager>.instance;
            return new Date(timeManager.today.Day, timeManager.today.Month, timeManager.today.Year);
        }
    }
}

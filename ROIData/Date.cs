using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData {
    public class Date {
        private const int DaysPerMonth = 30;
        private const int DaysPerYear = DaysPerMonth * 12;

        public int Day { get; }
        public int Month { get; }
        public int Year { get; }

        public int UnixDays => Day - 1 + ((Month - 4) * DaysPerMonth) + ((Year - 1) * DaysPerYear);

        public Date(int day, int month, int year) {
            Day = day;
            Month = month;
            Year = year;
        }

        public override string ToString() {
            return Year + "-" + Month + "-" + Day;
        }

		public static Date Now {
			get {
				var timeManager = ManagerBehaviour<TimeManager>.instance;
				return new Date(timeManager.day, timeManager.month, timeManager.year);
			}
		}
    }
}

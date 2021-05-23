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

        public int UnixDays => Day + (Month * DaysPerMonth) + (Year * DaysPerYear);

        public Date(int day, int month, int year) {
            Day = day;
            Month = month;
            Year = year;
        }

        public override string ToString() {
            return Year + "-" + Month + "-" + Day;
        }
    }
}

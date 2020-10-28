using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData {
    public class Date {
        public int Day { get; }
        public int Month { get; }
        public int Year { get; }

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

using System;
using ROIData;

namespace Planspiel.Models {
    public class SaveDataModel {
        public long SteamID { get; set; }

        /// <summary>
        /// Game time at which this save data was created.
        /// </summary>
        public Date Date { get; set; }

        public double Profit { get; set; }

        public double CompanyValue { get; set; }

        public double DemandSatisfaction { get; set; }

        /// <summary>
        /// Average machine uptime.
        /// </summary>
        public double MachineUptime { get; set; }

        public bool AbleToPayLoansBack { get; set; }

        public double AveragePollution { get; set; }
    }
}

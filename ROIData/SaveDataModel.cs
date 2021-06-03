using System;
using System.Collections.Generic;
using ROIData;

namespace Planspiel.Models {
    public class SaveDataModel {
        public long SteamID { get; set; }

        /// <summary>
        /// Game time at which this save data was created.
        /// </summary>
        public int UnixDays { get; set; }

        public float PassedTime { get; set; }

        public double Profit { get; set; }

        public double CompanyValue { get; set; }

        public List<ProductDemandInfo> DemandSatisfaction { get; set; }

        /// <summary>
        /// Average machine uptime.
        /// </summary>
        public double MachineUptime { get; set; }

        public List<LoanInfo> LoansList { get; set; }

        public double AveragePollution { get; set; }

        public int BuildingCount { get; set; }

        public int UnlockedResearchCount { get; set; }

        public int RegionCount { get; set; }

        public double Balance { get; set; }
    }
}

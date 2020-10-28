﻿using System.Collections.Generic;
using System.Linq;
using ProjectAutomata;

namespace ROIData {
    public static class PollutionCalculator {

        private static readonly RegionManager regionManager = ManagerBehaviour<RegionManager>.instance;
        private static readonly PermitManager permitManager = ManagerBehaviour<PermitManager>.instance;
        private static readonly PollutionManager pollutionManager = ManagerBehaviour<PollutionManager>.instance;
        private static readonly ICollection<Region> regions 
            = ROIDataMod.GetField<ICollection<Region>>(typeof(RegionManager), "_serializedRegions", regionManager);

        public static double GetAveragePollution() 
            => regions.Where(r => permitManager.OwnsPermitForRegion(ROIDataMod.Player, r, null))
                      .SelectMany(r => r.tiles).Average(t => pollutionManager.GetPollution(t));
    }
}

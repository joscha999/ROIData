using System.Collections.Generic;
using System.Linq;
using ProjectAutomata;

namespace ROIData {
    public static class RegionCountCalculator {
        private static readonly RegionManager regionManager = ManagerBehaviour<RegionManager>.instance;
        private static readonly PermitManager permitManager = ManagerBehaviour<PermitManager>.instance;
        private static readonly ICollection<Region> regions 
            = ROIDataMod.GetField<ICollection<Region>>(typeof(RegionManager), "_serializedRegions", regionManager);

        public static int GetRegionsCount() => regions.Count(r => permitManager.OwnsPermitForRegion(ROIDataMod.Player, r, null));
    }
}

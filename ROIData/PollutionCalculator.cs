using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class PollutionCalculator {

        static readonly RegionManager regionManager = ManagerBehaviour<RegionManager>.instance;
        static readonly PermitManager permitManager = ManagerBehaviour<PermitManager>.instance;
        static readonly PollutionManager pollutionManager = ManagerBehaviour<PollutionManager>.instance;
        static readonly ICollection<Region> regions = ROIDataMod.GetField<ICollection<Region>>(typeof(RegionManager), "_serializedRegions", regionManager);
        static readonly List<string> cleanerBuildings = new List<string> { "Air Purifier", "Chemical Scrubber", "Water Treatment Plant" };
        static readonly List<string> ignoreBuildings = new List<string> { "Headquarters", "WATER SIPHON", "WATER PUMP", "CROP FARM", "POTATO FIELD", "LUMBERYARD", "LUMBERYARD HARVESTER", "SAND COLLECTOR", "SAND HARVESTER", "PLANTATION", "BERRY FIELD" };
        public static double GetAveragePollution() {
            double pollution = 0;
            int tiles = 0;

            foreach (var region in regions) {
                foreach (int tile in region.tiles) {
                    if (permitManager.OwnsPermitForRegion(ROIDataMod.Player, region, null)) {
                        tiles++;
                        pollution += pollutionManager.GetPollution(tile);
                    }
                }
            }

            return pollution / tiles;
        }

        public static int GetPollutedTilesCount() {
            int pollutedTilesCount = 0;
            foreach (Region region in regions) {
                foreach (int tile in region.tiles) {
                    if (pollutionManager.GetPollution(tile) > 0 && permitManager.OwnsPermitForRegion(ROIDataMod.Player, region, null))
                        pollutedTilesCount++;
                }
            }
            return pollutedTilesCount;
        }

        public static double GetCleanerRatio() {
            var cleaners = 0f;
            var factories = 0f;

            foreach (Building building in ROIDataMod.Player.buildings) {
                if (ignoreBuildings.Contains(building.name))
                    continue;

                if (cleanerBuildings.Contains(building.name)) {
                    cleaners++;
                } else {
                    factories++;
                }
            }
            return cleaners / factories;
        }
    }
}

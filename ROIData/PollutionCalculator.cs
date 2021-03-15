using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectAutomata;
using UnityEngine;

namespace ROIData {
    public static class PollutionCalculator {

        private static readonly RegionManager regionManager = ManagerBehaviour<RegionManager>.instance;
        private static readonly PermitManager permitManager = ManagerBehaviour<PermitManager>.instance;
        private static readonly PollutionManager pollutionManager = ManagerBehaviour<PollutionManager>.instance;
        private static readonly ICollection<Region> regions 
            = ROIDataMod.GetField<ICollection<Region>>(typeof(RegionManager), "_serializedRegions", regionManager);
        private static readonly BuildingManager buildingManager = ManagerBehaviour<BuildingManager>.instance;

        public static double GetAveragePollution() 
            => regions.Where(r => permitManager.OwnsPermitForRegion(ROIDataMod.Player, r, null))
                      .SelectMany(r => r.tiles).Average(t => pollutionManager.GetPollution(t));

        private static double GetTileCountOfOwnedRegions() {
            var tileCount = 0;

            foreach (var r in regions) {
                if (permitManager.OwnsPermitForRegion(ROIDataMod.Player, r, null)) {
                    foreach (var t in r.tiles) {
                        tileCount++;
                    }
                }
            }

            return tileCount;
        }
        public static void GetAveragePollutionAdv() {

            var pollution = 0f;
            List<int> pollutedTiles = new List<int>();
            List<Building> polluterBuildings = new List<Building>();

            //look at every building
            foreach (var b in buildingManager.buildingsList) {
                //check if building is owned by player & check if building creates pollution
                if (b.buildingOwner is HumanPlayer) {
                    var polluter = b.GetComponent<PolluterBuilding>();

                    if (polluter != null) {
                        polluterBuildings.Add(b);
                        List<int> affectedTiles = ROIDataMod.GetField<List<int>>(typeof(PolluterBuilding), "_affectedTiles", polluter);

                        //go through all affected tiles
                        foreach (var tile in affectedTiles) {
                            //check for duplicates
                            if (!pollutedTiles.Contains(tile)) {
                                //add to tile list
                                pollutedTiles.Add(tile);
                                //add pollution to pollution count
                                pollution += pollutionManager.GetPollution(tile);
                            }
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Pollution Analysis in Detail: ")
                .AppendLine("- PolluterBuildingCount: ").Append(polluterBuildings.Count)
                .AppendLine("- Buildings: ");

            foreach (var polluter in polluterBuildings) {
                sb.Append("  -> " + polluter.buildingName).AppendLine();
            }

            sb.AppendLine("- Polluted Tiles Count: ").Append(pollutedTiles.Count)
                .AppendLine("- Polluted Tiles Percentage: ").Append((pollutedTiles.Count / GetTileCountOfOwnedRegions()) * 100).Append("%")
                .AppendLine("- Average Pollution of Polluted Tiles: ").Append(pollution / pollutedTiles.Count())
                .AppendLine("- Average Pollution of Tiles: ").Append(GetAveragePollution());

            Debug.Log(sb.ToString());
        }
    }
}

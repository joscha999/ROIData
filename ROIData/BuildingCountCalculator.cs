using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class BuildingCountCalculator {
        private static readonly BuildingManager buildingManager = ManagerBehaviour<BuildingManager>.instance;

        public static int GetBuildingCount() => buildingManager.buildingsList.Count(b => b.buildingOwner is HumanPlayer);
    }
}

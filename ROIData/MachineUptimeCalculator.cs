using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class MachineUptimeCalculator {
        public static double GetAverageMachineUptime() {
            if (ROIDataMod.Player.buildings.recipeUsers.Count == 0)
                return 1;

            return ROIDataMod.Player.buildings.recipeUsers.Average(f => f.GetComponent<BuildingAnalysis>()
                            .GetLastMonthValue(f.GetComponent<Upkeep>().upkeepAnalysisItemDef) / 100);
        }

        //double uptimeSum = 0;

        //foreach (RecipeUser factory in factories) {
        //    BuildingAnalysis buildingAnalysis = factory.GetComponent<BuildingAnalysis>();
        //    Upkeep upkeep = factory.GetComponent<Upkeep>();
        //    uptimeSum += (buildingAnalysis.GetLastMonthValue(upkeep.uptimeAnalysisItemDef) / 100);
        //}
    }
}

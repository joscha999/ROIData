using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class MachineUptimeCalculator {
        public static double GetAverageMachineUptime() {
            List<RecipeUser> factories = ROIDataMod.Player.buildings.recipeUsers;
            double uptimeSum = 0;

            foreach (RecipeUser factory in factories) {
                BuildingAnalysis buildingAnalysis = factory.GetComponent<BuildingAnalysis>();
                Upkeep upkeep = factory.GetComponent<Upkeep>();

                uptimeSum += (buildingAnalysis.GetLastMonthValue(upkeep.uptimeAnalysisItemDef) / 100);
            }
            return uptimeSum / factories.Count;
        }
    }
}

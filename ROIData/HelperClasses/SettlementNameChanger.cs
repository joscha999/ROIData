using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData.HelperClasses
{
    public static class SettlementNameChanger
    {
        //TODO: Create actual map -> look at names -> rename keys in dictionary.
        private static Dictionary<string, string> oldNewSettlementNames = new Dictionary<string, string>()
        {
            {"Wocklum","Zuckerrost"},{"Dingsfelde","Leutingen"},{"Dreistegen","Höllenpforte"},{"Sorsum","Irmgardshausen"},{"Hilten","Fuchsfeste"},{"Haitzen","Magdalenenhütte"},{"Derscheid","Leberthal"}
        };
        public static void ChangeName(SettlementBase settlement, string name)
        {
            typeof(Settlement).GetField("_settlementName", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(settlement, name);
        }

        public static void RenameAll()
        {
            var settlements = ManagerBehaviour<SettlementManager>.instance.settlements;
            foreach (var settlement in settlements)
            {
                if (oldNewSettlementNames.TryGetValue(settlement.settlementName, out var newName))
                {
                    ChangeName(settlement, newName);
                }
            }
        }
    }
}

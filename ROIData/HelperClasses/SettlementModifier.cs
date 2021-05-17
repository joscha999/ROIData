using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;
using UnityEngine;

namespace ROIData.HelperClasses
{
    public static class SettlementModifier
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

        public static void ForceGrowth(SettlementBase settlement, int people) {

            typeof(Settlement).GetField("_population", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(settlement, settlement.population + people);
        }

        public static void GetInfo(SettlementBase settlement) {
            Debug.Log(typeof(Settlement).GetField("_type", BindingFlags.NonPublic | BindingFlags.Instance).ToString());
        }

        public static void ForceAdvancement(SettlementBase settlement) {
            var bla = ManagerBehaviour<SettlementManager>.instance.GetComponent<SettlementAdvancement>();
        }
    }
}

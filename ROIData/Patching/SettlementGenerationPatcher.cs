using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;
using Harmony;
using UnityEngine;
using ROIData.HelperClasses;

namespace ROIData.Patching {
    
    [HarmonyPatch(typeof(SettlementsWGOperation))]
    [HarmonyPatch("MakeSettlement")]
    public class SettlementGenerationPatcher {
        private static Dictionary<string, int> SettlementPopulationDict = new Dictionary<string, int>() {
            {"Effie", 300_000}, {"Point Venture", 100_000}, {"Sammy Martinez", 100_000}, 
            {"Chula Vista", 100_000}, {"Apple Canyon Lake", 100_000}, {"Caballo", 300_000}
        };

        private static Dictionary<string, string> OldAndNewNamesDict = new Dictionary<string, string>() {
            {"Effie", "Zuckerrost"}, {"Point Venture", "Leutingen"}, {"Sammy Martinez", "Höllenpforte"}, 
            {"Chula Vista", "Fuchsfeste"}, {"Apple Canyon Lake", "Magdalenenhütte"}, {"Caballo", "Irmgardshausen"}
        };

        //Prefix
        static bool Prefix() => false;

        //postfix
        static void Postfix(ref Region region, ref SettlementType settlementType, ref SettlementBase __result, SettlementsWGOperation __instance) {
            Debug.Log("Patching region: " + region.regionName);

            var st = settlementType;
            var config = Reflection.GetField<List<SettlementConfig>>(typeof(SettlementsWGOperation), "_settlementConfigs", __instance)
                .FindAll((SettlementConfig sc) => sc.CanSpawnType(st)).RandomItem();

            //Set Population Size
            if (!SettlementPopulationDict.TryGetValue(region.regionName, out int population)) {
                Debug.Log("PopulationAdjust: RegionName not present in Dictionary: " + region.regionName);
                population = 50_000;
            }

            if (!OldAndNewNamesDict.TryGetValue(region.regionName, out string regionName)) {
                Debug.Log("NameChange: RegionName not present in Dictionary: " + region.regionName);
                regionName = region.regionName;
            }

            __result = ManagerBehaviour<SettlementManager>.instance.CreateSettlement(regionName, settlementType, population, region, config);
        }
    }
}
using Harmony;
using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ROIData.Patching {
    [HarmonyPatch(typeof(Shop))]
    [HarmonyPatch("Initialize")]
    public class ShopProductPatcher {
        private static SettlementManager SettlementManager = ManagerBehaviour<SettlementManager>.instance;
        private static List<SettlementBase> Settlements = SettlementManager.settlements;

        private static Dictionary<string, string[]> SettlementShopProducts = new Dictionary<string, string[]>() {
            {"Neumorsum", new string[3]{"Baumarkt", "Sand", "Coal"} }
        };

        public static void PrintShopData() {
            StringBuilder sb = new StringBuilder();

            foreach (var settlement in Settlements) {
                sb.AppendLine("Shops in settlement: " + settlement.settlementName);

                foreach (var shop in settlement.buildings.shops) {
                    sb.AppendLine("Shop: " + shop.name);

                    foreach (var product in shop.sold) {
                        sb.Append(product.name + ";");
                    }

                    sb.AppendLine();

                    foreach (var tag in shop.soldTags) {
                        sb.Append(tag.ToString());
                    }

                    sb.AppendLine();
                }

                sb.AppendLine()
                    .AppendLine();
            }

            Debug.Log(sb.ToString());
        }

        static bool Prefix() => false;

        static void Postfix(ref List<ProductDefinition> __result, Shop __instance) {
            Debug.Log("ShopProductPatcher executed.");

            foreach (var settlement in Settlements) {
                if (!SettlementShopProducts.TryGetValue(settlement.settlementName, out string[] shopChangeArray)) {
                    Debug.Log("Settlement not found: " + settlement.settlementName);
                }

                if (shopChangeArray[0] == __instance.name) {
                    List<ProductDefinition> products = __instance.sold;
                    products[products.FindIndex(i => i.Equals(GetProductDefinition(shopChangeArray[1])))] = GetProductDefinition(shopChangeArray[2]);
                    __result = products;
                }
            } 
        }

        private static ProductDefinition GetProductDefinition(string name) {
            return GameData.instance.GetAssetsRO(typeof(ProductDefinition))
                    .FirstOrDefault(p => p.name == name) as ProductDefinition;
        }
    }
}
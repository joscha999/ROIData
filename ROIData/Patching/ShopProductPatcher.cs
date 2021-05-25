using Harmony;
using ProjectAutomata;
using ROIData.HelperClasses;
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

        //Zuckerrost

        //Lokal: Pizza -> Burgers
        //BAUTEIL-GESCHÄFT RubberTubes -> InteriorBody
        //Lebensmittelgeschäft: CannedFish -> Flour

        //Irmgardshausen

        //Spielzeugladen: Toy Train Set -> Toy Furniture
        //Lokal: Burgers -> Pizza

        //Magdalenenhütte

        //Lebensmittelgeschäft: Flour -> CannedFish

        //Leutingen

        //Lebensmittelgeschäft: BagOfChips -> CannedFish

        private static readonly List<ReplaceInformation> ReplaceInfo = new List<ReplaceInformation>() {
            new ReplaceInformation {
                Settlement = "Neumorsum",
                Shop = "Spielzeugladen",
                OldProduct = "Marbles",
                NewProduct = "Burgers"
            }
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

        static bool Prefix(Shop __instance) => false;
            //var b = !ReplaceInfo.Any(ri => ri.Settlement == __instance.settlement.settlementName && ri.Shop == __instance.name);
            //Debug.Log($"Prefix:: Settlement: {__instance.settlement.settlementName}, Shop: {__instance.name}, B: {b}");

        static void Postfix(Shop __instance) {
            Debug.Log($"Postfix:: Settlement: {__instance.settlement.settlementName}, Shop: {__instance.name}");
            //if (Reflection.GetField<bool>(typeof(Shop), "initialized", __instance))
            //    return;

            __instance.sold = __instance.GetValidProducts(__instance.settlement, new List<ProductDefinition>())
                .Take(__instance.maxProducts).ToList();

            var b = false;
            foreach (var ri in ReplaceInfo) {
                if (ri.Settlement != __instance.settlement.settlementName || ri.Shop != __instance.name)
                    continue;

                __instance.sold.RemoveAll(pd => pd.name == ri.OldProduct);
                __instance.sold.Insert(0, ri.NewProductDef);
                b = true;
                Debug.Log($"Product {ri.OldProduct} replaced with {ri.NewProduct} in Shop {__instance.name} in Settlement {__instance.settlement.settlementName}.");
            }

            __instance.UpdateDemand();
            Reflection.ExecuteMethod(typeof(Shop), "UpdatePriceModifiers", __instance);
            Reflection.SetField(typeof(Shop), "initialized", __instance, true);

            if (!b)
                Debug.Log($"Settlement: {__instance.settlement.settlementName}, Shop: {__instance.name}. No replacementinfo found.");
        }

        private static ProductDefinition GetProductDefinition(string name) {
            return GameData.instance.GetAssetsRO(typeof(ProductDefinition))
                    .FirstOrDefault(p => p.name == name) as ProductDefinition;
        }

        private class ReplaceInformation {
            public string Settlement { get; set; }
            public string Shop { get; set; }
            public string OldProduct { get; set; }
            public string NewProduct { get; set; }

            public ProductDefinition NewProductDef => GetProductDefinition(NewProduct);
        }
    }
}
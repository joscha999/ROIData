using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ROIData.Patching {
    public class ShopProductPatcher {

        public static void PrintShopData() {
            SettlementManager settlementManager = ManagerBehaviour<SettlementManager>.instance;
            List<SettlementBase> settlements = settlementManager.settlements;

            StringBuilder sb = new StringBuilder();

            foreach (var settlement in settlements) {
                sb.AppendLine("Shops in settlement: " + settlement.settlementName);

                foreach (var shop in settlement.buildings.shops) {
                    sb.AppendLine("Shop: " + shop.name);

                    foreach (var product in shop.sold) {
                        sb.Append(product.name + ";");
                    }
                }

                sb.AppendLine();
            }

            Debug.Log(sb.ToString());
        }

        static bool Prefix() => false;

        static void Postfix() {

        }
    }
}

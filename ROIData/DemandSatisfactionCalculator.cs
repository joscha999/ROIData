using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;
using UnityEngine;

namespace ROIData {
    public static class DemandSatisfactionCalculator {
        public static double CalculateAverageDemandSatisfaction() {
            //shops (in player settlement)
            List<Shop> shops = ROIDataMod.Player.hq.settlement.buildings.shops;
            
            //counters
            int offset = 0;
            StringBuilder sb = new StringBuilder();

            foreach (Shop shop in shops) {
                foreach (KeyValuePair<ProductDefinition, int> shopDemand in ROIDataMod.GetField<Dictionary<ProductDefinition, int>>(typeof(Shop), "_demand", shop)) {
                    ProductDefinition product = shopDemand.Key;
                    int demand = shopDemand.Value * 2;
                    int sales = shop.GetSoldCount(ROIDataMod.Player, product, new GamePeriod(0, 0, 29));

                    //only register if something has been sold (aka. product is being produced by player)
                    if (sales > 0) {
                        offset += Math.Abs(sales - demand);
                        sb.Append(shop.buildingName).Append(" requests ")
                            .Append(demand).Append(" units of ")
                            .Append(product.productName).Append(". ")
                            .Append(offset).AppendLine(" units offset.");
                    }
                }
            }
            //return average offset
            Debug.Log(sb.ToString());
            return offset;
        }
    }
}

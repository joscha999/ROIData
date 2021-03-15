using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;
using UnityEngine;

namespace ROIData {
    public static class DemandSatisfactionCalculator {
        private static List<Shop> Shops => ROIDataMod.Player.hq.settlement.buildings.shops;

        public static double CalculateAverageDemandSatisfaction() {
            
            //counters
            int offset = 0;
            StringBuilder sb = new StringBuilder();

            foreach (Shop shop in Shops) {
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

        public static List<ProductInfo> RemainingDemands() {

            List<ProductInfo> listOfRemainingDemands = new List<ProductInfo>();

            foreach (Shop shop in Shops) {
                foreach (KeyValuePair<ProductDefinition, int> shopDemand in ROIDataMod.GetField<Dictionary<ProductDefinition, int>>(typeof(Shop), "_demand", shop)) {
                    ProductDefinition product = shopDemand.Key;
                    int demand = shopDemand.Value * 2;
                    int sales = shop.GetSoldCount(ROIDataMod.Player, product, new GamePeriod(0, 0, 29));

                    if (sales > 0) {
                        int offset = Math.Abs(sales - demand);
                        string productName = product.productName;

                        listOfRemainingDemands.Add(new ProductInfo(productName, offset));
                    }
                }
            }

            return listOfRemainingDemands;
        }
    }
}

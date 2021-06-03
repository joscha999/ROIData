using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;
using ROIData.HelperClasses;
using UnityEngine;

namespace ROIData {
    public static class DemandSatisfactionCalculator {
        private static List<Shop> Shops => ROIDataMod.Player.hq.settlement.buildings.shops;

        public static double CalculateAverageDemandSatisfaction() {
            
            //counters
            int offset = 0;
            StringBuilder sb = new StringBuilder();

            foreach (Shop shop in Shops) {
                foreach (KeyValuePair<ProductDefinition, int> shopDemand in Reflection.GetField<Dictionary<ProductDefinition, int>>(typeof(Shop), "_demand", shop)) {
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

        //public static List<ProductDemandInfo> RemainingDemands() {

        //    List<ProductDemandInfo> listOfRemainingDemands = new List<ProductDemandInfo>();

        //    foreach (Shop shop in Shops) {
        //        foreach (KeyValuePair<ProductDefinition, int> shopDemand in Reflection.GetField<Dictionary<ProductDefinition, int>>(typeof(Shop), "_demand", shop)) {
        //            ProductDefinition product = shopDemand.Key;
        //            int demand = shopDemand.Value * 2;
        //            int sales = shop.GetSoldCount(ROIDataMod.Player, product, new GamePeriod(0, 0, 29));

        //            if (sales > 0) {
        //                int offset = Math.Abs(sales - demand);
        //                string productName = product.productName;

        //                listOfRemainingDemands.Add(new ProductInfo(productName, offset));
        //            }
        //        }
        //    }

        //    return listOfRemainingDemands;
        //}

        //This method returns the ProductInfo for all shops, regardless of settlement.
        public static List<ProductDemandInfo> GetProductDemandInfos() {
            List<ProductDemandInfo> productInfos = new List<ProductDemandInfo>();
            List<SettlementBase> settlements = ManagerBehaviour<SettlementManager>.instance.settlements;

            foreach (var settlement in settlements) {
                foreach (var shop in settlement.buildings.shops) {
                    foreach (var shopDemand in Reflection.GetField<Dictionary<ProductDefinition, int>>(typeof(Shop), "_demand", shop)) {
                        ProductDefinition productDefinition = shopDemand.Key;
                        int productDemand = shopDemand.Value;
                        int productSalesLastMonth = shop.GetSoldCount(ROIDataMod.Player, productDefinition, GamePeriod.month);

                        if (productSalesLastMonth > 0) {
                            productInfos.Add(new ProductDemandInfo(productDefinition.productName, settlement.settlementName, shop.name, productDemand, productSalesLastMonth));
                        }
                    }
                }
            }

            return productInfos;
        }
    }
}

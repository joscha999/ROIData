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
    [HarmonyPatch("GetPrice")]
    public class ShopPricePatcher {
		private static readonly Dictionary<Shop, Dictionary<ProductDefinition, float>> ReplaceCache
			= new Dictionary<Shop, Dictionary<ProductDefinition, float>>();

        private static readonly List<ReplaceInformation> ReplaceInfo = new List<ReplaceInformation> {
            //OrangeSoda
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "Lokal", NewProduct = "OrangeSoda", NewPrice = 94_000
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Lokal", NewProduct = "OrangeSoda", NewPrice = 94_000
            },
            //Headlights
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "BAUTEIL-GESCHÄFT", NewProduct = "Headlights", NewPrice = 90_000
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "BAUTEIL-GESCHÄFT", NewProduct = "Headlights", NewPrice = 90_000
            },
            //Marbles
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "Spielzeugladen", NewProduct = "Marbles", NewPrice = 60_000
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Baumarkt", NewProduct = "Marbles", NewPrice = 60_000
            },
            //CannedFish
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Lebensmittelgeschäft", NewProduct = "CannedFish", NewPrice = 188_000
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Lebensmittelgeschäft", NewProduct = "CannedFish", NewPrice = 197_400 //+5%
            },
            //Telephone
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Haushaltsgüter", NewProduct = "Telephone", NewPrice = 194_000
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Haushaltsgüter", NewProduct = "Telephone", NewPrice = 203_700 //+5%
            },
            //Toy Train Set
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Spielzeugladen", NewProduct = "Toy Train Set", NewPrice = 75_000
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Spielzeugladen", NewProduct = "Toy Train Set", NewPrice = 78_750 //+5%
            },
            //Beer
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Spirituosengeschäft", NewProduct = "Beer", NewPrice = 72_000
            },
            //Burgers
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Lokal", NewProduct = "Burgers", NewPrice = 86_000
            },
            //CombustionEngine
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "BAUTEIL-GESCHÄFT", NewProduct = "CombustionEngine", NewPrice = 565_000
            },
            //InteriorBody
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "BAUTEIL-GESCHÄFT", NewProduct = "InteriorBody", NewPrice = 442_000
            },
            //English
            //OrangeSoda
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "Diner", NewProduct = "OrangeSoda", NewPrice = 94_000
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Diner", NewProduct = "OrangeSoda", NewPrice = 94_000
            },
            //Headlights
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "PARTS SHOP", NewProduct = "Headlights", NewPrice = 90_000
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "PARTS SHOP", NewProduct = "Headlights", NewPrice = 90_000
            },
            //Marbles
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "Toy Store", NewProduct = "Marbles", NewPrice = 60_000
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Hardware Store", NewProduct = "Marbles", NewPrice = 60_000
            },
            //CannedFish
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Grocery Store", NewProduct = "CannedFish", NewPrice = 188_000
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Grocery Store", NewProduct = "CannedFish", NewPrice = 197_400 //+5%
            },
            //Telephone
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Home Goods", NewProduct = "Telephone", NewPrice = 194_000
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Home Goods", NewProduct = "Telephone", NewPrice = 203_700 //+5%
            },
            //Toy Train Set
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Toy Store", NewProduct = "Toy Train Set", NewPrice = 75_000
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Toy Store", NewProduct = "Toy Train Set", NewPrice = 78_750 //+5%
            },
            //Beer
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Liquor Store", NewProduct = "Beer", NewPrice = 72_000
            },
            //Burgers
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Diner", NewProduct = "Burgers", NewPrice = 86_000
            },
            //CombustionEngine
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "PARTS SHOP", NewProduct = "CombustionEngine", NewPrice = 565_000
            },
            //InteriorBody
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "PARTS SHOP", NewProduct = "InteriorBody", NewPrice = 442_000
            }
        };

        public static void DebugPriceModifiers() {
            StringBuilder sb = new StringBuilder();
            foreach (var settlement in ManagerBehaviour<SettlementManager>.instance.settlements) {
                sb.AppendLine("Shops in settlement: " + settlement.settlementName);

                foreach (var shop in settlement.buildings.shops) {
                    sb.AppendLine("Shop: " + shop.name);

                    var modifiers = Reflection.GetField<Shop.PriceModifiers>(typeof(Shop), "_priceModifiers", shop);
                    foreach (var item in modifiers) {
                        sb.Append($"{item.Key.productName} with modifier {item.Value}").AppendLine();
                    }
                }
            }

            Debug.Log(sb.ToString());
        }

        static void Postfix(Shop __instance, ref float __result, ref ProductDefinition product) {
			__result = GetNewPrice(__instance, product);
		}

		private static float GetNewPrice(Shop s, ProductDefinition product) {
			if (!ReplaceCache.TryGetValue(s, out var priceDict)) {
				priceDict = new Dictionary<ProductDefinition, float>();
				ReplaceCache.Add(s, priceDict);
			}

			if (!priceDict.TryGetValue(product, out var newPrice)) {
				newPrice = FindNewPrice(s, product);
				priceDict.Add(product, newPrice);
			}

			return newPrice;
		}

		private static float FindNewPrice(Shop s, ProductDefinition product) {
			foreach (var ri in ReplaceInfo) {
				if (ri.Settlement != s.settlement.settlementName || ri.Shop != s.name)
					continue;

				if (product == ri.NewProductDef)
					return ri.NewPrice;
			}

			return s.GetPrice(product, ROIDataMod.Player);
		}
    }
}

using Harmony;
using ProjectAutomata;
using ROIData.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.Patching {
    [HarmonyPatch(typeof(Shop))]
    [HarmonyPatch("UpdateDemand")]
    public class ShopDemandPatcher {
        private static readonly Dictionary<Shop, Dictionary<ProductDefinition, int>> ReplaceCache
            = new Dictionary<Shop, Dictionary<ProductDefinition, int>>();

        private static readonly List<ReplaceInformation> ReplaceInfo = new List<ReplaceInformation> {
            //OrangeSoda
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "Lokal", NewProduct = "OrangeSoda", NewDemand = 4
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Lokal", NewProduct = "OrangeSoda", NewDemand = 4
            },
            //Headlights
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "BAUTEIL-GESCHÄFT", NewProduct = "Headlights", NewDemand = 4
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "BAUTEIL-GESCHÄFT", NewProduct = "Headlights", NewDemand = 4
            },
            //Marbles
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "Spielzeugladen", NewProduct = "Marbles", NewDemand = 4
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Baumarkt", NewProduct = "Marbles", NewDemand = 4
            },
            //CannedFish
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Lebensmittelgeschäft", NewProduct = "CannedFish", NewDemand = 2
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Lebensmittelgeschäft", NewProduct = "CannedFish", NewDemand = 2
            },
            //Telephone
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Haushaltsgüter", NewProduct = "Telephone", NewDemand = 2
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Haushaltsgüter", NewProduct = "Telephone", NewDemand = 2
            },
            //Toy Train Set
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Spielzeugladen", NewProduct = "Toy Train Set", NewDemand = 4
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Spielzeugladen", NewProduct = "Toy Train Set", NewDemand = 4
            },
            //Beer
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Spirituosengeschäft", NewProduct = "Beer", NewDemand = 4
            },
            //Burgers
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Lokal", NewProduct = "Burgers", NewDemand = 4
            },
            //CombustionEngine
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "BAUTEIL-GESCHÄFT", NewProduct = "CombustionEngine", NewDemand = 1
            },
            //InteriorBody
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "BAUTEIL-GESCHÄFT", NewProduct = "InteriorBody", NewDemand = 1
            },
            //English
            //OrangeSoda
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "Diner", NewProduct = "OrangeSoda", NewDemand = 4
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Diner", NewProduct = "OrangeSoda", NewDemand = 4
            },
            //Headlights
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "PARTS SHOP", NewProduct = "Headlights", NewDemand = 4
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "PARTS SHOP", NewProduct = "Headlights", NewDemand = 4
            },
            //Marbles
            new ReplaceInformation {
                Settlement = "Irmgardshausen", Shop = "Toy Store", NewProduct = "Marbles", NewDemand = 4
            },
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Hardware Store", NewProduct = "Marbles", NewDemand = 4
            },
            //CannedFish
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Grocery Store", NewProduct = "CannedFish", NewDemand = 2
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Grocery Store", NewProduct = "CannedFish", NewDemand = 2
            },
            //Telephone
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Home Goods", NewProduct = "Telephone", NewDemand = 2
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Home Goods", NewProduct = "Telephone", NewDemand = 2
            },
            //Toy Train Set
            new ReplaceInformation {
                Settlement = "Magdalenenhütte", Shop = "Toy Store", NewProduct = "Toy Train Set", NewDemand = 4
            },
            new ReplaceInformation {
                Settlement = "Leutingen", Shop = "Toy Store", NewProduct = "Toy Train Set", NewDemand = 4
            },
            //Beer
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Liquor Store", NewProduct = "Beer", NewDemand = 4
            },
            //Burgers
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "Diner", NewProduct = "Burgers", NewDemand = 4
            },
            //CombustionEngine
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "PARTS SHOP", NewProduct = "CombustionEngine", NewDemand = 1
            },
            //InteriorBody
            new ReplaceInformation {
                Settlement = "Zuckerrost", Shop = "PARTS SHOP", NewProduct = "InteriorBody", NewDemand = 1
            }
        };

        static void Postfix(Shop __instance) {
            var _demand = Reflection.GetField<Dictionary<ProductDefinition, int>>(typeof(Shop), "_demand", __instance);
            foreach (var product in __instance.sold) {
                if (GetNewDemand(__instance, product, out var newDemand))
                    _demand[product] = newDemand;
            }
        }

        private static bool GetNewDemand(Shop s, ProductDefinition product, out int newDemand) {
            if (!ReplaceCache.TryGetValue(s, out var demandDict)) {
                demandDict = new Dictionary<ProductDefinition, int>();
                ReplaceCache.Add(s, demandDict);
            }

            if (demandDict.TryGetValue(product, out newDemand)) {
                return true;
            } else if (FindNewDemand(s, product, out newDemand)) {
                demandDict.Add(product, newDemand);
                return true;
            }

            newDemand = 0;
            return false;
        }

        private static bool FindNewDemand(Shop s, ProductDefinition product, out int newDemand) {
            foreach (var ri in ReplaceInfo) {
                if (ri.Settlement != s.settlement.settlementName || ri.Shop != s.name)
                    continue;

                if (product == ri.NewProductDef)
                     newDemand = ri.NewDemand;
            }

            newDemand = 0;
            return false;
        }
    }
}

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
            foreach (var item in __instance.sold) {
                foreach (var ri in ReplaceInfo) {
                    if (ri.Settlement != __instance.settlement.settlementName || ri.Shop != __instance.name)
                        continue;

                    if (item == ri.NewProductDef)
                        _demand[item] = ri.NewDemand;
                }
            }
        }
    }
}

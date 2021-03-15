using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;
using UnityEngine;

namespace ROIData {
    public class CustomEventManager {

        public static void CreateEvent() {
            StaticWorldEvent swe = new StaticWorldEvent();
            WorldEventEffect wee = new WorldEventEffect();
            StaticWorldEventData wedb = ScriptableObject.CreateInstance<StaticWorldEventData>();
            WorldEventEffectData weed = new WorldEventEffectData();
            WorldEventManager wem = ManagerBehaviour<WorldEventManager>.instance;

            var listOfProducts = new List<ProductDefinition>();
            var listOfBuildings = new List<Building>();
            var listOfVehicles = new List<Vehicle>();
            var listOfRecipes = new List<Recipe>();
            var listOfDataCategories = new List<DataCategory>();
            var listOfBuildingTags = new List<BuildingTag> {
                GameData.instance.GetAsset<BuildingTag>("Factory")
            };
            var listOfProductTags = new List<ProductTag>();

            IList<Building> WEEbuildings = new List<Building>();
            IList<ProductDefinition> WEEproducts = new List<ProductDefinition>();
            IList<Recipe> WEErecipes = new List<Recipe>();
            IList<Vehicle> WEEvehicles = new List<Vehicle>();

            WorldEventBuildingFilter _buildingFilter = new WorldEventBuildingFilter {
                amount = WorldEventEffectFilterAmount.All,
                exceptBuildings = listOfBuildings.ToArray(),
                fromAll = false,
                fromBuildings = listOfBuildings.ToArray(),
                fromTags = listOfBuildingTags.ToArray()
            };
            Debug.Log("WorldEventBuildingFilter angelegt");

            WorldEventProductFilter _productFilter = new WorldEventProductFilter {
                amount = WorldEventEffectFilterAmount.All,
                categoryRestrictions = listOfDataCategories.ToArray(),
                fromAll = true,
                fromProducts = listOfProducts.ToArray(),
                fromTags = listOfProductTags.ToArray(),
                producers = listOfBuildings.ToArray()
            };
            Debug.Log("WorldEventProductFilter angelegt");

            WorldEventRecipeFilter _recipeFilter = new WorldEventRecipeFilter {
                amount = WorldEventEffectFilterAmount.All,
                fromAll = false,
                fromRecipes = listOfRecipes.ToArray(),
                resultProducts = listOfProducts.ToArray(),
                resultProductsTags = listOfProductTags.ToArray()
            };
            Debug.Log("WorldEventRecipeFilter angelegt");

            WorldEventVehicleFilter _vehicleFilter = new WorldEventVehicleFilter {
                amount = WorldEventEffectFilterAmount.All,
                fromAll = false,
                fromVehicles = listOfVehicles.ToArray()
            };
            Debug.Log("WorldEventVehicleFilter angelegt");

            WorldEventEffectData _weed = new WorldEventEffectData {
                buildingFilter = _buildingFilter,
                isCritical = false,
                modifier = -50,
                moneyAmount = 0,
                networkFilter = new WorldEventNetworkFilter(),
                productFilter = _productFilter,
                range = 0,
                recipeFilter = _recipeFilter,
                type = WorldEventEffectType.Demand,
                vehicleFilter = _vehicleFilter,
                whenToApply = WorldEventEffectApplyOption.LongTermModifier,
                whereFilter = WorldEventEffectWhereFilter.Region
            };
            Debug.Log("WorldEventEffectData angelegt");
            
            WorldEventEffect _wee = new WorldEventEffect {
                buildings = WEEbuildings,
                data = _weed,
                modifierMultiplier = 1,
                network = string.Empty,
                products = WEEproducts,
                recipes = WEErecipes,
                region = ROIDataMod.Player.hq.region,
                vehicles = WEEvehicles
            };

            //Beginn neuer Stuff
            ProductDefinition[] productDefinitions = new ProductDefinition[] { 
                GameData.instance.GetAssetsRO(typeof(ProductDefinition)).FirstOrDefault(p => p.name == "Potato") as ProductDefinition 
            };

            List<WorldEventEffectBuildSettings> weebs = new List<WorldEventEffectBuildSettings> {
                new WorldEventEffectBuildSettings {
                    buildings = WEEbuildings,
                    products = productDefinitions,
                    recipes = WEErecipes,
                    vehicles = WEEvehicles
                }
            };
            Debug.Log("WorldEventEffectBuildSettings angelegt");

            //WorldEventCreationParams
            WorldEventCreationParams wecp = new WorldEventCreationParams {
                region = ROIDataMod.Player.hq.region,
                permanent = true,
                forceEvent = true,
                additionalEffects = new WorldEventEffectData[] { _weed },
                difficulty = 1,
                duration = 720,
                durationMult = 1f,
                modifierMult = 1f,
                settings = weebs.ToArray()
            };
            Debug.Log("WorldEventCreationParams angelegt");

            //WorldEventDataBase
            if (wedb == null) {
                Debug.Log("WorldEventDataBase ist null");
            } else {
                wedb.eventName = "Massiver Nachfrageverlust";
                wedb.description = "50 Prozent weniger Nachfrage für alle Produkte.";
                wedb.when = -1;
                wedb.triggerMode = WorldEventTriggerMode.MANUAL;
                wedb.catalystBuildings = listOfBuildings;
                wedb.easy = wedb.normal = wedb.hard = new WorldEventDataDifficulty {
                    durationMax = 720,
                    durationMin = 0,
                    effects = new List<WorldEventEffectData> { _weed }
                };

                Debug.Log("WorldEventDataBase::After");
            }

            var filterCache = typeof(WorldEventCreator)
                .GetField("_filterCache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .GetValue(null) as WorldEventFilterCache;
            Debug.Log("WorldEventFilterCache geholt");

            List<StaticWorldEventData> sweds = new List<StaticWorldEventData> { wedb };
            filterCache.Initialize(sweds, Array.Empty<DynamicWorldEventData>(), 1);
            Debug.Log("filterCache initialisiert");

            //WorldEventManager
            if (wem == null) {
                Debug.Log("WorldEventManager ist null");
            } else {
                //wem.TriggerStaticEvent(wedb, ROIDataMod.Player.worldEvents);
                //wecp geaddet für neuen Stuff
                if (wecp == null) {
                    Debug.Log("WorldEventCreationParams ist null");
                } else {
                    Debug.Log("Player has all needed catalysts: " + WorldEventCatalystsChecker.PlayerHasAllNeededCatalysts(wedb));
                }

                wem.TriggerStaticEvent(wedb, ROIDataMod.Player.worldEvents, wecp);
                //ProjectAutomata.WorldEventCreator.CreateStaticEvent (ProjectAutomata.StaticWorldEventData data, ProjectAutomata.IActor actor, ProjectAutomata.WorldEventCreationParams parameters)
                //Vermutung: StaticWorldEvent staticWorldEvent = _eventPool.Get(); -> Event existiert nicht und kann daher nicht gefunden werden.

                Debug.Log("Event wurde getriggert.");
            }
        }
    }
}

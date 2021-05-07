using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;
using ROIData.CustomData;
using ROIData.CustomFilters;
using UnityEngine;

namespace ROIData {
    public class CustomStaticEvent {
        private StaticWorldEventData StaticWorldEventData;
        private WorldEventCreationParams WorldEventCreationParams;

        public string Name => StaticWorldEventData?.eventName ?? "N/A";
        public bool OneTimeEvent { get; private set; }

        public CustomStaticEvent WithStaticWorldEventData(StaticWorldEventData staticWorldEventData)
        {
            StaticWorldEventData = staticWorldEventData;
            return this;
        }

        public CustomStaticEvent WithWorldCreationParams(WorldEventCreationParams worldEventCreationParams)
        {
            WorldEventCreationParams = worldEventCreationParams;
            return this;
        }

        public CustomStaticEvent IsOneTimeEvent() {
            OneTimeEvent = true;
            return this;
        }

        public bool TryTrigger()
        {
            WorldEventManager wem = ManagerBehaviour<WorldEventManager>.instance;

            var filterCache = typeof(WorldEventCreator)
                .GetField("_filterCache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .GetValue(null) as WorldEventFilterCache;

            List<StaticWorldEventData> sweds = new List<StaticWorldEventData> { StaticWorldEventData };
            filterCache.Initialize(sweds, Array.Empty<DynamicWorldEventData>(), 1);

            return wem != null && wem.TriggerStaticEvent(StaticWorldEventData, ROIDataMod.Player.worldEvents, WorldEventCreationParams);
        }

        //TODO: Liste an laufenden Events
        //TODO: TryEnd

        public static CustomStaticEvent CreateResearchSpeedBoostEvent()
        {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        //.SetMinDuration(0) //min und max sind standardmäßig 30 tage (basically useless)
                        //.SetMaxDuration(10)
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.ResearchSpeed, false, 100, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .Build())
                        .Build())
                    .WithName("ResearchBoostEvent")
                    .WithDescription("Small research speed boost!")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(2) //in february?
                    .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .SetDifficulty(1)
                    //.SetDuration(60)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetDurationMultiplier(1)
                    .SetEffectMultiplier(1)
                    .Build()
                ); ;
        }
        //Event: ResearchSpeed
        //Event: Pollution (einer der werte)
        //Event: Strafe für zu viel pollution
        //Event: Grant & Fine für Zahlungen und Belohnungen
        //Event: Upkeep
        //Event: NetworkSpeed (bleibt bis ende erhalten), DispatchCost
        //Event: Demand (Negativ), BuildingCost (Increase)
        //Event: Demand (Positiv, außer auf Tiernahrung)

        public static CustomStaticEvent CreateDecreasedDemandAndIncreasedBuildingCostsEvent() {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .SetMinDuration(0)
                        .SetMaxDuration(10)
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.Demand, false, -50, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.OneTime)
                            .AddProductFilter(
                                new CustomProductFilter(WorldEventEffectFilterAmount.All, true)
                                .Build())
                            .AddBuildingFilter(
                                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, true)
                                .Build())
                            .Build())
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.BuildingCost, false, 50, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.OneTime)
                            .AddBuildingFilter(
                                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, true)
                                .Build())
                            .Build())
                        .Build())
                    .WithName("DecreasedDemandAndIncreasedBuildingCostsEvent")
                    .WithDescription("Decreased demand and increased building costs!")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .SetDifficulty(1)
                    .SetDuration(1)
                    .SetDurationMultiplier(1)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreateNetworkSpeedAndDispatchCostEvent()
        {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .SetMinDuration(0)
                        .SetMaxDuration(10)
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.NetworkSpeed, false, 50, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.OneTime)
                            .Build())
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.DispatchCost, false, 50, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.OneTime)
                            .Build())
                        .Build())
                    .WithName("NetworkSpeedAndDispatchCostEvent")
                    .WithDescription("Increased Network Speed and Dispatch Costs!")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                    .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .SetDifficulty(1)
                    .SetDuration(1)
                    .SetDurationMultiplier(1)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }


        public static CustomStaticEvent CreateIncreasedPollutionRateEvent() {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .SetMinDuration(0)
                        .SetMaxDuration(10)
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.PollutionRate, false, 50, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.OneTime)
                            .AddBuildingFilter(
                                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, true)
                                .Build())
                            .Build())
                        .Build())
                    .WithName("PollutionEvent")
                    .WithDescription("Increased pollution rate!")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                    .Build())
                .WithWorldCreationParams(
                    //permanent and forceEvent
                    new CustomCreationParams(true, true)
                    .SetDifficulty(1)
                    .SetDuration(1)
                    .SetDurationMultiplier(1)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }
        public static CustomStaticEvent CreatePollutionFineEvent() {
            return new CustomStaticEvent()
                .IsOneTimeEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        //.SetMinDuration(0)
                        //.SetMaxDuration(10)
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.Fine, true, 1, 20000000, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.OneTime)
                            .Build())
                        .Build())
                    .WithName("PollutionFineEvent")
                    .WithDescription("Punished for pollution!")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(1) //-1
                    .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(false, true)
                    .SetDifficulty(1)
                    .SetDuration(10)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetDurationMultiplier(1)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static void CreateEvent() {
            StaticWorldEvent swe = new StaticWorldEvent();
            WorldEventEffect wee = new WorldEventEffect();
            //StaticWorldEventData staticWorldEventData = ScriptableObject.CreateInstance<StaticWorldEventData>();
            WorldEventEffectData weed = new WorldEventEffectData();
            

            //var listOfProducts = new List<ProductDefinition>();
            //var listOfBuildings = new List<Building>();
            //var listOfVehicles = new List<Vehicle>();
            //var listOfRecipes = new List<Recipe>();
            //var listOfDataCategories = new List<DataCategory>();
            var listOfBuildingTags = new List<BuildingTag> {
                GameData.instance.GetAsset<BuildingTag>("Factory")
            };
            //var listOfProductTags = new List<ProductTag>();

            //IList<Building> WEEbuildings = new List<Building>();
            //IList<ProductDefinition> WEEproducts = new List<ProductDefinition>();
            //IList<Recipe> WEErecipes = new List<Recipe>();
            //IList<Vehicle> WEEvehicles = new List<Vehicle>();

            var buildingFilter = new CustomBuildingFilter(WorldEventEffectFilterAmount.All, false)
                .Build();

            var productFilter = new CustomProductFilter(WorldEventEffectFilterAmount.All, true)
                .Build();

            var recipeFilter = new CustomRecipeFilter(WorldEventEffectFilterAmount.All, false)
                .Build();

            var vehicleFilter = new CustomVehicleFilter(WorldEventEffectFilterAmount.All, false)
                .Build();

            var worldEventEffectData = new CustomEffectData(WorldEventEffectType.Demand, false, -50, 0, 0)
                .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                .AddWhereFilter(WorldEventEffectWhereFilter.Region)
                .AddBuildingFilter(buildingFilter)
                .AddProductFilter(productFilter)
                .AddRecipeFilter(recipeFilter)
                .AddVehicleFilter(vehicleFilter)
                .Build();

            ProductDefinition[] products = new ProductDefinition[] { 
                GameData.instance.GetAssetsRO(typeof(ProductDefinition)).FirstOrDefault(p => p.name == "Potato") as ProductDefinition
            };

            var worldEventEffectBuildSetting = new CustomBuildSettings()
                .WithProducts(products)
                .Build();

            var worldEventCreationParameters = new CustomCreationParams(true, true)
                .SetDifficulty(1)
                .SetDuration(720)
                .SetDurationMultiplier(1f)
                .SetEffectMultiplier(1f)
                .SetRegion(ROIDataMod.Player.hq.region)
                .WithBuildSetting(worldEventEffectBuildSetting)
                .WithEffectData(worldEventEffectData)
                .Build();

            var worldEventDataDifficulty = new CustomDataDifficulty()
                .SetMinDuration(0)
                .SetMaxDuration(720)
                .WithEffect(worldEventEffectData)
                .Build();

            var staticWorldEventData = new CustomStaticEventData(worldEventDataDifficulty)
                .WithName("Massiver Nachfrageverlust")
                .WithDescription("50 Prozent weniger Nachfrage für alle Produkte.")
                .WithWhen(-1)
                .WithTrigger(WorldEventTriggerMode.MANUAL)
                .Build();

            
        }
    }
}

//WorldEventDataBase
//if (staticWorldEventData == null) {
//    Debug.Log("WorldEventDataBase ist null");
//} else {
//    staticWorldEventData.eventName = "Massiver Nachfrageverlust";
//    staticWorldEventData.description = "50 Prozent weniger Nachfrage für alle Produkte.";
//    staticWorldEventData.when = -1;
//    staticWorldEventData.triggerMode = WorldEventTriggerMode.MANUAL;
//    staticWorldEventData.catalystBuildings = listOfBuildings;
//    staticWorldEventData.easy = staticWorldEventData.normal = staticWorldEventData.hard = worldEventDataDifficulty;

//    Debug.Log("WorldEventDataBase::After");
//}

//List<WorldEventEffectBuildSettings> weebs = new List<WorldEventEffectBuildSettings> {
//                new WorldEventEffectBuildSettings {
//                    buildings = WEEbuildings,
//                    products = productDefinitions,
//                    recipes = WEErecipes,
//                    vehicles = WEEvehicles
//                }
//            };
//Debug.Log("WorldEventEffectBuildSettings angelegt");

//WorldEventCreationParams
//WorldEventCreationParams wecp = new WorldEventCreationParams {
//    region = ROIDataMod.Player.hq.region,
//    permanent = true,
//    forceEvent = true,
//    additionalEffects = new WorldEventEffectData[] { _weed },
//    difficulty = 1,
//    duration = 720,
//    durationMult = 1f,
//    modifierMult = 1f,
//    settings = weebs.ToArray()
//};
//Debug.Log("WorldEventCreationParams angelegt");

//WorldEventEffectData _weed = new WorldEventEffectData {
//    buildingFilter = _buildingFilter,
//    isCritical = false,
//    modifier = -50,
//    moneyAmount = 0,
//    networkFilter = new WorldEventNetworkFilter(),
//    productFilter = _productFilter,
//    range = 0,
//    recipeFilter = _recipeFilter,
//    type = WorldEventEffectType.Demand,
//    vehicleFilter = _vehicleFilter,
//    whenToApply = WorldEventEffectApplyOption.LongTermModifier,
//    whereFilter = WorldEventEffectWhereFilter.Region
//};
//Debug.Log("WorldEventEffectData angelegt");

//WorldEventEffect _wee = new WorldEventEffect
//{
//    buildings = WEEbuildings,
//    data = _weed,
//    modifierMultiplier = 1,
//    network = string.Empty,
//    products = WEEproducts,
//    recipes = WEErecipes,
//    region = ROIDataMod.Player.hq.region,
//    vehicles = WEEvehicles
//};

//Idee: Initialisierung der Filter auslagern.
//WorldEventBuildingFilter _buildingFilter = new WorldEventBuildingFilter {
//    amount = WorldEventEffectFilterAmount.All,
//    exceptBuildings = listOfBuildings.ToArray(),
//    fromAll = false,
//    fromBuildings = listOfBuildings.ToArray(),
//    fromTags = listOfBuildingTags.ToArray()
//};
//Debug.Log("WorldEventBuildingFilter angelegt");

//WorldEventProductFilter _productFilter = new WorldEventProductFilter {
//    amount = WorldEventEffectFilterAmount.All,
//    categoryRestrictions = listOfDataCategories.ToArray(),
//    fromAll = true,
//    fromProducts = listOfProducts.ToArray(),
//    fromTags = listOfProductTags.ToArray(),
//    producers = listOfBuildings.ToArray()
//};
//Debug.Log("WorldEventProductFilter angelegt");

//WorldEventRecipeFilter _recipeFilter = new WorldEventRecipeFilter {
//    amount = WorldEventEffectFilterAmount.All,
//    fromAll = false,
//    fromRecipes = listOfRecipes.ToArray(),
//    resultProducts = listOfProducts.ToArray(),
//    resultProductsTags = listOfProductTags.ToArray()
//};
//Debug.Log("WorldEventRecipeFilter angelegt");

//WorldEventVehicleFilter _vehicleFilter = new WorldEventVehicleFilter {
//    amount = WorldEventEffectFilterAmount.All,
//    fromAll = false,
//    fromVehicles = listOfVehicles.ToArray()
//};
//Debug.Log("WorldEventVehicleFilter angelegt");

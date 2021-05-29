using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;
using ROIData.CustomData;
using ROIData.CustomFilters;
using ROIData.EventParams;
using UnityEngine;

namespace ROIData {
    public class CustomStaticEvent {
        private StaticWorldEventData StaticWorldEventData;
        private WorldEventCreationParams WorldEventCreationParams;
        public string Name => StaticWorldEventData?.eventName ?? "N/A";
        public bool OneTimeEvent { get; private set; }

        public static IntProductEventParameters ProductPriceEventData
			= new IntProductEventParameters("100,Marbles");
        public static IntProductEventParameters ProductDemandEventData
			= new IntProductEventParameters("100,OrangeSoda,Headlights,CannedFish,Telephone,Burgers");
        //new
        public static IntProductEventParameters ProductDemandEventData33
			= new IntProductEventParameters("33,Marbles,Beer");
        public static IntProductEventParameters ProductDemandEventData300
			= new IntProductEventParameters("300,Toy Train Set");

        public static int ResearchSpeedBoost = 100;

        public static WorldEventEffectData ResearchBoostEvent
			= new CustomEffectData(WorldEventEffectType.ResearchSpeed, false, ResearchSpeedBoost, 0, 0)
            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
            .Build();

        public static WorldEventEffectData ProductPriceEvent
			= new CustomEffectData(WorldEventEffectType.ProductPrice, false, ProductPriceEventData.Modifier, 0, 0)
            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
            .AddProductFilter(
                new CustomProductFilter(WorldEventEffectFilterAmount.All, false)
                .WithProducts(ProductPriceEventData.Products)
                .Build())
            .AddBuildingFilter(
                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, false)
                .Build())
            .Build();

        public static WorldEventEffectData ProductDemandEvent
			= new CustomEffectData(WorldEventEffectType.Demand, false, ProductDemandEventData.Modifier, 0, 0)
            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
            .AddProductFilter(
                new CustomProductFilter(WorldEventEffectFilterAmount.All, false)
                .WithProducts(ProductDemandEventData.Products)
                .Build())
            .AddBuildingFilter(
                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, false)
                .Build())
            .Build();

        //new
        public static WorldEventEffectData ProductDemandEvent33
			= new CustomEffectData(WorldEventEffectType.Demand, false, ProductDemandEventData33.Modifier, 0, 0)
            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
            .AddProductFilter(
                new CustomProductFilter(WorldEventEffectFilterAmount.All, false)
                .WithProducts(ProductDemandEventData33.Products)
                .Build())
            .AddBuildingFilter(
                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, false)
                .Build())
            .Build();

        //new
        public static WorldEventEffectData ProductDemandEvent300
			= new CustomEffectData(WorldEventEffectType.Demand, false, ProductDemandEventData300.Modifier, 0, 0)
            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
            .AddProductFilter(
                new CustomProductFilter(WorldEventEffectFilterAmount.All, false)
                .WithProducts(ProductDemandEventData300.Products)
                .Build())
            .AddBuildingFilter(
                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, false)
                .Build())
            .Build();

        public static List<WorldEventEffectData> WorldEventEffects = new List<WorldEventEffectData> {
            ResearchBoostEvent /*ProductPriceEvent, ProductDemandEvent, ProductDemandEvent33, ProductDemandEvent300,*/
			/*MakeEffectForProduct(7, "OrangeSoda"), MakeEffectForProduct(-9, "Headlights"), MakeEffectForProduct(9, "Marbles"),
			MakeEffectForProduct(85, "Telephone"), MakeEffectForProduct(87, "CannedFish"), MakeEffectForProduct(25, "Toy Train Set"),
			MakeEffectForProduct(-6, "Beer"), MakeEffectForProduct(-6, "Burgers"), MakeEffectForProduct(-20, "InteriorBody")*/
        };

		private static WorldEventEffectData MakeEffectForProduct(int modifier, string product) {
			var productInfo = new ProductDefinition[]{ GameData.instance.GetAssetsRO(typeof(ProductDefinition))
					.FirstOrDefault(p => p.name == product) as ProductDefinition };

			return new CustomEffectData(WorldEventEffectType.ProductPrice, false, modifier, 0, 0)
				.SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
				.AddProductFilter(
					new CustomProductFilter(WorldEventEffectFilterAmount.All, false)
					.WithProducts(productInfo)
					.Build())
				.AddBuildingFilter(
					new CustomBuildingFilter(WorldEventEffectFilterAmount.All, false)
					.Build())
				.Build();
	}

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

        public static CustomStaticEvent CreateLongTermEvent() {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffects(
                            WorldEventEffects)
                        .Build())
                    .WithName("Einstellungen")
                    .WithDescription("Für die Gesamtdauer der Aufgaben ist die Forschungsgeschwindigkeit erhöht. " +
                    "Weiterhin sind Nachfrage und Preise für einige Produkte angepasst.")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                    .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .SetDifficulty(1)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreateMessageEvent(MessageEventParameters parameters)
        {
            return new CustomStaticEvent()
                .IsOneTimeEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .Build())
                    .WithName(parameters.Title)
                    .WithDescription(parameters.Description)
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                    .IsGood()
                    .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(false, true)
                    .SetDifficulty(1)
                    .SetDuration(60)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreateResearchSpeedEvent(int modifier)
        {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.ResearchSpeed, false, modifier, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .Build())
                        .Build())
                    .WithName("Veränderte Forschungsgeschwindigkeit")
                    .WithDescription(modifier >= 0 ? "Forschungsgeschwindigkeit erhöht." : "Forschungsgeschwindigkeit verringert.")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                    .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .SetDifficulty(1)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreateBuildingCostEvent(int modifier)
        {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.BuildingCost, false, modifier, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .AddBuildingFilter(
                                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, true)
                                .Build())
                            .Build())
                        .Build())
                    .WithName("Veränderte Gebäudekosten")
                    .WithDescription(modifier >= 0 ? "Gebäudekosten erhöht." : "Gebäudekosten verringert.")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .SetDifficulty(1)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreateUpkeepEvent(int modifier)
        {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.Upkeep, false, modifier, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .AddBuildingFilter(
                                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, true)
                                .Build())
                            .Build())
                        .Build())
                    .WithName("Veränderte Instandhaltungskosten")
                    .WithDescription(modifier >= 0 ? "Instandhaltungskosten erhöht." : "Instandhaltungskosten verringert.")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                    .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .SetDifficulty(1)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreatePollutionFineEvent(int amount) {
            return new CustomStaticEvent()
                .IsOneTimeEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.Fine, true, 1, amount, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.OneTime)
                            .Build())
                        .Build())
                    .WithName("[Strafe] Umweltverschmutzung")
                    .WithDescription("Strafe aufgrund erhöhter Emissionswerte.")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1) //1
                    .IsBad()
                    .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(false, true)
                    .SetDifficulty(1)
                    .SetDuration(30)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetDurationMultiplier(1)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreateFineEvent(IntStringEventParameters parameters)
        {
            return new CustomStaticEvent()
                .IsOneTimeEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.Fine, true, 1, parameters.Amount, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.OneTime)
                            .Build())
                        .Build())
                    .WithName("[Strafe] Personalisierte Strafe")
                    .WithDescription(parameters.Message)
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1) //1
                    .IsBad()
                    .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(false, true)
                    .SetDifficulty(1)
                    .SetDuration(30)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetDurationMultiplier(1)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreateNetworkSpeedEvent(int modifier)
        {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.NetworkSpeed, false, modifier, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .AddNetworkFilter(
                                new CustomNetworkFilter(HelperClasses.CustomNetworkType.Road)
                                .Build())
                            .Build())
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.NetworkSpeed, false, modifier, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .AddNetworkFilter(
                                new CustomNetworkFilter(HelperClasses.CustomNetworkType.Rail)
                                .Build())
                            .Build())
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.NetworkSpeed, false, modifier, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .AddNetworkFilter(
                                new CustomNetworkFilter(HelperClasses.CustomNetworkType.Water)
                                .Build())
                            .Build())
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.NetworkSpeed, false, modifier, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .AddNetworkFilter(
                                new CustomNetworkFilter(HelperClasses.CustomNetworkType.Air)
                                .Build())
                            .Build())
                        .Build())
                    .WithName("Veränderte Fahrzeuggeschwindigkeit")
                    .WithDescription(modifier >= 0 ? "Fahrzeuggeschwindigkeit erhöht." : "Fahrzeuggeschwindigkeit verringert")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .SetDifficulty(1)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        //TODO: check if only Water dispatch costs are affected. Second filter might overwrite first.
        public static CustomStaticEvent CreateDispactCostEvent(int modifier)
        {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.DispatchCost, false, modifier, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .AddNetworkFilter(
                                new CustomNetworkFilter(HelperClasses.CustomNetworkType.Rail)
                                .Build())
                            .AddNetworkFilter(
                                new CustomNetworkFilter(HelperClasses.CustomNetworkType.Water)
                                .Build())
                            .Build())
                        .Build())
                    .WithName("Veränderte Fahrtkosten")
                    .WithDescription(modifier >= 0? "Fahrtkosten erhöht." : "Fahrtkosten verringert.")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .SetDifficulty(1)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreateGrantEvent(IntStringEventParameters parameters)
        {
            return new CustomStaticEvent()
                .IsOneTimeEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.Grant, true, 1, parameters.Amount, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.OneTime)
                            .Build())
                        .Build())
                    .WithName("[Belohnung] Finanzieller Zuschuss")
                    .WithDescription(parameters.Message)
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                    .IsBad()
                    .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(false, true)
                    .SetDifficulty(1)
                    .SetDuration(30)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetDurationMultiplier(1)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreateProductPriceEvent(IntProductEventParameters parameters) {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.ProductPrice, false, parameters.Modifier, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .AddProductFilter(
                                new CustomProductFilter(WorldEventEffectFilterAmount.All, false)
                                .WithProducts(parameters.Products)
                                .Build())
                            .AddBuildingFilter(
                                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, false)
                                .Build())
                            .Build())
                        .Build())
                    .WithName("Veränderter Preis")
                    .WithDescription(parameters.Modifier >= 0 ? "Preis erhöht." : "Preis verringert.")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .WithBuildSetting(
                        new CustomBuildSettings()
                        .WithProducts(parameters.Products)
                        .Build())
                    .SetDifficulty(1)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static CustomStaticEvent CreateDemandEvent(IntProductEventParameters parameters) {
            return new CustomStaticEvent()
                .WithStaticWorldEventData(
                    new CustomStaticEventData(
                        new CustomDataDifficulty()
                        .WithEffect(
                            new CustomEffectData(WorldEventEffectType.Demand, false, parameters.Modifier, 0, 0)
                            .SetApplyOption(WorldEventEffectApplyOption.LongTermModifier)
                            .AddProductFilter(
                                new CustomProductFilter(WorldEventEffectFilterAmount.All, false)
                                .WithProducts(parameters.Products)
                                .Build())
                            .AddBuildingFilter(
                                new CustomBuildingFilter(WorldEventEffectFilterAmount.All, false)
                                .Build())
                            .Build())
                        .Build())
                    .WithName("Veränderte Nachfrage")
                    .WithDescription(parameters.Modifier >= 0 ? "Produktnachfrage erhöht." : "Produktnachfrage verringert.")
                    .WithTrigger(WorldEventTriggerMode.MANUAL)
                    .WithWhen(-1)
                .Build())
                .WithWorldCreationParams(
                    new CustomCreationParams(true, true)
                    .WithBuildSetting(
                        new CustomBuildSettings()
                        .WithProducts(parameters.Products)
                        .Build())
                    .SetDifficulty(1)
                    .SetRegion(ROIDataMod.Player.hq.region)
                    .SetEffectMultiplier(1)
                    .Build()
                );
        }

        public static void CreateEvent() {
            StaticWorldEvent swe = new StaticWorldEvent();
            WorldEventEffect wee = new WorldEventEffect();
            WorldEventEffectData weed = new WorldEventEffectData();

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

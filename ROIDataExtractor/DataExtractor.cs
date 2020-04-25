using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Serialization;
using SVGImporter;
using System.Globalization;

namespace ROIDataExtractor {
	public class DataExtractor : Mod {
		private bool saveReadyCalled = true;

		private GameDate lastUpdate = new GameDate(0, 0, 0);

		private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings {
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			//PreserveReferencesHandling = PreserveReferencesHandling.None,
			Formatting = Formatting.Indented,
			ContractResolver = new Excluder()
		};

		public void Update() {
			var actorManager = ManagerBehaviour<ActorManager>.instance;
			if (actorManager == null)
				return;

			//check if no actors (settlements, state, players, ...) are found
			if (actorManager.actors.count == 0)
				return;

			var moneyManager = ManagerBehaviour<MoneyManager>.instance;
			if (moneyManager == null)
				return;

			if (moneyManager.balances == null || moneyManager.balances.Count == 0)
				return;

			//_bills - MoneyAgent
			if ((typeof(MoneyAgent)
				.GetField("_bills", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(Player.money) as Dictionary<int, MoneyBillTimeTree>)?
				.Count == 0) {
				return;
			}

			//run once
			//if (!saveReadyCalled)
			//	SaveReady();

			//AllVehicles();

			//run every month
			var timeManager = ManagerBehaviour<TimeManager>.instance;

			if (timeManager.today.Day == 1 && !saveReadyCalled)
				SaveReady();
			else if (timeManager.today.Day == 2)
				saveReadyCalled = false;

		}

		private void SaveReady() {
			saveReadyCalled = true;

			//GetPlayerBalance();
			//WriteFormula();
			//WriteRecipes();
			//WriteShopData();
			//AllVehicles();

			StringBuilder sb = new StringBuilder();

			//sb.Append("Cleaner Ratio is: ").AppendLine(CleanerRatio())
			//	.Append("Company Value is: ").AppendLine(CompanyValue().ToString())
			//	.Append("Balance is: ").AppendLine(PlayerBalance().ToString())
			//	.Append("Loans: ").AppendLine(PlayerLoans())
			//	.Append("Shops:").AppendLine().AppendLine(ShopData())
			//	.Append("Production: ").AppendLine().AppendLine(ProductionData())
			//	//.Append("Found the following achievements: ").AppendLine().AppendLine(AchievementData())
			//	.Append("Pollution:").AppendLine().AppendLine(PollutionData());

			//foreach (var transport in Transports) {
			//	transport.ToStringSb(sb);
			//}
			//sb.Append(PlayerData());

			//Debug.Log(sb.ToString());
			//UpdateVehiclesMonthly();
		}

		private void UpdateVehiclesMonthly() {
			//var sb = new StringBuilder();
			//sb.AppendLine("Transports for the last month:");

			//Transports.ForEach(t => sb.AppendLine(t.ToString()));

			//sb.Append("Count: ").AppendLine(Transports.Count.ToString());
			//sb.Append("Average(Cost): ").AppendLine(Transports.Average(t => t.Cost).ToString());
			//sb.Append("Sum(Cost): ").AppendLine(Transports.Sum(t => t.Cost).ToString());
			//Debug.Log(sb.ToString());
			Transports.Clear();
		}

		private HumanPlayer Player => ManagerBehaviour<ActorManager>.instance.actors.FirstOrDefault(a => a is HumanPlayer) as HumanPlayer;

		private readonly Dictionary<GameDate, double> BalanceData = new Dictionary<GameDate, double>();

		private void GetPlayerBalance() {
			var moneyManager = ManagerBehaviour<MoneyManager>.instance;
			var balance = moneyManager.GetBalance(Player.money);

			BalanceData.Add(ManagerBehaviour<TimeManager>.instance.today, balance);
			File.WriteAllText(System.IO.Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RiseOfIndustry", "balanceData.json"),
						JsonConvert.SerializeObject(BalanceData, jsonSettings));
		}

		private void WriteFormula() {
			var settlementManager = ManagerBehaviour<SettlementManager>.instance;
			var settlements = settlementManager.settlements;

			foreach (var settlement in settlements) {
				foreach (var building in Player.buildings) {
					var upkeep = building.GetComponent<Upkeep>();

					if (upkeep != null) {
						//Debug.Log("Formula: " + JsonConvert.SerializeObject(upkeep.monthlyUpkeep, jsonSettings));
						//Building with gatherers: Costs with no
						Debug.Log("Upkeep of " + building.buildingName + " is " + upkeep.totalMonthlyUpkeep + " $/month.");
					}
				}
			}
		}

		private string CleanerRatio() {
			List<string> cleanerBuildings = new List<string>
			{
				"Air Purifier", "CHEMICAL SCRUBBER", "WATER TREATMENT PLANT"
			};

			List<string> ignoreBuildings = new List<string>
			{
				"Headquarters", "WATER SIPHON", "WATER PUMP", "CROP FARM", "POTATO FIELD", "LUMBERYARD", "LUMBERYARD HARVESTER",
				"SAND COLLECTOR", "SAND HARVESTER", "PLANTATION", "BERRY FIELD"
			};

			//Counters
			var cleanerCount = 0f;
			var notCleanerCount = 0f;

			foreach (var building in Player.buildings) {
				//ignore 
				if (ignoreBuildings.Contains(building.name)) {
					continue;
				}

				//A cleaner was found
				if (cleanerBuildings.Contains(building.name)) {
					cleanerCount++;
				} else {
					notCleanerCount++;
				}
			}

			if (notCleanerCount == 0) {
				return "Cleaner Ratio is 0";
			}

			var factor = cleanerCount / notCleanerCount;

			return $"Cleaner Ratio is: {factor}(Cleaners: {cleanerCount}; Buildings: {notCleanerCount})";
		}

		private int CompanyValue() {
			var m = Player.GetComponent<CompanyStats>();

			return m.ComputeCompanyValue();
		}

		private double PlayerBalance() {
			MoneyManager mm = ManagerBehaviour<MoneyManager>.instance;

			return mm.GetBalance(Player.money);
		}

		private String PlayerLoans() {
			LoansAgent la = Player.GetComponent<LoansAgent>();
			StringBuilder sb = new StringBuilder();

			sb.Append("Found ").Append(la.loans.Count).AppendLine(" loans.");

			foreach (var loan in la.loans) {
				sb.Append("Title: ").AppendLine(loan.title);
				sb.Append("Type: ").AppendLine(loan.type.ToString());
				sb.Append("Amount: ").AppendLine(loan.amount.ToString("C", CultureInfo.GetCultureInfo("en-US")));
				sb.Append("Amount to Pay: ").AppendLine(loan.amountToPay.ToString("C", CultureInfo.GetCultureInfo("en-US")));
				sb.Append("Amount with Apr: ").AppendLine(loan.amountWithApr.ToString("C", CultureInfo.GetCultureInfo("en-US")));
				sb.Append("Apr: ").AppendLine(loan.apr.ToString("P"));
				sb.Append("Duration: ").Append(loan.duration.ToString()).AppendLine(" days");
				sb.Append("Remaining Payments: ").AppendLine(loan.remainingPayments.ToString("N"));
				sb.AppendLine();
			}

			return sb.ToString();
		}

		private string ShopData() {
			SettlementManager settlementManager = ManagerBehaviour<SettlementManager>.instance;
			List<SettlementBase> settlements = settlementManager.settlements;

			StringBuilder sb = new StringBuilder();
			List<SaleStatistic> saleStatistics = new List<SaleStatistic>();

			foreach (var settlement in settlements) {
				foreach (var shop in settlement.buildings.shops) {
					Dictionary<ProductDefinition, int> demands = GetField<Dictionary<ProductDefinition, int>>(typeof(Shop), "_demand", shop);

					foreach (var kvp_demand in demands) {
						ProductDefinition product = kvp_demand.Key;
						int demand = kvp_demand.Value;
						int sales = shop.GetSoldCount(Player, product, new GamePeriod(0,0,29));

						saleStatistics.Add(new SaleStatistic(settlement, shop, product, demand, sales));
					}
				}
			}

			saleStatistics.Where(saleStatistic => saleStatistic.Sales != 0).ForEach(saleStatistic => saleStatistic.ToStringSb(sb));
			return sb.ToString();
		}

		private class SaleStatistic {

			public SettlementBase Settlement { get; }
			public Shop Shop { get; }
			public ProductDefinition Product { get; }
			public int Demand { get; }
			public int Sales { get; }

			public SaleStatistic(SettlementBase settlement, Shop shop, ProductDefinition product, int demand, int sales) {
				Settlement = settlement;
				Shop = shop;
				Product = product;
				Demand = demand;
				Sales = sales;
			}

			public StringBuilder ToStringSb(StringBuilder sb = null) {
				if (sb == null)
					sb = new StringBuilder();

				float ratio = Sales / ((float)Demand * 2);
				float offset = ratio - 1;

				sb.Append("Settlement: ").Append(Settlement.settlementName)
					.Append(", Shop: ").Append(Shop.buildingName)
					.Append(", Product: ").Append(Product.productName)
					.Append(", Demand (15d): ").Append(Demand.ToString())
					.Append(", Sales (30d): ").Append(Sales.ToString())
					.Append(", Satisfaction of Demand: ").Append(offset.ToString("P"))
					.AppendLine();

				return sb;
			}
		}

		public string ProductionData() {
			StringBuilder sb = new StringBuilder();
			List<FactoryStatistic> factoryStatistics = new List<FactoryStatistic>();
			

			//ModuleOwner and Module
			//Dictionary<ModuleOwner, List<ProjectAutomata.Module>> gatherHubHarvesterDictionary = new Dictionary<ModuleOwner, List<ProjectAutomata.Module>>();

			foreach (var recipeUser in Player.buildings.recipeUsers) {
				factoryStatistics.Add(new FactoryStatistic(recipeUser));
			}

			//foreach (var transport in Transports) {
			//	var fs = factoryStatistics.Find(f => transport.Vehicle.fleet.building.GetComponent<RecipeUser>() == f.RecipeUser);

			//	if (fs != null) {
			//		fs.Transports.Add(transport);
			//	}
			//}

			////ArgumentException: Could not convert ProjectAutomata.BuildingLogistics to ProjectAutomata.BuildingLogistics.
			//foreach (var transport in Transports) {
			//	BuildingBehaviour origin = GetField<BuildingLogistics>(typeof(TransportJob<LandTransportJob>), "_origin", transport.TransportJob);
			//	var fs = factoryStatistics.Find(f => f.RecipeUser == origin.GetComponent<RecipeUser>());

			//	if (fs != null) {
			//		fs.Transports.Add(transport);
			//	}
			//}

			////ArgumentException: Could not convert ProjectAutomata.BuildingLogistics to ProjectAutomata.BuildingLogistics.
			//foreach (var transport in Transports) {
			//	BuildingLogistics origin = GetField<BuildingLogistics>(typeof(TransportJob<LandTransportJob>), "_origin", transport.TransportJob);
			//	var fs = factoryStatistics.Find(f => f.RecipeUser == origin.GetComponent<RecipeUser>());

			//	if (fs != null) {
			//		fs.Transports.Add(transport);
			//	}
			//}

			factoryStatistics.ForEach(r => r.ToStringSb(sb));
			return sb.ToString();
		}

		public class FactoryStatistic {
			public RecipeUser RecipeUser { get; }
			public List<Transport> Transports { get; } = new List<Transport>();
			public FactoryStatistic(RecipeUser recipeUser) {
				RecipeUser = recipeUser;
			}

			public StringBuilder ToStringSb(StringBuilder sb = null) {
				if (sb == null)
					sb = new StringBuilder();

				Recipe currentRecipe = RecipeUser.currentRecipe;

				sb.Append("Factory: ").AppendLine(RecipeUser.buildingName);

				//check if recipeUser is ModuleOwner
				ModuleOwner owner = RecipeUser.GetComponent<ModuleOwner>();
				if (owner != null) {
					//print out number of modules
					List<ProjectAutomata.Module> modules = owner.modules;
					sb.Append("- Modules (").Append(modules.Count.ToString())
						.AppendLine(")");
					foreach (var module in modules) {
						sb.Append("-- Name: ").Append(module.buildingName)
							.Append(", Upkeep: ").AppendLine(module.ModuleUpkeep().ToString("C", CultureInfo.GetCultureInfo("en-US")));
					}
				}

				sb.Append("- Active: ").AppendLine(RecipeUser.isProducing.ToString())
					.AppendLine("- Currently Producing");

				int counter = 0;

				foreach (var product in currentRecipe.result) {
					counter++;
					sb.Append("-- Product ").AppendLine(counter.ToString())
						.Append("--- Name: ").AppendLine(product._definition.productName)
						.Append("--- Amount: ").AppendLine(product.amount.ToString());
				}

				Upkeep upkeep = RecipeUser.GetComponent<Upkeep>();
				BuildingEfficiency buildingEfficiency = RecipeUser.GetComponent<BuildingEfficiency>();
				BuildingAnalysis buildingAnalysis = RecipeUser.GetComponent<BuildingAnalysis>();

				sb.Append("- Production Time: ").AppendLine((currentRecipe.gameDays * RecipeUser.productionSpeed).ToString())
					.Append("- Building Efficiency: ").AppendLine(buildingEfficiency.efficiency.ToString("P"))
					.Append("- Upkeep: ").AppendLine(upkeep.totalMonthlyUpkeep.ToString("C", CultureInfo.GetCultureInfo("en-US")))
					.Append("- Uptime: ").AppendLine((buildingAnalysis.GetLastMonthValue(upkeep.uptimeAnalysisItemDef) / 100).ToString("P"));

				sb.AppendLine("- Production Statistics")
					.Append("-- Last month: ").AppendLine(RecipeUser.producedLastMonth.ToString())
					.Append("-- Total production: ").AppendLine(RecipeUser.totalProduced.ToString())
					.AppendLine("- Storage");

				foreach (var product in RecipeUser.productStorage) {
					sb.Append("-- Product: ").Append(product._definition.productName)
						.Append(" (").Append(product.amount)
						.AppendLine(" units)");
				}

				foreach (var transport in Transports) {
					transport.ToStringSb(sb);
				}

				sb.AppendLine();
				return sb;
			}
		}

		public string AchievementData() {
			AchievementManager achievementManager = AchievementManager.instance;
			List<Achievement> achievements = achievementManager.achievements;
			StringBuilder sb = new StringBuilder();

			sb.Append("Achievements Enabled: ").Append(achievementManager.achievementsEnabled)
				.Append(", Disabled with Mods: ").Append(achievementManager.disableWithMods)
				.AppendLine();

			//achievementManager.achievementsEnabled = true;
			//sb.Append("Achievements Enabled: ").Append(achievementManager.achievementsEnabled)
			//	.AppendLine();

			foreach (var achievement in achievements) {
				List<Achievement.AchievementStat> achievementStats = GetField<List<Achievement.AchievementStat>>(typeof(Achievement), "_achievementStats", achievement);
				
				//basic information
				sb.Append("Id: ").Append(achievement.achievementId)
					.Append(", Name: ").Append(achievement.name)
					.AppendLine("Statistics: ");

				//included statistics
				foreach (var statistic in achievementStats) {
					sb.Append(", Stat-Id: ").Append(statistic.stat.statId)
						.Append(", Stat-Name: ").Append(statistic.stat.name)
						.Append(", Stat-Value: ").Append(statistic.achievementValue)
						.Append(", Stat-Value-2: ").Append(statistic.stat.value)
						//AchievementManager statistics
						.Append(", AchievementManager.GetStat() [Float]: ").Append(achievementManager.GetStat(statistic.stat.name, StatType.Float, false))
						.Append(", AchievementManager.GetStat() [Int]: ").Append(achievementManager.GetStat(statistic.stat.name, StatType.Int, false))
						.Append(", AchievementManager.GetStat() [Average]: ").Append(achievementManager.GetStat(statistic.stat.name, StatType.Average, false))
						.AppendLine();
				}
				sb.AppendLine()
					.AppendLine();
			}
			return sb.ToString();
		}

		private string PollutionData() {
			StringBuilder sb = new StringBuilder();
			RegionManager regionManager = ManagerBehaviour<RegionManager>.instance;
			PermitManager permitManager = ManagerBehaviour<PermitManager>.instance;

			var regions = GetField<ICollection<Region>>(typeof(RegionManager), "_serializedRegions", regionManager);
			foreach (var region in regions) {
				if (permitManager.OwnsPermitForRegion(Player, region, null)) {
					sb.Append("- Region: ").AppendLine(region.regionName)
						.Append("-- Settlement: ").AppendLine(region.settlement.settlementName)
						.Append("-- Pollution: ").AppendLine(PollutionForRegion(region).ToString())
						.AppendLine();
				}
			}

			return sb.ToString();
		}

		private double PollutionForRegion(Region region) {
			double pollution = 0;
			PollutionManager pollutionManager = ManagerBehaviour<PollutionManager>.instance;

			foreach (var tile in region.tiles)
				pollution += pollutionManager.GetPollution(tile);

			return pollution / region.tiles.count;
		}

		private string GlobalMarketData() {
			GlobalMarket globalMarket = ManagerBehaviour<GlobalMarket>.instance;
			StringBuilder sb = new StringBuilder();
			Dictionary<ProductDefinition, ProductPricingInfo> globalMarketProducts = GetField<Dictionary<ProductDefinition, ProductPricingInfo>>(typeof(GlobalMarket), "_pricingInfoByProduct", globalMarket);

			foreach (var globalMarketProduct in globalMarketProducts) {
				ProductDefinition product = globalMarketProduct.Key;
				ProductPricingInfo pricingInfo = globalMarketProduct.Value;

				if (globalMarket.GetProductDemand(product, Player) > 0) {
					sb.Append("- Product: ").AppendLine(product.productName)
						.Append("-- Category: ").AppendLine(product.productCategory.categoryName)
						.Append("-- Value: ").AppendLine(pricingInfo.value.ToString("C", CultureInfo.GetCultureInfo("en-US")))
						.Append("-- Price: ").AppendLine(pricingInfo.price.ToString("C", CultureInfo.GetCultureInfo("en-US")))
						.Append("-- Modifier: ").AppendLine(pricingInfo.modifier.ToString("P"))
						.Append("-- Trend: ").AppendLine(pricingInfo.trend.ToString())
						.Append("-- Demand: ").AppendLine(globalMarket.GetProductDemand(product, Player).ToString("N"))
						.AppendLine();
				}
			}
			return sb.ToString();
		}

		private class ProductInformation {

			public ProductDefinition Product { get; }
			public float Price { get; }
			public int Demand { get; }
			public int Sales { get; }

			public ProductInformation(ProductDefinition product, float price, int demand, int sales) {
				Product = product;
				Price = price;
				Demand = demand;
				Sales = sales;
			}
		}
		private class Salepoint {

			public Shop Shop { get; }
			public List<ProductInformation> ShopData { get; }

			public Salepoint(Shop shop, List<ProductInformation> shopData) {
				Shop = shop;
				ShopData = shopData;
			}

			public StringBuilder ToStringSb(StringBuilder sb = null) {
				if (sb == null)
					sb = new StringBuilder();

				sb.Append("- Shop: ").AppendLine(Shop.buildingName);

				foreach (var productInformation in ShopData) {
					sb.Append("-- Product: ").AppendLine(productInformation.Product.productName)
						.Append("--- Price: ").AppendLine(productInformation.Price.ToString("C", CultureInfo.GetCultureInfo("en-US")))
						.Append("--- Demanded: ").AppendLine(productInformation.Demand.ToString("N"))
						.Append("--- Sold by Player: ").AppendLine(productInformation.Sales.ToString("N"));

					float ratio = productInformation.Sales / ((float)productInformation.Demand * 2);
					float offset = ratio - 1;

					sb.Append("--- Satisfaction of Demand: ").AppendLine(offset.ToString("P"));
				}

				sb.AppendLine();
				return sb;
			}
		}

		private String PlayerData() {
			StringBuilder sb = new StringBuilder();

			//Settlements
			sb.AppendLine("# # # # Settlements: # # # #")
				.AppendLine(SettlementData())
				.AppendLine();

			//Houses
			sb.AppendLine("# # # # Houses: # # # #")
				.AppendLine(HouseData())
				.AppendLine();

			//Polluted
			sb.AppendLine("# # # # Pollution: # # # #")
				.AppendLine("Polluted Buildings").AppendLine(PollutedBuildingsData())
				.AppendLine("Polluted Regions").AppendLine(PollutionData())
				.AppendLine();

			//Contracts
			sb.AppendLine("# # # # Contracts: # # # #")
				.AppendLine(ContractData())
				.AppendLine();

			//GlobalMarket
			sb.AppendLine("# # # # Global Market: # # # #")
				.AppendLine(GlobalMarketData())
				.AppendLine();

			//Shops
			sb.AppendLine("# # # # Shops: # # # #")
				.AppendLine(SalepointData())
				.AppendLine();

			//Factories
			sb.AppendLine("# # # # Factories: # # # #")
				.AppendLine(ProductionData())
				.AppendLine();

			return sb.ToString();
		}

		private string HouseData() {
			StringBuilder sb = new StringBuilder();
			SettlementBase settlement = Player.hq.settlement;
			SettlementBuildingCollection buildings = settlement.buildings;

			//Houses
			List<ResidentialBuilding> houses = buildings.houses;
			sb.Append("- ").Append(settlement.settlementName).Append(" has ")
				.Append(houses.Count).Append(" houses. ");

			int deathcounter = 0;
			foreach (var house in houses) {
				if (house.isDead) {
					deathcounter++;
				}
			}

			sb.Append(deathcounter).Append(" houses ")
				.AppendLine("are uninhabitable due to pollution.");

			return sb.ToString();
		}

		private string SalepointData() {
			StringBuilder sb = new StringBuilder();
			SettlementBuildingCollection buildings = Player.hq.settlement.buildings;

			//Shops
			List<Shop> shops = buildings.shops;
			List<Salepoint> salepoints = new List<Salepoint>();

			foreach (var shop in shops) {
				Dictionary<ProductDefinition, int> productDemands = GetField<Dictionary<ProductDefinition, int>>(typeof(Shop), "_demand", shop);
				List<ProductInformation> productInformations = new List<ProductInformation>();

				foreach (var productDemand in productDemands) {
					ProductDefinition product = productDemand.Key;
					float price = shop.GetPrice(product, Player);
					int demand = productDemand.Value;
					int sales = shop.GetSoldCount(Player, product, new GamePeriod(0, 0, 29));

					//add new product information
					productInformations.Add(new ProductInformation(product, price, demand, sales));
				}
				//add new shop
				salepoints.Add(new Salepoint(shop, productInformations));
			}

			foreach (var salepoint in salepoints) {
				salepoint.ToStringSb(sb);
			}

			return sb.ToString();
		}

		private string PollutedBuildingsData() {
			StringBuilder sb = new StringBuilder();
			SettlementBuildingCollection buildings = Player.hq.settlement.buildings;

			//Buildings affected by Pollution
			List<BuildingPollutionEffect> affectedByPollution = buildings.affectedByPollution;
			sb.Append("- ").Append(affectedByPollution.Count).AppendLine(" buildings are affected by pollution.");

			if (affectedByPollution.Count != 0) {
				foreach (var polluted in affectedByPollution) {
					sb.Append("-- Building: ").AppendLine(polluted.buildingName);
				}
			}

			return sb.ToString();
		}

		private string SettlementData() {
			StringBuilder sb = new StringBuilder();
			SettlementBase settlement = Player.hq.settlement;

			//Basics
			sb.Append("- Settlement: ").AppendLine(settlement.settlementName)
				.Append("-- Actor: ").AppendLine(settlement.actorName)
				.Append("-- Population: ").AppendLine(settlement.population.ToString("N"))
				.Append("-- Type: ").AppendLine(settlement.type.settlementTypeName);

			//Resources
			Region region = settlement.region;
			List<ResourceSite> resourceSites = region.resourceSites;

			sb.AppendLine("- Resources");

			foreach (var resourceSite in resourceSites) {
				sb.Append("-- Resource: ").Append(resourceSite.product.productName)
					.Append(" (").Append(resourceSite.resourceAmount)
					.AppendLine(" units)");
			}

			//Tier
			SettlementTier settlementTier = settlement.tier;
			sb.Append("- Tier: ").AppendLine(settlement.tier.tierName)
				.Append("-- Next Tier: ").AppendLine(settlement.tier.nextTier.tierName)
				.Append("-- Previous Tier: ").AppendLine(settlement.tier.previousTier.tierName);

			//Products
			sb.AppendLine("- Needed Products");
			foreach (var product in settlement.tier.neededProducts) {
				sb.Append("-- Product: ").Append(product.product.productName)
					.Append(" (from ").Append(product.amountRange.min)
					.Append(" to ").Append(product.amountRange.max)
					.AppendLine(" units)");
			}

			return sb.ToString();
		}

		private string ContractData() {
			StringBuilder sb = new StringBuilder();
			ReadOnlyList<Contract> contracts = Player.contracts.activeContracts;
			int counter = 0;

			foreach (var contract in contracts) {
				counter++;
				sb.Append("- Contract ").AppendLine(counter.ToString("N"))
					.Append("-- Issuer: ").Append(contract.issuer.actorName)
					.Append("-- Product: ").Append(contract.product.productName)
					.Append("-- Amount: ").Append(contract.amount)
					.Append("-- Target: ").Append(contract.target.buildingName)
					.AppendLine();
			}

			return sb.ToString();
		}

		private readonly List<Vehicle> DeliveringVehicles = new List<Vehicle>();
		private readonly List<Transport> Transports = new List<Transport>();

		public class Transport {
			//public Vehicle Vehicle { get; }
			//public LandTransportJob TransportJob { get; }
			//public WaypointMover Mover { get; }
			public BuildingLogistics Origin { get; }
			public BuildingLogistics Destination { get; }
			//NEW
			public Product Product { get; }
			public int Count { get; }
			public int Distance { get; }
			public float Costs { get; }

			//OLD
			//public Transport(Vehicle vehicle, LandTransportJob transportJob, WaypointMover mover, BuildingLogistics origin, BuildingLogistics destination) {
			//	Vehicle = vehicle;
			//	TransportJob = transportJob;
			//	Mover = mover;
			//	Origin = origin;
			//	Destination = destination;
			//}

			public Transport(BuildingLogistics origin, BuildingLogistics destination, Product product, int count, int distance, float costs) {
				Origin = origin;
				Destination = destination;
				Product = product;
				Count = count;
				Distance = distance;
				Costs = costs;
			}

			public StringBuilder ToStringSb(StringBuilder sb = null) {
				if (sb == null)
					sb = new StringBuilder();

				//TransportRequestPaymentHandlerBehaviour payer = Origin.building.GetComponent<TransportRequestPaymentHandlerBehaviour>();
				//NetworkManager networkManager = GetField<NetworkManager>(typeof(WaypointMover), "network", Mover);

				//Product product = Origin.building.recipeUser.currentOutput;
				//ReadOnlyList<int> originToDestinationPath = TransportJob.originToDestinationPath;

				//string originDestinationDistance = "Distance: " + Mover.distance + ", TilesMovedCount: " + Mover.tilesMovedCount + ", TilesInPath: " + Mover.tilesInPath + ", PathCount: " + originToDestinationPath.count;
				//int amount = 1;
				//var costs = payer.GetVehicleDispatchCost(Origin, Destination, Vehicle, Mover.distance, amount);

				//sb.AppendLine("--- Transport")
				//	.Append("---- Origin: ").AppendLine(Origin.buildingName)
				//	.Append("---- Destination: ").AppendLine(Destination.buildingName)
				//	.Append("---- Distance: ").AppendLine(originDestinationDistance)
				//	.Append("---- Product: ").AppendLine(product.definition.productName)
				//	.Append("---- Amount: ").AppendLine(amount.ToString())
				//	.Append("---- Costs: ").AppendLine(costs.ToString("C", CultureInfo.GetCultureInfo("en-US")));

				sb.AppendLine("--- Transport")
					.Append("Origin: ").AppendLine(Origin.buildingName)
					.Append("Destination: ").AppendLine(Destination.buildingName)
					.Append("Product: ").AppendLine(Product.definition.productName)
					.Append("Count: ").AppendLine(Count.ToString())
					.Append("Distance: ").AppendLine(Distance.ToString())
					.Append("Costs: ").AppendLine(Costs.ToString("C", CultureInfo.GetCultureInfo("en-US")))
					.AppendLine();

				return sb;
			}
		}

		private void CheckVehicleDelivery(Vehicle v) {
			//check for new vehicles // not tracked and save data
			if (!DeliveringVehicles.Contains(v) && IsVehicleDelivering(v)) {
				DeliveringVehicles.Add(v);
				AddTransport(v);
			}

			//remove vehicle if it's not delivering anymore
			if (DeliveringVehicles.Contains(v) && !IsVehicleDelivering(v))
				DeliveringVehicles.Remove(v);
		}

		private bool IsVehicleDelivering(Vehicle v) {
			if (v.activeJob == null)
				return false;

			if (v.activeJob is LandTransportJob ltj)
				return !ltj.IsGoingHome();

			return false;
		}

		private void AddTransport(Vehicle vehicle) {
			if (vehicle.activeJob == null) {
				Debug.Log("AddTransport::ActiveJob::Null");
				return;
			}

			if (vehicle.mover == null) {
				Debug.Log("AddTransport::Mover::Null");
				return;
			}

			if (vehicle.activeJob is LandTransportJob transportJob
				&& transportJob.TryGetField<BuildingLogistics>("_origin", out var origin)
				&& transportJob.TryGetField<BuildingLogistics>("_destination", out var destination)
				&& vehicle.mover is WaypointMover waypointMover) {

				TransportRequestPaymentHandlerBehaviour payer = origin.building.GetComponent<TransportRequestPaymentHandlerBehaviour>();
				WaypointPath wpp = GetField<WaypointPath>(typeof(WaypointMover), "waypointPath", waypointMover);
				//TransportRequestHandle request = origin.GetComponent<TransportRequestHandle>();
				//ProductDefinition product = request.product;
				//int productAmount = request.productAmount;
				Product product = origin.building.recipeUser.currentOutput;

				int pathLength = wpp.count;
				int productCount = vehicle.productStorage.Count(null);
				float costs = payer.GetVehicleDispatchCost(origin, destination, vehicle, pathLength, productCount);

				if (payer != null) {
					//var transport = new Transport(vehicle, transportJob, waypointMover, origin, destination);
					Transport transport = new Transport(origin, destination, product, productCount, pathLength, costs);
					Transports.Add(transport);
				}
			}
		}

		private void AllVehicles() {
			foreach (var vehicle in ManagerBehaviour<VehicleManager>.instance.vehicles) {
				if (vehicle == null)
					continue;

				if (!(vehicle.GetOwner() is HumanPlayer))
					continue;

				CheckVehicleDelivery(vehicle);
				//VehicleOutput(vehicle);
			}
		}

		private void VehicleOutput(Vehicle vehicle) {
			var sb = new StringBuilder();
			sb.AppendLine(vehicle.name);

			foreach (var product in vehicle.productStorage)
				sb.Append(product.definition.name).Append(", ").AppendLine(vehicle.productStorage.Count(product.definition).ToString());

			if (vehicle.mover is WaypointMover wm && wm.TryGetField<WaypointPath>("waypointPath", out var wpp))
				sb.Append("Path Length: ").AppendLine(wm.tilesInPath.ToString());

			if (vehicle.activeJob != null) {
				sb.Append("Job Type: ").AppendLine(vehicle.activeJob.GetType().ToString());

				if (vehicle.activeJob is LandTransportJob ltj) {
					if (ltj.TryGetField<BuildingLogistics>("_origin", out var originBl)) {
						sb.Append("Origin: ").AppendLine(originBl.buildingName);
						sb.Append("Origin.Settlement: ").AppendLine(originBl.building.settlement.settlementName);
					}

					if (ltj.TryGetField<BuildingLogistics>("_destination", out var destBl)) {
						sb.Append("Destination: ").AppendLine(destBl.buildingName);
						sb.Append("Origin.Settlement: ").AppendLine(destBl.building.settlement.settlementName);
					}

					sb.Append("IsGoingHome(): ").AppendLine(ltj.IsGoingHome().ToString());
				}
			}

			Debug.Log(sb.ToString());
		}

		private void WriteRecipes() {
			var fieldInfo = RecipeDatabase.instance.GetType().GetField("_graph", BindingFlags.Instance | BindingFlags.NonPublic);
			var data = fieldInfo.GetValue(RecipeDatabase.instance);

			if (data != null) {
				var traceWriter = new MemoryTraceWriter();
				jsonSettings.TraceWriter = traceWriter;

				try {
					File.WriteAllText(System.IO.Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RiseOfIndustry", "recipeGraph.json"),
						JsonConvert.SerializeObject(data, jsonSettings));
				} catch {
					Debug.Log("Failed to serialize:\n" + traceWriter);
					throw;
				}
			}
		}

		private String WriteShopData() {
			//get Settlements
			var settlementManager = ManagerBehaviour<SettlementManager>.instance;
			var settlements = settlementManager.settlements;
			StringBuilder sb = new StringBuilder();

			//get Time
			var timeManager = ManagerBehaviour<TimeManager>.instance;
			//loop over all settlements
			foreach (var settlement in settlements) {
				Debug.Log("Settlement " + settlement.settlementName + " found.");
				//loop over all shops
				foreach (var shop in settlement.buildings.shops) {
					Debug.Log("Shop " + shop.buildingName + " found.");

					var sales = GetField<Dictionary<IActor, ProductInfoCollection>>(typeof(Shop), "_sales", shop);

					foreach (var keyValuePair in sales) {
						var actor = keyValuePair.Key;
						var productCollection = keyValuePair.Value;

						if (actor is HumanPlayer) {
							var data = GetField<Dictionary<ProductDefinition, TimeTree<ProductInfoCollection.SaleInfo>>>(typeof(ProductInfoCollection), "_data", productCollection);
							//Debug.Log("PRODUCT COLLECTION DATA: " + data.ToString());
							sb.AppendLine(data.ToString());
						}
					}
				}
			}

			return sb.ToString();
		}

		public static T GetField<T>(Type t, string field, object instance) where T : class {
			if (t == null)
				throw new ArgumentNullException(nameof(t));
			if (string.IsNullOrEmpty(field))
				throw new ArgumentNullException(nameof(field));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			var fieldInfo = t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);

			if (fieldInfo == null)
				throw new ArgumentException($"Could not find field {field} on instance {instance}.");

			var returnVal = fieldInfo.GetValue(instance) as T;

			if (returnVal == null)
				throw new ArgumentException($"Could not convert {fieldInfo.FieldType} to {typeof(T)}.");

			return returnVal;
		}

		public static O GetField<O, T>(string field, object instance)
			where T : class where O : class => GetField<O>(typeof(T), field, instance);
	}

	public static class ReflectionExtensions {
		public static T GetField<T>(this object obj, string field) where T : class {
			var fieldinfo = obj.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
			return fieldinfo.GetValue(obj) as T;
		}

		public static bool TryGetField<T>(this object obj, string field, out T t) where T : class {
			t = obj.GetField<T>(field);
			return t != null;
		}
	}

	public class Excluder : DefaultContractResolver {
		private static readonly List<string> BlackList = new List<string> { "gameDaysFormula", "prefab", "firstIngredient", "firstResult" };

		private static readonly List<Type> TypeBlacklist = new List<Type> {
			typeof(Mesh), typeof(SVGAsset), typeof(Vector3), typeof(Building), typeof(BuildingTopology),
			typeof(BuildingPlacementValidator), typeof(BuildingTag), typeof(BuildingConnectionManager),
			typeof(PermitType), typeof(BuildingAnalysis), typeof(BuildingLogistics), typeof(VehicleQueue),
			typeof(Factory), typeof(ProductSpecificProductStorage), typeof(AiBuildingPlacementEvaluator),
			typeof(BuildingOutline), typeof(GameObject), typeof(IDemolishableRenderer), typeof(ManualDestinationManager),
			typeof(ParticleSystem), typeof(Vector2), typeof(Vector3Int), typeof(Vector2Int), typeof(Vector4),
			typeof(BuildingCategory), typeof(DataCategory), typeof(Region), typeof(SettlementBase)
		};

		private static bool ShouldSerialize(string name) => !BlackList.Any(b => name == b);

		private static bool ShouldSerializeType(Type type) =>
			!TypeBlacklist.Any(t => t == type || (type.IsArray && type.GetElementType() == t) || t.IsAssignableFrom(type) || type.IsAssignableFrom(t));

		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
			var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(pi => ShouldSerializeType(pi.PropertyType))
				.Select(pi => base.CreateProperty(pi, memberSerialization))
				.Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(fi => ShouldSerializeType(fi.FieldType))
				.Select(fi => base.CreateProperty(fi, memberSerialization)))
				.ToList();

			props.ForEach(p => {
				p.Writable = true;
				p.Readable = true;
				p.ShouldSerialize = _ => ShouldSerialize(p.PropertyName);
			});
			return props;
		}
	}
}
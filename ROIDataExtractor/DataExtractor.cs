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

namespace ROIDataExtractor {
	public class DataExtractor : Mod {
		private bool saveReadyCalled;

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

			//run every month
			var timeManager = ManagerBehaviour<TimeManager>.instance;
			if (timeManager != null && (timeManager.today - lastUpdate).Months >= 1)
			{
				SaveReady();
				lastUpdate = timeManager.today;
			}

			//if (timeManager.today.Day == 1 && !saveReadyCalled)
			//	SaveReady();
			//else if (timeManager.today.Day == 2)
			//	saveReadyCalled = false;
		}

		private void SaveReady() {
			saveReadyCalled = true;

			//GetPlayerBalance();
			//WriteFormula();
			//WriteRecipes();
			//WriteShopData();
			VehicleOutput();
		}

		private HumanPlayer Player => ManagerBehaviour<ActorManager>.instance.actors.FirstOrDefault(a => a is HumanPlayer) as HumanPlayer;

		private Dictionary<GameDate, double> BalanceData = new Dictionary<GameDate, double>();

		private void GetPlayerBalance() {
			var moneyManager = ManagerBehaviour<MoneyManager>.instance;
			var balance = moneyManager.GetBalance(Player.money);

			BalanceData.Add(ManagerBehaviour<TimeManager>.instance.today, balance);
			File.WriteAllText(System.IO.Path.Combine(
						Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RiseOfIndustry", "balanceData.json"),
						JsonConvert.SerializeObject(BalanceData, jsonSettings));
		}

		private void WriteFormula()
		{
			var settlementManager = ManagerBehaviour<SettlementManager>.instance;
			var settlements = settlementManager.settlements;

			foreach (var settlement in settlements)
			{
				foreach (var building in Player.buildings)
				{
					var upkeep = building.GetComponent<Upkeep>();

					if (upkeep != null)
					{
						//Debug.Log("Formula: " + JsonConvert.SerializeObject(upkeep.monthlyUpkeep, jsonSettings));
						//Building with gatherers: Costs with no
						Debug.Log("Upkeep of " + building.buildingName + " is " + upkeep.totalMonthlyUpkeep + " $/month.");
					}
				}
			}
		}

		private void VehicleOutput()
		{

			foreach (var vehicle in ManagerBehaviour<VehicleManager>.instance.vehicles)
			{
				if (vehicle == null)
				{
					continue;
				}
				
				var sb = new StringBuilder();
				sb.AppendLine(vehicle.name);

				//Debug.Log("Vehicle " + vehicle.vehicleName + " with job: " + JsonConvert.SerializeObject(vehicle, jsonSettings));
				foreach (var product in vehicle.productStorage)
				{
					sb.Append(product.definition.name).Append(", ").AppendLine(vehicle.productStorage.Count(product.definition).ToString());
				}

				//
				var job = vehicle.activeJob;
				if (job != null)
				{
					foreach (var task in job.tasks)
					{
						if (task is MovePathTask mpt)
						{
							var fieldInfo = typeof(MovePathTask).GetField("_path", BindingFlags.Instance | BindingFlags.NonPublic);
							var data = fieldInfo.GetValue(mpt);

							if (data is List<int> path)
							{
								//path.Aggregate(sb, (sbb, i) => sbb.Append(i).Append(", "));
								path.ForEach(i => sb.Append(i).Append(", "));
								sb.Append("Count: ").AppendLine(path.Count.ToString());
							}
						}
					}
				}
				//

				Debug.Log(sb.ToString());
			}
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

		public static T GetField<T>(Type t, string field, object instance) where T : class
		{
			var fieldInfo = t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
			return fieldInfo.GetValue(instance) as T;
		}
		private void WriteShopData()
		{
			//get Settlements
			var settlementManager = ManagerBehaviour<SettlementManager>.instance;
			var settlements = settlementManager.settlements;

			//get Time
			var timeManager = ManagerBehaviour<TimeManager>.instance;
			//loop over all settlements
			foreach (var settlement in settlements)
			{
				Debug.Log("Settlement " + settlement.settlementName + " found.");
				//loop over all shops
				foreach (var shop in settlement.buildings.shops)
				{
					Debug.Log("Shop " + shop.buildingName + " found.");

					var sales = GetField<Dictionary<IActor, ProductInfoCollection>>(typeof(Shop), "_sales", shop);

					foreach (var keyValuePair in sales)
					{
						var actor = keyValuePair.Key;
						var productCollection = keyValuePair.Value;

						if (actor is HumanPlayer)
						{
							var data = GetField<Dictionary<ProductDefinition, TimeTree<ProductInfoCollection.SaleInfo>>>(typeof(ProductInfoCollection), "_data", productCollection);
							Debug.Log("PRODUCT COLLECTION DATA: " + data.ToString());
						}
					}
				}		
			}
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
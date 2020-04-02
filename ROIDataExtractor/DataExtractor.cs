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

			AllVehicles();

			//run every month
			var timeManager = ManagerBehaviour<TimeManager>.instance;
			if (timeManager != null && (timeManager.today - lastUpdate).Months >= 1) {
				lastUpdate = timeManager.today;

				var sb = new StringBuilder();
				sb.AppendLine("Transports for the last month:");

				Transports.ForEach(t => sb.AppendLine(t.ToString()));

				sb.Append("Count: ").AppendLine(Transports.Count.ToString());
				sb.Append("Average(Cost): ").AppendLine(Transports.Average(t => t.Cost).ToString());
				sb.Append("Sum(Cost): ").AppendLine(Transports.Sum(t => t.Cost).ToString());
				Debug.Log(sb.ToString());

				Transports.Clear();

				//SaveReady();
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
			AllVehicles();
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

		private readonly List<Vehicle> DeliveringVehicles = new List<Vehicle>();
		private readonly List<Transport> Transports = new List<Transport>();

		private struct Transport {
			public float Cost { get; }

			public Transport(float cost) {
				Cost = cost;

				_toString = $"Cost: {Cost}";
			}

			private readonly string _toString;
			public override string ToString() => _toString ?? "";
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

		private void AddTransport(Vehicle v) {
			if (v.activeJob == null) {
				Debug.Log("AddTransport::ActiveJob::Null");
				return;
			}

			if (v.mover == null) {
				Debug.Log("AddTransport::Mover::Null");
				return;
			}

			if (v.activeJob is LandTransportJob ltj
				&& ltj.TryGetField<BuildingLogistics>("_origin", out var origin)
				&& ltj.TryGetField<BuildingLogistics>("_destination", out var dest)
				&& v.mover is WaypointMover wm) {
				var payer = origin.building.GetComponent<TransportRequestPaymentHandlerBehaviour>();

				if (payer != null) {
					var transport = new Transport(payer.GetVehicleDispatchCost(origin, dest, v, wm.tilesInPath, v.productStorage.Count(null)));
					Transports.Add(transport);
					Debug.Log(transport.ToString());
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

		private void WriteShopData() {
			//get Settlements
			var settlementManager = ManagerBehaviour<SettlementManager>.instance;
			var settlements = settlementManager.settlements;

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
							Debug.Log("PRODUCT COLLECTION DATA: " + data.ToString());
						}
					}
				}
			}
		}

		public static T GetField<T>(Type t, string field, object instance) where T : class {
			var fieldInfo = t.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
			return fieldInfo.GetValue(instance) as T;
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
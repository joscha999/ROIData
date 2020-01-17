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
			var bla = ManagerBehaviour<ActorManager>.instance;
			if (bla == null)
				return;

			if (bla.actors.count == 0)
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

			//if (!saveReadyCalled)
			//	SaveReady();

			var timeManager = ManagerBehaviour<TimeManager>.instance;
			if (timeManager != null && (timeManager.today - lastUpdate).Months >= 1) {
				GetPlayerBalance();
				lastUpdate = timeManager.today;
			}
		}

		private void SaveReady() {
			saveReadyCalled = true;

			GetPlayerBalance();
			//WriteRecipes();
		}

		private HumanPlayer Player => ManagerBehaviour<ActorManager>.instance.actors.FirstOrDefault(a => a is HumanPlayer) as HumanPlayer;

		private void GetPlayerBalance() {
			var moneyManager = ManagerBehaviour<MoneyManager>.instance;
			var actorManager = ManagerBehaviour<ActorManager>.instance;
			var timeManager = ManagerBehaviour<TimeManager>.instance;

			Debug.Log($"Found {actorManager.actors.count} actors");
			foreach (var actor in actorManager.actors) {
				Debug.Log($"Found actor \"{actor.actorName}\" with type {actor.GetType()}!");
			}

			foreach (var balance in moneyManager.balances) {
				Debug.Log($"Found balance for \"{balance.Key.actor.actorName}\": {balance.Value}");
			}

			var playerActor = actorManager.actors.FirstOrDefault(a => a is HumanPlayer);

			if (playerActor is HumanPlayer hp) {
				var balance = moneyManager.GetBalance(hp.money);
				Debug.Log("Found Player with balance: " + balance);
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Planspiel.Models;
using ProjectAutomata;
using Steamworks;
using UnityEngine;

namespace ROIData {
    public class ROIDataMod : Mod {
		private const string BaseAddress = "https://localhost:5001/";
		public static HumanPlayer Player => ManagerBehaviour<ActorManager>.instance.actors.FirstOrDefault(a => a is HumanPlayer) as HumanPlayer;
		private bool monthlyUpdateCalled = true;
		private static HttpClient _httpClient;

		//There may only be one HttpClient in the whole application lifetime, will lazy initialize when needed.
		public static HttpClient HttpClient {
			get {
				if (_httpClient == null) {
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
					_httpClient = new HttpClient();
				}
				return _httpClient;
			}
		}

		public List<GameDate> allow = new List<GameDate> {
				//Aufgabe 1 - Scenario1 / Kartoffeln
				new GameDate(1,4,1),
				//Aufgabe 2 - Scenario3 / Holzzüge
				new GameDate(1,6,1),
				//Aufgabe 3 - Scenario5 / Murmeln
				new GameDate(5,8,3),
				//Aufgabe 4 - Scenario7 / Spielzeugzüge
				new GameDate(7,1,1),
				//Aufgabe 5 - Scenario11 / Eisenwaren
				new GameDate(13,9,1)
			};
		public List<GameDate> forbid = new List<GameDate> {
				//Aufgabe 1 - Ende
				new GameDate(1,5,30),
				//Aufgabe 2 - Ende
				new GameDate(3,5,30),
				//Aufgabe 3 - Ende
				new GameDate(6,12,30),
				//Aufgabe 4 - Ende
				new GameDate(9,7,30)
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

			//run every month
			var timeManager = ManagerBehaviour<TimeManager>.instance;
			if (timeManager.today.Day == 1 && !monthlyUpdateCalled)
				MonthlyUpdate();
			else if (timeManager.today.Day == 2)
				monthlyUpdateCalled = false;

			UpdateCanAdvanceTime();
		}

		private void MonthlyUpdate() {
			monthlyUpdateCalled = true;

			SaveDataModel sdm = new SaveDataModel {
				SteamID = (long)SteamUser.GetSteamID().m_SteamID,
				TimeStamp = TimeStampCalculator.GetTimeStamp(),
				Profit = PlayerBalanceCalculator.GetPlayerBalance(),
				CompanyValue = CompanyValueCalculator.GetCompanyValue(),
				DemandSatisfaction = DemandSatisfactionCalculator.CalculateAverageDemandSatisfaction(),
				MachineUptime = MachineUptimeCalculator.GetAverageMachineUptime(),
				AbleToPayLoansBack = LoanCalculator.GetAbleToPayBackLoan(),
				AveragePollution = PollutionCalculator.GetAveragePollution()
			};
			Debug.Log(JsonConvert.SerializeObject(sdm));
			//System.Threading.Tasks.Task.Run(() => PostSaveDataAsync(sdm));
		}

		public static async Task<string> PostSaveDataAsync(SaveDataModel sdm) {
			var postJson = JsonConvert.SerializeObject(sdm);
			var postContent = new StringContent(postJson, Encoding.UTF8, "application/json");
			var response = await HttpClient.PostAsync(BaseAddress + "api/Data", postContent).ConfigureAwait(false);

			return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
		}

		private void UpdateCanAdvanceTime() {
			TimeManager timeManager = ManagerBehaviour<TimeManager>.instance;
			
			if (allow.Contains(timeManager.today)) {
				timeManager.canAdvanceTime = true;
			} else if (forbid.Contains(timeManager.today)) {
				timeManager.canAdvanceTime = false;
			}
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
	}
}

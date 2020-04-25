using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
		public static HumanPlayer Player => ManagerBehaviour<ActorManager>.instance.actors.FirstOrDefault(a => a is HumanPlayer) as HumanPlayer;

		private bool monthlyUpdateCalled = true;

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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Planspiel.Models;
using ProjectAutomata;
using ROIData.HelperClasses;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace ROIData {
    public class ROIDataMod : Mod {
		private const string postAdress = "https://roi.jgdev.de/api/Data";
		public static HumanPlayer Player => ManagerBehaviour<ActorManager>.instance.actors.FirstOrDefault(a => a is HumanPlayer) as HumanPlayer;
		public static Dictionary<CustomEventType, CustomStaticEvent> EventTypeCustomEventPairs
			= new Dictionary<CustomEventType, CustomStaticEvent>();

		private static string sdpath = System.IO.Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RiseOfIndustry", "SaveData");
		private bool alreadySubscribed;
		private bool activatedEvent;

		//GameDate Format: Year Month Day
		//public List<GameDate> taskStartTimes = new List<GameDate> {
		//		//Aufgabe 1 - Scenario1 / Kartoffeln
		//		new GameDate(1,4,1),
		//		//Aufgabe 2 - Scenario3 / Holzzüge
		//		new GameDate(2,4,1),
		//		//Aufgabe 3 - Scenario5 / Murmeln
		//		new GameDate(5,9,1),
		//		//Aufgabe 4 - Scenario7 / Spielzeugzüge
		//		new GameDate(7,9,1),
		//		//Aufgabe 5 - Scenario11 / Eisenwaren
		//		new GameDate(13,9,1)
		//	};
		//public List<GameDate> taskEndTimes = new List<GameDate> {
		//		//Aufgabe 1 - Ende
		//		new GameDate(2,3,30),
		//		//Aufgabe 2 - Ende
		//		new GameDate(3,9,30),
		//		//Aufgabe 3 - Ende
		//		new GameDate(7,8,30),
		//		//Aufgabe 4 - Ende
		//		new GameDate(9,8,30),
		//		//Aufgabe 5 - Ende
		//		new GameDate(15,8,30)
		//	};

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

			//Get instance of Timemanager
			var timeManager = ManagerBehaviour<TimeManager>.instance;

			//run this when onDayEnd-Event fires
			//Create pointer to HandleSomethingHappened
			//Add it to onDayEnd's list of "Event Handlers"

			if (!alreadySubscribed) {
				timeManager.onDayEnd += new TimeManager.TimeManagerCallback(SendData);
				alreadySubscribed = true;
			}

			
			if (!activatedEvent && TryActivateEvent(out var eventManager, out var eventAgent)) {
				activatedEvent = true;
			}

			//Unpause time
			//UpdateCanAdvanceTime();
		}


		//Update -> 
		//PrintEventData 
		//-> ActivateEvent -> AppendEventData
		private bool TryActivateEvent(out WorldEventManager wem, out WorldEventAgent wea) {
			wea = null;
			wem = ManagerBehaviour<WorldEventManager>.instance;
			
			if (Player == null) {
				return false;
			}

			if (wem == null) {
				return false;
			}
			
			wea = Player.GetComponent<WorldEventAgent>();

			if (wea == null) {
				return false;
			}

			var staticEvents = GetField<StaticWorldEventsTrigger>(typeof(WorldEventManager), "_staticEvents", wem);

			if (staticEvents == null) {
				return false;
			}

			if (staticEvents.eventData == null) {
				return false;
			}

			ActivateEvents();

			return true;
		}

		private void ActivateEvents() {
			//AwaitAndExecuteActions();
			CustomStaticEvent.CreateResearchSpeedBoostEvent().TryTrigger();
			//CustomStaticEvent.CreateResearchSpeedBoostEvent().TryTrigger();
			//CustomStaticEvent.CreateResearchSpeedBoostEvent().TryTrigger();
			//CustomStaticEvent.CreateDecreasedDemandAndIncreasedBuildingCostsEvent().TryTrigger();
			//CustomStaticEvent.CreateNetworkSpeedAndDispatchCostEvent().TryTrigger();
			//CustomStaticEvent.CreateIncreasedPollutionRateEvent().TryTrigger();
			CustomStaticEvent.CreatePollutionFineEvent().TryTrigger();


			//Auslösen und Stoppen von Events klappt wenn nicht IsOneTimeEvent.
		}

		private CustomStaticEvent RevolveEvent(CustomEventType type) {
            switch (type) {
                case CustomEventType.IncreasedResearchSpeed: return CustomStaticEvent.CreateResearchSpeedBoostEvent();
                case CustomEventType.PollutionFine: return CustomStaticEvent.CreatePollutionFineEvent();
				case CustomEventType.Grant: //TODO: Add Other Event Creation Methods.
                case CustomEventType.Fine: 
                case CustomEventType.Upkeep: 
                case CustomEventType.TrainShipNetworkSpeedIncrease: 
                case CustomEventType.TrainShipDispatchCost: 
                case CustomEventType.DemandDecrease: 
                case CustomEventType.DemandIncrease: 
                case CustomEventType.BuildingCostIncrease: 
                default: return null;
            }
        }

		private void StopEvent(CustomStaticEvent customEvent) {

			foreach (IWorldEventAgent worldEventAgent in GetField<List<IWorldEventAgent>>(typeof(WorldEventManager), 
				"_worldEventAgents", ManagerBehaviour<WorldEventManager>.instance)) {
				foreach (StaticWorldEvent item in new List<StaticWorldEvent>(worldEventAgent.GetActiveStaticEvents())) {
                    if (item.data.eventName == customEvent.Name) {
						worldEventAgent.EndEvent(item);
					}
				}
			}
		}

        private void AwaitAndExecuteActions() {
			//TODO: every 20 seconds, get action inputs from WebInterface
			//Trigger
			var receivedTrigger = 1000; //ResearchBoost
			CustomEventType receivedKey = (CustomEventType)receivedTrigger;
			
			if (!EventTypeCustomEventPairs.TryGetValue(receivedKey, out CustomStaticEvent customEvent)) {
				customEvent = RevolveEvent(receivedKey);
				customEvent?.TryTrigger();

				//only add to list when not null and not onetimevent, i.e., a Fine or Grant will not be added to the list.
                if (customEvent != null && !customEvent.OneTimeEvent) {
					EventTypeCustomEventPairs.Add(receivedKey, customEvent);
					//CustomEventDebug.PrintEventData(); //Added
				}
			} else {
				StopEvent(customEvent);
				EventTypeCustomEventPairs.Remove(receivedKey);
			}
        }

		private void SendData(GameDate _) {
			SaveDataModel sdm = new SaveDataModel {
				SteamID = (long)SteamUser.GetSteamID().m_SteamID,
				Date = TimeStampCalculator.GetTimeStamp(),
				PassedTime = Time.realtimeSinceStartup,
				Profit = PlayerProfitCalculator.GetProfit(),
				CompanyValue = CompanyValueCalculator.GetCompanyValue(),
				DemandSatisfaction = DemandSatisfactionCalculator.RemainingDemands(),
				MachineUptime = MachineUptimeCalculator.GetAverageMachineUptime(),
				LoansList = LoanCalculator.GetLoansList(),
				AveragePollution = PollutionCalculator.GetAveragePollution(),
				BuildingCount = BuildingCountCalculator.GetBuildingCount(),
				RegionCount = RegionCountCalculator.GetRegionsCount(),
				UnlockedResearchCount = TechTreeCalculator.GetTechTreeUnlocks(),
				Balance = PlayerBalance.GetPlayerBalance()
			};

			var jsonData = JsonConvert.SerializeObject(sdm);
			Debug.Log(jsonData);
			Directory.CreateDirectory(sdpath);

			try {
				File.WriteAllText(System.IO.Path.Combine(
					sdpath, sdm.Date.ToString() + ".json"),
					jsonData);
			} catch (Exception e) {
				Debug.Log("Failed to serialize:\n" + e);
				throw;
			}

			Debug.Log(jsonData);
			//TechTreeCalculator.PrintInformation();
			//PollutionCalculator.GetAveragePollutionAdv();
			//StartCoroutine(PostRequest(postAdress, jsonData));
		}

		private IEnumerator PostRequest(string url, string json) {
			var uwr = new UnityWebRequest(url, "POST");
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
			uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
			uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			uwr.SetRequestHeader("Content-Type", "application/json");

			//Send the request then wait here until it returns
			yield return uwr.SendWebRequest();

			if (uwr.isNetworkError) {
				Debug.Log("Error While Sending: " + uwr.error);
			} else {
				Debug.Log("Received: " + uwr.downloadHandler.text);
			}
		}

		//private void UpdateCanAdvanceTime() {
		//	TimeManager timeManager = ManagerBehaviour<TimeManager>.instance;
			
		//	if (taskStartTimes.Contains(timeManager.today)) {
		//		//timeManager.canAdvanceTime = true;
		//		timeManager.canPauseTime = false;
		//	} else if (taskEndTimes.Contains(timeManager.today)) {
		//		timeManager.canAdvanceTime = false;
		//	}
		//}

		public static T GetField<T>(Type t, string field, object instance, bool isstatic = false) where T : class {
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

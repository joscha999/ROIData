﻿using System;
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
using Harmony;
using ROIData.Patching;

namespace ROIData {
    public class ROIDataMod : Mod {
		
		public static HumanPlayer Player => ManagerBehaviour<ActorManager>.instance.actors.FirstOrDefault(a => a is HumanPlayer) as HumanPlayer;
		public static Dictionary<CustomEventType, CustomStaticEvent> EventTypeCustomEventPairs
			= new Dictionary<CustomEventType, CustomStaticEvent>();
		public static SpeedControls SpeedControls = ManagerBehaviour<SpeedControls>.instance;
		public static AnalyticsManager AnalyticsManager => CachedManagerBehaviour<AnalyticsManager>.instance;

		private bool alreadySubscribed;
		private bool activatedEvent;

		public void Awake() {
			Debug.Log("Patching");
			var harmony = HarmonyInstance.Create("ROIData.SettlementPatch");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

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
                timeManager.onDayEnd += TimeManager_onDayEnd;
				alreadySubscribed = true;
			}

            //TODO: Einkommentieren

            //if (!activatedEvent && TryActivateEvent(out var eventManager, out var eventAgent)) {
            //    activatedEvent = true;
            //}

            //WebConnectionHandler.Update(this);
            //TaskSystem.Update();
        }

        private void TimeManager_onDayEnd(GameDate gd)
        {
            WebConnectionHandler.SendData(this);
        }

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

			var staticEvents = Reflection.GetField<StaticWorldEventsTrigger>(typeof(WorldEventManager), "_staticEvents", wem);

			if (staticEvents == null) {
				return false;
			}

			if (staticEvents.eventData == null) {
				return false;
			}

			ActivateEvents();
			//SettlementModifier.ForceGrowth(Player.hq.region.settlement, 100000);
			//ShopProductPatcher.PrintShopData();
			SpeedyBoi.Register();

			return true;
		}

        private void PrintGameSpeed()
        {
            StringBuilder sb = new StringBuilder();
            SpeedControls sc = ManagerBehaviour<SpeedControls>.instance;

            sb.AppendLine("Level: " + sc.level)
                .AppendLine("BeforePause: " + sc.levelBeforePause);

            Debug.Log(sb.ToString());
        }

        private void ActivateEvents() {
			CustomStaticEvent.CreateLongTermEvent().TryTrigger();
			//CustomStaticEvent.CreateResearchSpeedEvent(100).TryTrigger();
			//CustomStaticEvent.CreateMessageEvent(new EventParams.MessageEventParameters("Hallo Welt,Hallo Welt")).TryTrigger();
			//CustomStaticEvent.CreateDemandEvent(new EventParams.IntProductEventParameters("-50,Potato,Marbles")).TryTrigger();
			//CustomStaticEvent.CreateProductPriceEvent(new EventParams.IntProductEventParameters("100,Fish,Marbles")).TryTrigger(); //Fish $3,82k
		}

		private CustomStaticEvent RevolveEvent(CustomEventType type) {
            switch (type) {
                case CustomEventType.ResearchSpeed: return CustomStaticEvent.CreateResearchSpeedEvent(100);
                case CustomEventType.PollutionFine: return CustomStaticEvent.CreatePollutionFineEvent(100_000);
				case CustomEventType.Grant: return CustomStaticEvent.CreateGrantEvent(new EventParams.IntStringEventParameters("100000,Sie erhalten eine Belohnung."));
                case CustomEventType.Fine: return CustomStaticEvent.CreateFineEvent(new EventParams.IntStringEventParameters("100000,Sie erhalten eine Strafe."));
                case CustomEventType.Upkeep: return CustomStaticEvent.CreateUpkeepEvent(50);
                case CustomEventType.TrainShipNetworkSpeed: return CustomStaticEvent.CreateNetworkSpeedEvent(500);
                case CustomEventType.TrainShipDispatchCost: return CustomStaticEvent.CreateDispactCostEvent(500);
                case CustomEventType.Demand: return CustomStaticEvent.CreateDemandEvent(new EventParams.IntProductEventParameters("-50,Potato,Marbles"));
                case CustomEventType.BuildingCost: return CustomStaticEvent.CreateBuildingCostEvent(50);
                default: return null;
            }
        }

		private void StopEvent(CustomStaticEvent customEvent) {

			foreach (IWorldEventAgent worldEventAgent in Reflection.GetField<List<IWorldEventAgent>>(typeof(WorldEventManager), 
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

		//private void UpdateCanAdvanceTime() {
		//	TimeManager timeManager = ManagerBehaviour<TimeManager>.instance;
			
		//	if (taskStartTimes.Contains(timeManager.today)) {
		//		//timeManager.canAdvanceTime = true;
		//		timeManager.canPauseTime = false;
		//	} else if (taskEndTimes.Contains(timeManager.today)) {
		//		timeManager.canAdvanceTime = false;
		//	}
		//}
	}
}

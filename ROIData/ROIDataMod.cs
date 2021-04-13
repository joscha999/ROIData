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
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace ROIData {
    public class ROIDataMod : Mod {
		private const string postAdress = "https://roi.jgdev.de/api/Data";
		public static HumanPlayer Player => ManagerBehaviour<ActorManager>.instance.actors.FirstOrDefault(a => a is HumanPlayer) as HumanPlayer;
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

			PrintEventData();
			//Unpause time
			//UpdateCanAdvanceTime();
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AppendEventData(StringBuilder stringB, StaticWorldEvent sWorldEvent) {
			stringB.AppendLine("----------------").AppendLine("----------------").AppendLine("StaticWorldEvent: ")
				.Append("Duration (int): ").Append(sWorldEvent.duration).AppendLine("; ")
				.Append("Region: ").Append(sWorldEvent.region.regionName).AppendLine(";")
				//data
				.Append("Event Name (string): ").Append(sWorldEvent.data.eventName).AppendLine(";")
				.Append("Description (string): ").Append(sWorldEvent.data.description).AppendLine(";")
				.Append("When (int): ").Append(sWorldEvent.data.when).AppendLine(";")
				.Append("Trigger (Enum): ").Append(sWorldEvent.data.triggerMode).AppendLine(";");

			if (sWorldEvent.data.catalystBuildings == null) {
				stringB.AppendLine("sWorldEvent.data.catalystBuildings ist null");
			} else {
				stringB.AppendLine("catalystBuildings (List<Building>):");
				foreach (var building in sWorldEvent.data.catalystBuildings) {
					stringB.Append(building.buildingName).Append(";");
				}
				stringB.AppendLine("----------------");
			}	
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

			wem.TriggerStaticEvent("Starvation", wea);
			CustomStaticEvent.CreateResearchEvent().TryTrigger();

			activatedEvent = true;

			return true;
		}

		private void PrintEventData() {
			if (!activatedEvent && TryActivateEvent(out var eventManager, out var eventAgent)) {
				//Debug.Log("If::inner");

				IEnumerable<StaticWorldEvent> activeStaticEvents = eventAgent.GetActiveStaticEvents();
				StringBuilder stringB = new StringBuilder();

				if (activeStaticEvents == null) {
					return;
				}

				//Debug.Log("ForEach::outer");
				foreach (var sWorldEvent in activeStaticEvents) {
					//Debug.Log("ForEach::inner");
					if (sWorldEvent == null) {
						stringB.AppendLine("sWorldEvent ist null");
						continue;
					}

					AppendEventData(stringB, sWorldEvent);
					//Debug.Log("Append::after");
					//NullRef in diesem Abschnitt!!!!

					if (sWorldEvent.effects == null) {
						stringB.AppendLine("sWorldEvent.effects ist null");
					} else {
						stringB.AppendLine("Effects (List<WorldEventEffect>): ");
						//Debug.Log("ForEach::outer");
						foreach (var effect in sWorldEvent.effects) {
							//Debug.Log("ForEach::inner");

							if (effect == null) {
								stringB.AppendLine("effect ist null");
							} else {
								stringB.AppendLine("----------------");
								stringB.Append("ModifierMultiplier (float): ").Append(effect.modifierMultiplier).AppendLine(";");
								stringB.Append("Modifier (float): ").Append(effect.modifier).AppendLine(";");
								stringB.Append("Network (string): ").Append(effect.network).AppendLine(";");

								//WorldEventEffect.region
								if (effect.region == null) {
									stringB.AppendLine("region ist null");
								} else {
									if (effect.region.regionName == null) {
										stringB.AppendLine("effect.region.regionName ist null");
									} else {
										stringB.Append("Region (string): ").Append(effect.region.regionName).AppendLine(";");
									}
								}

								

								//ILists
								GetIListContents(effect, stringB);

								//WorldEventEffect.data
								if (effect.data == null) {
									stringB.AppendLine("effect.data ist null");
								} else {
									stringB.AppendLine("----------------");
									stringB.AppendLine("Data (WorldEventEffectData): ");
									var eData = effect.data;
									stringB.Append("WorldEventEffectType (Enum): ").Append(eData.type).AppendLine(";")
										.Append("WorldEffectApplyOption (Enum): ").Append(eData.whenToApply).AppendLine(",")
										.Append("isCritical (bool): ").Append(eData.isCritical).AppendLine(",")
										.Append("modifier (int): ").Append(eData.modifier).AppendLine(",")
										.Append("moneyAmount (int): ").Append(eData.moneyAmount).AppendLine(",")
										.Append("whereFilter (WorldEventEffectWhereFilter): ").Append(eData.whereFilter).AppendLine(",");
										//.Append("productFilter: ").Append(ed.productFilter).AppendLine(",")
										//.Append("buildingFilter: ").Append(ed.buildingFilter).AppendLine(",")
										//.Append("recipeFilter: ").Append(ed.recipeFilter).AppendLine(",")
										//.Append("vehicleFilter: ").Append(ed.vehicleFilter).AppendLine(",")
										

									GetProductFilter(eData.productFilter, stringB);
									GetBuildingFilter(eData.buildingFilter, stringB);
									GetRecipeFilter(eData.recipeFilter, stringB);

									stringB.Append("networkFilter (string): ").Append(eData.networkFilter.network).AppendLine(",");

									GetVehicleFilter(eData.vehicleFilter, stringB);

									stringB.Append("range (int): ").Append(eData.range).AppendLine(",");
								}
							}
						}
					}
				}

				Debug.Log(stringB.ToString());
			}
		}

		private void GetIListContents(WorldEventEffect effect, StringBuilder stringB) {
			stringB.AppendLine("----------------");
			if (effect.products == null) {
				stringB.Append("effect.products ist null");
			} else {
				stringB.AppendLine("products (IList<ProductDefinition>):");
				foreach (var product in effect.products) {
					stringB.Append(product.productName).Append(";");
				}
			}

			if (effect.buildings == null) {
				stringB.AppendLine("effect.buildings ist null");
			} else {
				stringB.AppendLine("buildings (IList<Building>):");
				foreach (var building in effect.buildings) {
					stringB.Append(building.buildingName).Append(";");
				}
			}

			if (effect.vehicles == null) {
				stringB.AppendLine("effect.vehicles ist null");
			} else {
				stringB.AppendLine("vehicles (IList<Vehicle>):");
				foreach (var vehicle in effect.vehicles) {
					stringB.Append(vehicle.vehicleName).Append(";");
				}
			}

			if (effect.recipes == null) {
				stringB.AppendLine("effect.recipes ist null");
			} else {
				stringB.AppendLine("recipes (IList<Recipe>):");
				foreach (var recipe in effect.recipes) {
					stringB.Append(recipe.Title).Append(";");
				}
			}
		}

		private void GetBuildingFilter(WorldEventBuildingFilter webf, StringBuilder sb) {
			sb.AppendLine("----------------");
			sb.AppendLine("BuildingFilter (WorldEventBuildingFilter): ")
				.Append("fromAll (bool): ").Append(webf.fromAll).AppendLine(";")
				.Append("Amound (Enum): ").Append(webf.amount).AppendLine(";");

			if (webf.fromBuildings == null) {
				Debug.Log("fromBuildings is null");
			} else {
				sb.AppendLine("fromBuildings (Building[]):");
				foreach (var building in webf.fromBuildings) {
					sb.Append(building.buildingName).Append(";");
				}
			}

			if (webf.fromTags == null) {
				Debug.Log("fromTags is null");
			} else {
				sb.AppendLine("fromTags (BuildingTag[]):");
				foreach (var tag in webf.fromTags) {
					sb.Append(tag.ToString()).Append(";");
				}
			}

			if (webf.exceptBuildings == null) {
				Debug.Log("exceptBuildings is null");
			} else {
				sb.AppendLine("exceptBuildings (Building[]):");
				foreach (var building in webf.exceptBuildings) {
					sb.Append(building.buildingName).Append(";");
				}
			}
		}

		private void GetRecipeFilter(WorldEventRecipeFilter werf, StringBuilder sb) {
			sb.AppendLine("----------------");
			sb.AppendLine("RecipeFilter (WorldEventRecipeFilter): ")
				.Append("fromAll (bool): ").Append(werf.fromAll).AppendLine(";")
				.Append("Amount (Enum): ").Append(werf.amount).AppendLine(";");

			if (werf.fromRecipes == null) {
				Debug.Log("fromRecipes is null");
			} else {
				sb.AppendLine("fromRecipes (Recipe[]):");
				foreach (var recipe in werf.fromRecipes) {
					sb.Append(recipe.Title).Append(";");
				}
			}

			if (werf.resultProducts == null) {
				Debug.Log("resultProducts is null");
			} else {
				sb.AppendLine("resultProducts (ProductDefinition[]):");
				foreach (var product in werf.resultProducts) {
					sb.Append(product.productName).Append(";");
				}
			}

			if (werf.resultProductsTags == null) {
				Debug.Log("resultProductsTags is null");
			} else {
				sb.AppendLine("resultProductsTags (ProductTag[]):");
				foreach (var tag in werf.resultProductsTags) {
					sb.Append(tag.ToString()).Append(";");
				}
			}
		}

		private void GetVehicleFilter(WorldEventVehicleFilter wevf, StringBuilder sb) {
			sb.AppendLine("----------------");
			sb.AppendLine("VehicleFilter (WorldEventVehicleFilter): ")
				.Append("fromAll (bool): ").Append(wevf.fromAll).AppendLine(";")
				.Append("Amount (Enum): ").Append(wevf.amount).AppendLine(";");

			if (wevf.fromVehicles == null) {
				Debug.Log("fromVehicles is null");
			} else {
				sb.AppendLine("fromVehicles (Vehicle[]):");
				foreach (var vehicle in wevf.fromVehicles) {
					sb.Append(vehicle.vehicleName).Append(";");
				}
			}
		}

		private void GetProductFilter(WorldEventProductFilter wepf, StringBuilder sb) {
			sb.AppendLine("----------------");
			sb.AppendLine("ProductFilter")
				.Append("fromAll: ").Append(wepf.fromAll).AppendLine(";")
				.Append("Amount (Enum): ").Append(wepf.amount).AppendLine(";");

			if (wepf.fromProducts == null) {
				Debug.Log("fromProducts is null");
			} else {
				sb.AppendLine("fromProducts (ProductDefinition[]):");
				foreach (var pDef in wepf.fromProducts) {
					sb.Append(pDef.productName).Append(";");
				}
			}

			if (wepf.fromTags == null) {
				Debug.Log("fromTags is null");
			} else {
				sb.AppendLine("Tags (ProductTag[]): ");
				foreach (var tag in wepf.fromTags) {
					sb.Append(tag.ToString()).Append(";");
				}
			}

			if (wepf.categoryRestrictions == null) {
				Debug.Log("categoryRestrictions is null");
			} else {
				sb.AppendLine("categoryRestrictions (DataCategory[]):");
				foreach (var dCategory in wepf.categoryRestrictions) {
					sb.Append(dCategory.categoryName);
				}
			}
			if (wepf.producers == null) {
				Debug.Log("producers is null");
			} else {
				sb.AppendLine("producers (Building[]):");
				foreach (var producer in wepf.producers) {
					sb.Append(producer.buildingName).Append(";");
				}
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

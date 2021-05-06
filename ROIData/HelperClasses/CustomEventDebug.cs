using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ROIData.HelperClasses {
    class CustomEventDebug {

		public static void PrintEventData() {
			
			//Debug.Log("If::inner");

			IEnumerable<StaticWorldEvent> activeStaticEvents = ROIDataMod.Player.GetComponent<WorldEventAgent>().GetActiveStaticEvents();
			
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

		public static void GetIListContents(WorldEventEffect effect, StringBuilder stringB) {
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

		public static void GetBuildingFilter(WorldEventBuildingFilter webf, StringBuilder sb) {
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
		public static void GetRecipeFilter(WorldEventRecipeFilter werf, StringBuilder sb) {
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

		public static void GetVehicleFilter(WorldEventVehicleFilter wevf, StringBuilder sb) {
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

		public static void GetProductFilter(WorldEventProductFilter wepf, StringBuilder sb) {
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
		public static void AppendEventData(StringBuilder stringB, StaticWorldEvent sWorldEvent) {
			stringB.AppendLine("----------------").AppendLine("----------------").AppendLine("StaticWorldEvent: ")
				.Append("Duration (int): ").Append(sWorldEvent.duration).AppendLine("; ")
				.Append("Region: ").Append(sWorldEvent.region.regionName).AppendLine(";")
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
	}
}

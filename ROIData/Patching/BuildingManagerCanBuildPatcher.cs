using Harmony;
using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ROIData.Patching {
	[HarmonyPatch(typeof(BuildingManager))]
	[HarmonyPatch("CanBuild")]
	[HarmonyPatch(new Type[] { typeof(int) })]
	public class BuildingManagerCanBuildIndexPatcher {
		public static bool PlacementEnabled { get; set; } = true;

		static bool Prefix() => PlacementEnabled;

		static void Postfix(ref bool __result) {
			if (!PlacementEnabled) {
				Debug.Log("BuildingManagerCanBuildIndexPatcher::Postfix::__result=false");
				__result = false;
			}
		}
	}

	[HarmonyPatch(typeof(BuildingManager))]
	[HarmonyPatch("CanBuild")]
	[HarmonyPatch(new Type[] { typeof(Building) })]
	public class BuildingManagerCanBuildBuildingPatcher {
		static bool Prefix() => BuildingManagerCanBuildIndexPatcher.PlacementEnabled;

		static void Postfix(ref bool __result) {
			if (!BuildingManagerCanBuildIndexPatcher.PlacementEnabled) {
				Debug.Log("BuildingManagerCanBuildBuildingPatcher::Postfix::__result=false");
				__result = false;
			}
		}
	}
}
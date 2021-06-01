using Harmony;
using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.Patching {
    [HarmonyPatch(typeof(PermitManager))]
    [HarmonyPatch("AuctionPermit")]
    [HarmonyPatch(new Type[] { typeof(IActor), typeof(Region), typeof(PermitType) })]
    class PermitManagerAuctionPermitPatcher {
        static void Postfix(PermitManager __instance, ref IActor actor, ref Region region, ref PermitType permitType, ref bool __result) {
            __instance.GrantPermitToActor(region, permitType, actor);
            ManagerBehaviour<AuctionsManager>.instance.EndCurrentAuction(true);
            __result = true;
        }
    }
}

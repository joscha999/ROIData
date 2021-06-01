using Harmony;
using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.Patching {
    [HarmonyPatch(typeof(AuctionDefinition))]
    [HarmonyPatch("GenerateAuction")]
    [HarmonyPatch(new Type[] { typeof(IActor), typeof(IAuctionReward) })]
    class AuctionDefinitionGenerateAuctionPatcher {
        static void Postfix(AuctionDefinition __instance, ref Auction __result, ref IActor auctioner, ref IAuctionReward reward) {
            __result = new Auction(__instance, 0, __instance.startingBid, __instance.bidController, reward, auctioner);
        }
    }
}
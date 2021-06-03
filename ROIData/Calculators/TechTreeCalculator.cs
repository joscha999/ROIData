using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;
using UnityEngine;

namespace ROIData {
    public static class TechTreeCalculator {
        public static int GetTechTreeUnlocks() => ROIDataMod.Player.Get<TechTreeAgent>().GetUnlocked().Count;

        public static void PrintInformation() {

            StringBuilder sb = new StringBuilder();
            sb.Append("Found the following unlocks: ");

            foreach (var item in ROIDataMod.Player.Get<TechTreeAgent>().GetUnlocked()) {
                sb.Append(item.unlockName).AppendLine();
            }

            Debug.Log(sb.ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public class PlayerBalance {

        public static double GetPlayerBalance() => ManagerBehaviour<MoneyManager>.instance.GetBalance(ROIDataMod.Player.Get<MoneyAgent>());
    }
}

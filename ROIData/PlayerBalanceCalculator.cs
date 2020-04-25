using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class PlayerBalanceCalculator {
        public static double GetPlayerBalance() {
            MoneyManager manager = ManagerBehaviour<MoneyManager>.instance;
            return manager.GetBalance(ROIDataMod.Player.money);
        }
    }
}

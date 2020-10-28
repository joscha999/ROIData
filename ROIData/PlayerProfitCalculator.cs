using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class PlayerProfitCalculator {
        private static readonly GameDate today = ManagerBehaviour<TimeManager>.instance.today;
        public static double GetProfit() => ROIDataMod.Player.Get<MoneyAgent>().GetProfit(today.AddDays(-1), today);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;
using ROIData.HelperClasses;
using UnityEngine;

namespace ROIData {
    public static class PlayerProfitCalculator {
        //private static GameDate Today => ManagerBehaviour<TimeManager>.instance.today;
        //private static MoneyAgent MoneyAgent => ROIDataMod.Player.Get<MoneyAgent>();
        //public static double GetProfit() => MoneyAgent.GetProfit(Today.AddDays(-GamePeriod.day.CountDays()), Today);

        public static double GetProfit() {
            MoneyAgent moneyAgent = ROIDataMod.Player.Get<MoneyAgent>();

            GameDate today = ManagerBehaviour<TimeManager>.instance.today;
            GameDate yesterday = today.AddDays(-1);

            //return moneyAgent.GetProfit(yesterday, today);
            return moneyAgent.GetProfit(yesterday, today);
        }

        public static void DebugMoneyBills(GameDate today) {
            StringBuilder sb = new StringBuilder();

            foreach (var bill in GetMoneyBills()) {
                //if (bill.date == today.AddDays(-1)) {
                    sb.AppendLine("Amount: " + bill.amount + ", MoneyBillCategory: " + bill.category.categoryName +
                        ", Recipient: " + bill.recipient.actor.actorName + ", Originator: " + bill.originator.actor.actorName +
                        ", GameDate: " + bill.date.ToString())
                        .AppendLine();
                //} 
            }

            Debug.Log(sb.ToString());
        }

        private static List<MoneyBill> GetMoneyBills() {
            MoneyAgent moneyAgent = ROIDataMod.Player.Get<MoneyAgent>();
            var billTimeTree = Reflection.GetField<Dictionary<int, MoneyBillTimeTree>>(typeof(MoneyAgent), "_bills", moneyAgent);
            List<MoneyBill> moneyBills = new List<MoneyBill>();

            foreach (var timeTree in billTimeTree.Values) {
                moneyBills.AddRange(timeTree.GetAll());
            }

            return moneyBills;
        }
    }
}
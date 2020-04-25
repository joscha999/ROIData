using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class LoanCalculator {
        public static bool GetAbleToPayBackLoan() {
            LoansAgent loansAgent = ROIDataMod.Player.GetComponent<LoansAgent>();
            double amountToPay = 0;

            foreach (Loan loan in loansAgent.loans) {
                amountToPay += loan.amountToPay;
            }

            return PlayerBalanceCalculator.GetPlayerBalance() > amountToPay * 1.1;
        }
    }
}

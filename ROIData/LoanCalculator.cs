using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class LoanCalculator {
        private static LoansAgent LoansAgent => ROIDataMod.Player.GetComponent<LoansAgent>();

        public static bool GetAbleToPayBackLoan() {

            double amountToPay = 0;

            foreach (Loan loan in LoansAgent.loans) {
                amountToPay += loan.amountToPay;
            }

            return PlayerProfitCalculator.GetProfit() > amountToPay * 1.1;
        }

        public static List<LoanInfo> GetLoansList() {
            List<LoanInfo> loans = new List<LoanInfo>();

            foreach (Loan loan in LoansAgent.loans) {
                loans.Add(new LoanInfo(loan.amount, loan.apr));
            }

            return loans;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData {
    public class LoanInfo {
        public float LoanAmount { get; set; }
        public float LoanInterest { get; set; }

        public LoanInfo(float loanAmount, float loanInterest) {
            LoanAmount = loanAmount;
            LoanInterest = loanInterest;
        }

        public override string ToString() {
            return "Amount: " + LoanAmount + ", Interest: " + LoanInterest;
        }
    }
}

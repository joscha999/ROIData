using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAutomata;

namespace ROIData {
    public static class CompanyValueCalculator {
        public static double GetCompanyValue() {
            return ROIDataMod.Player.GetComponent<CompanyStats>().ComputeCompanyValue();
        }
    }
}

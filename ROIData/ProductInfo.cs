using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData {
    public class ProductInfo {

        public string ProductName { get; }
        public int RemainingDemand { get; }

        public ProductInfo(string productName, int remainingDemand) {
            ProductName = productName;
            RemainingDemand = remainingDemand;
        }

        public override string ToString() {
            return "Product: " + ProductName + ", Demand: " + RemainingDemand;
        }
    }
}

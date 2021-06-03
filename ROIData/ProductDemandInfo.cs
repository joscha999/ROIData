using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData {
    public class ProductDemandInfo {

        public string ProductName { get; }
        public string Settlement { get; set; }
        public string Shop { get; set; }
        public int Demand { get; set; }
        public int Sales { get; }

        public ProductDemandInfo(string product, string settlement, string shop, int demand, int sales) {
            ProductName = product;
            Settlement = settlement;
            Shop = shop;
            Demand = demand;
            Sales = sales;
        }

        public override string ToString() {
            return "Product: " + ProductName + ", Settlement: " + Settlement + ", Shop: " + Shop + ", Demand: " + Demand + ", Sales: " + Sales;
        }
    }
}

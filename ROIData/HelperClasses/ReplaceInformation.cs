using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.HelperClasses {
    public class ReplaceInformation {
        public string Settlement { get; set; }
        public string Shop { get; set; }
        public string OldProduct { get; set; }
        public string NewProduct { get; set; }
        public int NewDemand { get; set; }
        public float NewPrice { get; set; }

        public ProductDefinition NewProductDef => GetProductDefinition(NewProduct);

        private static ProductDefinition GetProductDefinition(string name) {
            return GameData.instance.GetAssetsRO(typeof(ProductDefinition))
                    .FirstOrDefault(p => p.name == name) as ProductDefinition;
        }
    }
}

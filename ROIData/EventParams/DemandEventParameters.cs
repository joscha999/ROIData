using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.EventParams
{
    public class DemandEventParameters : EventParameters
    {
        public int Modifier { get; set; }
        public ProductDefinition[] Products { get; set; }

        public DemandEventParameters(string value) : base(value)
        {
        }

        protected override void Parse()
        {
            Modifier = int.Parse(Data[0]);

            Products = new ProductDefinition[Data.Count - 1];

            for (int i = 1; i < Data.Count; i++)
            {
                Products[i - 1] = GameData.instance.GetAssetsRO(typeof(ProductDefinition))
                    .FirstOrDefault(p => p.name == Data[i]) as ProductDefinition;
            }
        }
    }
}

using ProjectAutomata;
using ROIData.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.CustomFilters
{
    public class CustomNetworkFilter
    {

        private CustomNetworkType CustomNetworkType;

        public CustomNetworkFilter(CustomNetworkType networkType) => CustomNetworkType = networkType;

        public WorldEventNetworkFilter Build()
        {
            return new WorldEventNetworkFilter
            {
                network = CustomNetworkType.ToString()
            };
        }
    }
}

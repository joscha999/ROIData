using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.CustomFilters
{
    public class CustomVehicleFilter
    {
        private WorldEventEffectFilterAmount Amount;
        private bool FromAll;
        private List<Vehicle> Vehicles = new List<Vehicle>();

        public CustomVehicleFilter(WorldEventEffectFilterAmount amount, bool fromAll)
        {
            Amount = amount;
            FromAll = fromAll;
        }

        public CustomVehicleFilter WithVehicle(Vehicle vehicle)
        {
            Vehicles.Add(vehicle);
            return this;
        }

        public CustomVehicleFilter WithVehicles(IEnumerable<Vehicle> vehicles)
        {
            Vehicles.AddRange(vehicles);
            return this;
        }

        public WorldEventVehicleFilter Build()
        {
            return new WorldEventVehicleFilter
            {
                amount = Amount,
                fromAll = FromAll,
                fromVehicles = Vehicles.ToArray()
            };
        }
    }
}
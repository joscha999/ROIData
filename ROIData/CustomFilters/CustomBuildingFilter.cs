using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.CustomFilters
{
    public class CustomBuildingFilter
    {
        private WorldEventEffectFilterAmount Amount;
        private bool FromAll;
        private List<Building> ExceptBuildings = new List<Building>();
        private List<Building> FromBuildings = new List<Building>();
        private List<BuildingTag> BuildingTags = new List<BuildingTag>();

        public CustomBuildingFilter(WorldEventEffectFilterAmount amount, bool fromAll)
        {
            Amount = amount;
            FromAll = fromAll;
        }

        public CustomBuildingFilter WithBuildingException(Building building)
        {
            ExceptBuildings.Add(building);
            return this;
        }

        public CustomBuildingFilter WithBuildingExceptions(IEnumerable<Building> buildings)
        {
            ExceptBuildings.AddRange(buildings);
            return this;
        }

        public CustomBuildingFilter WithBuilding(Building building)
        {
            FromBuildings.Add(building);
            return this;
        }

        public CustomBuildingFilter WithBuildings(IEnumerable<Building> buildings)
        {
            FromBuildings.AddRange(buildings);
            return this;
        }

        public CustomBuildingFilter WithBuildingTag(BuildingTag buildingTag)
        {
            BuildingTags.Add(buildingTag);
            return this;
        }

        public CustomBuildingFilter WithBuildingTags(IEnumerable<BuildingTag> buildingTags)
        {
            BuildingTags.AddRange(buildingTags);
            return this;
        }

        public WorldEventBuildingFilter Build()
        {
            return new WorldEventBuildingFilter
            {
                amount = Amount,
                exceptBuildings = ExceptBuildings.ToArray(),
                fromAll = FromAll,
                fromBuildings = FromBuildings.ToArray(),
                fromTags = BuildingTags.ToArray()
            };
        }
    }
}

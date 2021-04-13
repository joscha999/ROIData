using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.CustomData
{
    public class CustomEffectData
    {
        private WorldEventEffectApplyOption WorldEventEffectApplyOption;
        private WorldEventEffectWhereFilter WorldEventEffectWhereFilter;
        private WorldEventEffectType WorldEventEffectType;
        private WorldEventBuildingFilter WorldEventBuildingFilter;
        private WorldEventProductFilter WorldEventProductFilter;
        private WorldEventRecipeFilter WorldEventRecipeFilter;
        private WorldEventVehicleFilter WorldEventVehicleFilter;
        private bool Critical;
        private int Modifier;
        private int MoneyAmount;
        private int Range;

        public CustomEffectData(WorldEventEffectType worldEventEffectType, bool critical, int modifier, int moneyAmount, int range) {
            WorldEventEffectType = worldEventEffectType;
            Critical = critical;
            Modifier = modifier;
            MoneyAmount = moneyAmount;
            Range = range;
        }

        public CustomEffectData SetApplyOption(WorldEventEffectApplyOption applyOption)
        {
            WorldEventEffectApplyOption = applyOption;
            return this;
        }

        public CustomEffectData AddWhereFilter(WorldEventEffectWhereFilter whereFilter)
        {
            WorldEventEffectWhereFilter = whereFilter;
            return this;
        }

        public CustomEffectData AddBuildingFilter(WorldEventBuildingFilter buildingFilter)
        {
            WorldEventBuildingFilter = buildingFilter;
            return this;
        }

        public CustomEffectData AddProductFilter(WorldEventProductFilter productFilter)
        {
            WorldEventProductFilter = productFilter;
            return this;
        }

        public CustomEffectData AddRecipeFilter(WorldEventRecipeFilter recipeFilter)
        {
            WorldEventRecipeFilter = recipeFilter;
            return this;
        }

        public CustomEffectData AddVehicleFilter(WorldEventVehicleFilter vehicleFilter)
        {
            WorldEventVehicleFilter = vehicleFilter;
            return this;
        }

        public WorldEventEffectData Build()
        {
            return new WorldEventEffectData
            {
                buildingFilter = WorldEventBuildingFilter,
                isCritical = Critical,
                modifier = Modifier,
                moneyAmount = MoneyAmount,
                networkFilter = new WorldEventNetworkFilter(),
                productFilter = WorldEventProductFilter,
                range = Range,
                recipeFilter = WorldEventRecipeFilter,
                type = WorldEventEffectType,
                vehicleFilter = WorldEventVehicleFilter,
                whenToApply = WorldEventEffectApplyOption,
                whereFilter = WorldEventEffectWhereFilter
            };
        }
    }
}

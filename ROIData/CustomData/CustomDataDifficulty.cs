using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.CustomData
{
    public class CustomDataDifficulty
    {
        private int MinDuration;
        private int MaxDuration;
        private List<WorldEventEffectData> WorldEventEffectData = new List<WorldEventEffectData>();

        public CustomDataDifficulty SetMinDuration(int duration)
        {
            MinDuration = duration;
            return this;
        }

        public CustomDataDifficulty SetMaxDuration(int duration)
        {
            MaxDuration = duration;
            return this;
        }

        public CustomDataDifficulty WithEffect(WorldEventEffectData effectData)
        {
            WorldEventEffectData.Add(effectData);
            return this;
        }

        public CustomDataDifficulty WithEffects(IEnumerable<WorldEventEffectData> effectDatas)
        {
            WorldEventEffectData.AddRange(effectDatas);
            return this;
        }

        public WorldEventDataDifficulty Build()
        {
            return new WorldEventDataDifficulty
            {
                durationMax = MinDuration,
                durationMin = MaxDuration,
                effects = WorldEventEffectData
            };
        }
    }
}

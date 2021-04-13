using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.CustomData
{
    public class CustomCreationParams
    {
        private int Difficulty;
        private int Duration;
        private bool Permanent;
        private bool ForceEvent;
        private float DurationMultiplier;
        private float ModifierMultiplier;
        private Region Region;
        private List<WorldEventEffectBuildSettings> WorldEventEffectBuildSettings = new List<WorldEventEffectBuildSettings>();
        private List<WorldEventEffectData> WorldEventEffectData = new List<WorldEventEffectData>();

        public CustomCreationParams(bool permanent, bool forceEvent)
        {
            Permanent = permanent;
            ForceEvent = forceEvent;
        }

        public CustomCreationParams SetDifficulty(int difficulty)
        {
            Difficulty = difficulty;
            return this;
        }

        public CustomCreationParams SetDuration(int duration)
        {
            Duration = duration;
            return this;
        }

        public CustomCreationParams SetDurationMultiplier(float multiplier)
        {
            DurationMultiplier = multiplier;
            return this;
        }

        public CustomCreationParams SetEffectMultiplier(float multiplier)
        {
            ModifierMultiplier = multiplier;
            return this;
        }

        public CustomCreationParams SetRegion(Region region)
        {
            Region = region;
            return this;
        }

        public CustomCreationParams WithBuildSetting(WorldEventEffectBuildSettings buildSetting)
        {
            WorldEventEffectBuildSettings.Add(buildSetting);
            return this;
        }

        public CustomCreationParams WithBuildSettings(IEnumerable<WorldEventEffectBuildSettings> buildSettings)
        {
            WorldEventEffectBuildSettings.AddRange(buildSettings);
            return this;
        }
        public CustomCreationParams WithEffectData(WorldEventEffectData effectData)
        {
            WorldEventEffectData.Add(effectData);
            return this;
        }
        public CustomCreationParams WithEffectDatas(IEnumerable<WorldEventEffectData> effectDatas)
        {
            WorldEventEffectData.AddRange(effectDatas);
            return this;
        }

        public WorldEventCreationParams Build()
        {
            return new WorldEventCreationParams
            {
                region = Region,
                permanent = Permanent,
                forceEvent = ForceEvent,
                additionalEffects = WorldEventEffectData.ToArray(),
                difficulty = Difficulty,
                duration = Duration,
                durationMult = DurationMultiplier,
                modifierMult = ModifierMultiplier,
                settings = WorldEventEffectBuildSettings.ToArray()
            };
        }
    }
}

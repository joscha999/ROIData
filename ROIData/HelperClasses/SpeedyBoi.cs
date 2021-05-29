using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ROIData.HelperClasses
{
    public class SpeedyBoi : IGameSpeedOverride
    {
        private static SpeedyBoi Instance;
        private static bool RequestedPause;

        private SpeedyBoi()
        {
            Instance = this;
        }

        public bool OverrideSpeedLevel(int currentSpeedLevel, out int speedLevel)
        {
            if (RequestedPause)
            {
                speedLevel = -1;
                return false;
            }
            
            var speedManager = ManagerBehaviour<SpeedControls>.instance;
            speedManager.ForceTimeScale(10);
            speedLevel = 10;
            return true;
        }

        public static void Register()
        {
            var speedManager = ManagerBehaviour<SpeedControls>.instance;
            var speedyBoi = Instance ?? new SpeedyBoi();

            var overrides = Reflection.GetField<List<IGameSpeedOverride>>(typeof(SpeedControls),
                "_speedOverrides", speedManager);

            if (overrides.Contains(speedyBoi))
                return;

            overrides.Insert(0, speedyBoi);

            speedManager.speedLevels = Enumerable.Range(1, 21).Select(i => (float)i).ToArray();

            speedManager.onLevelChangeEvent += SpeedManager_onLevelChangeEvent;

            //var oldLevels = speedManager.speedLevels;
            //speedManager.speedLevels = new float[oldLevels.Length + 1];
            //Array.Copy(oldLevels, speedManager.speedLevels, oldLevels.Length);
            //speedManager.speedLevels[speedManager.speedLevels.Length - 1] = 20;
        }

        private static void SpeedManager_onLevelChangeEvent(int level)
        {
            RequestedPause = level == -1;
        }
    }
}

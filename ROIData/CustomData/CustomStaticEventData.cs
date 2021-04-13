using ProjectAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ROIData.CustomData
{
    public class CustomStaticEventData
    {
        private string Name;
        private string Description;
        private int When;
        private WorldEventTriggerMode WorldEventTriggerMode;
        private WorldEventDataDifficulty Easy;
        private List<Building> CatalystBuildings = new List<Building>();

        public CustomStaticEventData(WorldEventDataDifficulty easy)
        {
            Easy = easy;
        }

        public CustomStaticEventData WithName(string name)
        {
            Name = name;
            return this;
        }

        public CustomStaticEventData WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public CustomStaticEventData WithWhen(int when)
        {
            When = when;
            return this;
        }

        public CustomStaticEventData WithTrigger(WorldEventTriggerMode triggerMode)
        {
            WorldEventTriggerMode = triggerMode;
            return this;
        }

        public CustomStaticEventData WithBuilding(Building building)
        {
            CatalystBuildings.Add(building);
            return this;
        }

        public CustomStaticEventData WithBuildings(IEnumerable<Building> buildings)
        {
            CatalystBuildings.AddRange(buildings);
            return this;
        }

        public StaticWorldEventData Build()
        {
            StaticWorldEventData eventData = ScriptableObject.CreateInstance<StaticWorldEventData>();

            eventData.eventName = Name;
            eventData.description = Description;
            eventData.when = When;
            eventData.triggerMode = WorldEventTriggerMode;
            eventData.easy = Easy;
            eventData.normal = Easy;
            eventData.hard = Easy;
            eventData.catalystBuildings = CatalystBuildings;
            //?
            eventData.opposite = new List<StaticWorldEventData>();

            return eventData;
        }
    }
}

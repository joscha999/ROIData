using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.Models {
	public class AssignmentAction
	{
		public int ID { get; set; }
		public bool Started { get; set; }
		public int GroupSteamID { get; set; } = -1;
		public int SecondsFromStart { get; set; }
		public AssignmentActionType Type { get; set; }
		public string Value { get; set; }
	}

	//NOTE: Never change these values while tasks are planned/running
	public enum AssignmentActionType {
		None = 0,

		//Control 1..99
		TaskStart = 1,
		TaskEnd = 2,
		Pause = 3,
		Unpause = 4,
		CreateSave = 5,
		DisplayMessage = 6,

		//Event Triggers 1000..
		ResearchSpeed = 1000,
		PollutionFine = 1001,
		Grant = 1002,
		Fine = 1003,
		Upkeep = 1004,
		TrainShipNetworkSpeed = 1005,
		TrainShipDispatchCost = 1006,
		Demand = 1007, //Value is int modifier
		BuildingCost = 1008,
	}
}
using System;

namespace ROIData.HelperClasses {

    public enum EventType {
        IncreasedResearchSpeed = 1000,
        PollutionFine = 1001,
        Grant = 1002,
        Fine = 1003,
        Upkeep = 1004,
        TrainShipNetworkSpeedIncrease = 1005,
        TrainShipDispatchCost = 1006,
        DemandDecrease = 1007,
        DemandIncrease = 1008,
        BuildingCostIncrease = 1009
    }
}



//Dictionary containing all events Key = Enum Value, Value = event

//Convert Example:
//WebInfReq -> assignmentTaskType = 1000
//var activateThis = (EventType)assignmentTaskType
//activateThis = IncreasedResearchSpeed

//Run Example
//look through dictionary
//if assignMentTaskType = Key
//if not already inside active events: add to currently active events list, existiert in der liste ein VALUE zu dem der KEY der angegeben wurde gehört?
//execute event event

//
//
//:
//


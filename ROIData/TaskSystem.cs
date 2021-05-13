using ProjectAutomata;
using ROIData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ROIData
{
    public static class TaskSystem
    {
        //RealTimeSinceTaskStart Field (float)
        private static float RealTimeSinceTaskStart;
        //CurrentTaskID field (aktuell laufenden aufgabe)
        private static AssignmentTask CurrentTask;
        //Übergeordnete Liste an Aufgaben
        private static List<AssignmentTask> TotalTasks = new List<AssignmentTask>();

        public static Dictionary<AssignmentActionType, CustomStaticEvent> EventTypeCustomEventPairs
            = new Dictionary<AssignmentActionType, CustomStaticEvent>();

        public static void ReceiveTasks(List<AssignmentTask> tasks)
        {
            //Liste mit Daten füllen (ist da ein neuer task drin)
            foreach (var task in tasks) {
                if (!TotalTasks.Contains(task)) {
                    TotalTasks.Add(task);
                } else { //Tasks aus tasks ist in TotalTasks enthalten
                    foreach (var action in task.Actions) {
                        if (!task.Actions.Contains(action)) {
                            //TODO: ist in der task selbst eine neue action enthalten? ist die action id bereits vorhanden?
                        }
                    }
                }
            } 
        }

        //TODO: Methode aufrufen. Wo? Logik so korrekt?
        public static void Update() {
            //- realtimesincetaskstart += time.unscaleddeltatime (um sachen in der aufgabe zeitlich zu steuern)
            RealTimeSinceTaskStart += Time.unscaledDeltaTime; //TODO: richtig so?

            foreach (var task in TotalTasks) {
                //wurde die utc starttime überschritten wenn kein task läuft -> task starten (id setzen, bzw task speichern)
                if (task.Started == false && RealTimeSinceTaskStart > task.UTCStart.Second) {
                    //Task starten
                    CurrentTask = task;
                    task.Started = true;
                    
                    if (CurrentTask.ID != 0) {
                        //wenn current task id nicht 0 -> actions prüfen (noch nicht ausgeführt aber schon über die zeit, dann ausführen)
                        foreach (var action in CurrentTask.Actions) {
                            if (action.Started == false && RealTimeSinceTaskStart > action.SecondsFromStart) { //TODO: hier ohne unscaled?
                                HandleAction(action);
                                action.Started = true;
                            }
                        }
                    }
                }
            }
        }

        //TODO: ist das so okay?
        public static void HandleAction(AssignmentAction action) {
            AssignmentActionType receivedKey = action.Type;
            var number = (int)receivedKey;

            //from 1 - 6: call HandleControl
            if (number >= 1 && number <= 99) {
                HandleControl(receivedKey);
            //from 1000 - 1008: call HandleEvent
            } else if (number >= 1000 && number <= 1008) {
                HandleEvent(receivedKey);
            }  
        }

        //Methode HandleEvent (von drüben hierhier kopieren)
        private static void HandleEvent(AssignmentActionType type) {
            if (!EventTypeCustomEventPairs.TryGetValue(type, out CustomStaticEvent customEvent)) {
                customEvent = RevolveEvent(type);
                customEvent?.TryTrigger();

                //only add to list when not null and not onetimevent, i.e., a Fine or Grant will not be added to the list.
                if (customEvent != null && !customEvent.OneTimeEvent) {
                    EventTypeCustomEventPairs.Add(type, customEvent);
                }
            } else {
                StopEvent(customEvent);
                EventTypeCustomEventPairs.Remove(type);
            }
        }

        //Method HandleControl
        private static void HandleControl(AssignmentActionType type) {
            //do something
        }

        private static CustomStaticEvent RevolveEvent(AssignmentActionType type) {
            switch (type) {
                case AssignmentActionType.ResearchSpeed: return CustomStaticEvent.CreateResearchSpeedEvent(100);
                case AssignmentActionType.PollutionFine: return CustomStaticEvent.CreatePollutionFineEvent(100_000);
                case AssignmentActionType.Grant: return CustomStaticEvent.CreateGrantEvent(100_000);
                case AssignmentActionType.Fine: return CustomStaticEvent.CreateFineEvent(100_000);
                case AssignmentActionType.Upkeep: return CustomStaticEvent.CreateUpkeepEvent(50);
                case AssignmentActionType.TrainShipNetworkSpeed: return CustomStaticEvent.CreateNetworkSpeedEvent(500);
                case AssignmentActionType.TrainShipDispatchCost: return CustomStaticEvent.CreateDispactCostEvent(500);
                case AssignmentActionType.Demand: return CustomStaticEvent.CreateDemandEvent(50);
                case AssignmentActionType.BuildingCost: return CustomStaticEvent.CreateBuildingCostEvent(50);
                default: return null;
            }
        }

        private static void StopEvent(CustomStaticEvent customEvent) {

            foreach (IWorldEventAgent worldEventAgent in ROIDataMod.GetField<List<IWorldEventAgent>>(typeof(WorldEventManager),
                "_worldEventAgents", ManagerBehaviour<WorldEventManager>.instance)) {
                foreach (StaticWorldEvent item in new List<StaticWorldEvent>(worldEventAgent.GetActiveStaticEvents())) {
                    if (item.data.eventName == customEvent.Name) {
                        worldEventAgent.EndEvent(item);
                    }
                }
            }
        }

        //TODO: Wie baue ich TryActivateEvent ein?
        private static bool TryActivateEvent(out WorldEventManager wem, out WorldEventAgent wea) {
            wea = null;
            wem = ManagerBehaviour<WorldEventManager>.instance;

            //TODO: Refactor
            if (ROIDataMod.Player == null) {
                return false;
            }

            if (wem == null) {
                return false;
            }

            wea = ROIDataMod.Player.GetComponent<WorldEventAgent>();

            if (wea == null) {
                return false;
            }

            var staticEvents = ROIDataMod.GetField<StaticWorldEventsTrigger>(typeof(WorldEventManager), "_staticEvents", wem);

            if (staticEvents == null) {
                return false;
            }

            if (staticEvents.eventData == null) {
                return false;
            }

            //ActivateEvents();
            //SettlementNameChanger.RenameAll();

            return true;
        }
    }
}
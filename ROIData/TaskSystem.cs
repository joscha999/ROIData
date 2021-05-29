using ProjectAutomata;
using ROIData.EventParams;
using ROIData.HelperClasses;
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
        //CurrentTaskID field (aktuell laufenden aufgabe)
        private static AssignmentTask CurrentTask;
        //Übergeordnete Liste an Aufgaben
        private static Dictionary<int, AssignmentTask> AllTasks = new Dictionary<int, AssignmentTask>();

        private static Dictionary<AssignmentActionType, CustomStaticEvent> EventTypeCustomEventPairs
            = new Dictionary<AssignmentActionType, CustomStaticEvent>();

        private static TimeManager TimeManager = ManagerBehaviour<TimeManager>.instance;

        public static void ReceiveTasks(List<AssignmentTask> tasks)
        {
            foreach (var task in tasks)
            {
                if (AllTasks.TryGetValue(task.ID, out var existingTask))
                {
                    UpdateTask(existingTask, task);
                } else
                {
                    AllTasks.Add(task.ID, task);
                }
            }
        }

        private static void UpdateTask(AssignmentTask existing, AssignmentTask received)
        {
            foreach (var item in received.Actions)
            {
                if (!existing.Actions.Any(e => e.ID == item.ID))
                {
                    existing.Actions.Add(item);
                }
            }
        }

        //TODO: Methode aufrufen. Wo? Logik so korrekt?
        public static void Update() {
            UpdateTasks();
            UpdateActions();
        }

        private static void UpdateTasks()
        {
            foreach (var task in AllTasks.Values)
            {
                //wurde die utc starttime überschritten wenn kein task läuft -> task starten (id setzen, bzw task speichern)
                if (!task.Started && DateTimeOffset.UtcNow >= task.UTCStart)
                {
                    //Task starten
                    CurrentTask = task;
                    task.Started = true;
                    Debug.Log($"Started task: {task.Name}, UTC Time: {DateTime.UtcNow}.");
                    task.UnscaledTimeStart = Time.realtimeSinceStartup;
                }
            }
        }

        private static void UpdateActions()
        {
            if (CurrentTask == null) 
                return;

            //wenn current task id nicht 0 -> actions prüfen (noch nicht ausgeführt aber schon über die zeit, dann ausführen)
            foreach (var action in CurrentTask.Actions)
            {
                if (!action.Started && CurrentTask.TimeSinceStart >= action.SecondsFromStart)
                { //TODO: hier ohne unscaled?
                    HandleAction(action);
                    action.Started = true;
                }
            }
        }

        //TODO: ist das so okay?
        public static void HandleAction(AssignmentAction action) {
            if (action.Type > AssignmentActionType.None
                && action.Type < AssignmentActionType.END_CONTROL_BLOCK)
            {
                HandleControl(action.Type, action.Value);
            }
            else if (action.Type >= AssignmentActionType.ResearchSpeed
                && action.Type < AssignmentActionType.END_EVENT_BLOCK)
            {
                HandleEvent(action.Type, action.Value);
            }
        }

        //Methode HandleEvent (von drüben hierhier kopieren)
        private static void HandleEvent(AssignmentActionType type, string value) {
            if (!EventTypeCustomEventPairs.TryGetValue(type, out CustomStaticEvent customEvent)) {
                customEvent = ResolveEvent(type, value);
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
        private static void HandleControl(AssignmentActionType type, string value) {
            switch (type)
            {
                case AssignmentActionType.None:
                    break;
                case AssignmentActionType.TaskStart:
                    break;
                case AssignmentActionType.TaskEnd:
                    break;
                case AssignmentActionType.Pause:
					PauseGame();
                    break;
                case AssignmentActionType.Unpause:
					UnpauseGame();
                    break;
                case AssignmentActionType.CreateSave:
					TryForceAutoSave(value);
                    break;
                case AssignmentActionType.DisplayMessage:
                    HandleEvent(type, value);
                    break;
				case AssignmentActionType.Research:
					Research(new ResearchControlParameters(value));
					break;
            }
        }

        private static CustomStaticEvent ResolveEvent(AssignmentActionType type, string value) {
            switch (type) {
                case AssignmentActionType.ResearchSpeed: return CustomStaticEvent.CreateResearchSpeedEvent(int.Parse(value));
                case AssignmentActionType.PollutionFine: return CustomStaticEvent.CreatePollutionFineEvent(int.Parse(value));
                case AssignmentActionType.Grant: return CustomStaticEvent.CreateGrantEvent(new IntStringEventParameters(value));
				//TODO: IntStringEventParameter? (Grant, Fine)
				case AssignmentActionType.Fine: return CustomStaticEvent.CreateFineEvent(new IntStringEventParameters(value));
                case AssignmentActionType.Upkeep: return CustomStaticEvent.CreateUpkeepEvent(int.Parse(value));
                case AssignmentActionType.TrainShipNetworkSpeed: return CustomStaticEvent.CreateNetworkSpeedEvent(int.Parse(value));
                case AssignmentActionType.TrainShipDispatchCost: return CustomStaticEvent.CreateDispactCostEvent(int.Parse(value));
                case AssignmentActionType.Demand: return CustomStaticEvent.CreateDemandEvent(new IntProductEventParameters(value));
                case AssignmentActionType.BuildingCost: return CustomStaticEvent.CreateBuildingCostEvent(int.Parse(value));
                case AssignmentActionType.DisplayMessage: return CustomStaticEvent.CreateMessageEvent(new MessageEventParameters(value));
                default: return null;
            }
        }

        private static void StopEvent(CustomStaticEvent customEvent) {
            foreach (IWorldEventAgent worldEventAgent in Reflection.GetField<List<IWorldEventAgent>>(typeof(WorldEventManager),
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

            var staticEvents = Reflection.GetField<StaticWorldEventsTrigger>(typeof(WorldEventManager), "_staticEvents", wem);

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

        private static void TryForceAutoSave(string name) {
            try {
                SavegameStorage.SaveSavegame(name, ManagerBehaviour<SavegameManager>
                    .instance.CreateSavegame(), SavegameHeader.Create(name));
            } catch (Exception exception) {
                Debug.LogError("Savegame exception on autosave!");
                Debug.LogException(exception);
            }
        }

        private static void UnpauseGame() {
            TimeManager.canAdvanceTime = true;
            TimeManager.canPauseTime = false;
        }

        private static void PauseGame() {
            TimeManager.canAdvanceTime = false;
        }

		public static void Research(ResearchControlParameters parameters) {
			if (parameters.Unlock == null) {
				Debug.Log("ERROR: TaskSystem.Research: Unlock is null!");
				return;
			}

			Debug.Log("Unlocking " + parameters.Unlock.name);
			ResearchRecursive(parameters.Unlock);
		}

		private static void ResearchRecursive(TechTreeUnlock unlock) {
			foreach (var u in unlock.requiredUnlocks)
				ResearchRecursive(u);

			foreach (var i in unlock.includedUnlocks)
				ResearchRecursive(i);

			ROIDataMod.Player.techTree.Unlock(unlock);
		}
    }
}
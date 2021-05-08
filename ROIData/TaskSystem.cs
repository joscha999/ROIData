using ROIData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData
{
    public static class TaskSystem
    {
        //RealTimeSinceTaskStart Field (float)
        //CurrentTaskID field (aktuell laufenden aufgabe)
        //Übergeordnete Liste an Aufgaben
        public static void ReceiveTasks(List<AssignmentTask> tasks)
        {
            //Liste mit Daten füllen (ist da ein neuer task drin)
            //ist in der task selbst eine neue action enthalten? ist die action id bereits vorhanden?
        }

        //Methode Update
        //- realtimesincetaskstart += time.unscaleddeltatime (um sachen in der aufgabe zeitlich zu steuern)
        //wurde die utc starttime überschritten wenn kein task läuft -> task starten (id setzen, bzw task speichern)
        //wenn current task id nicht 0 -> actions prüfen (noch nicht ausgeführt aber schon über die zeit, dann ausführen)

        //Methode HandleAction

        //Methode HandleEvent (von drüben hierhier kopieren)

        //Methode HandleControl


    }
}

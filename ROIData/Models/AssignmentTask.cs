using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ROIData.Models
{
	public class AssignmentTask
	{
        public int ID { get; set; }
        public bool Started { get; set; }
        public float UnscaledTimeStart { get; set; }
        public string Name { get; set; }
		public DateTimeOffset UTCStart { get; set; }
		public List<AssignmentAction> Actions { get; set; }

		public float TimeSinceStart => Time.realtimeSinceStartup - UnscaledTimeStart;
	}
}
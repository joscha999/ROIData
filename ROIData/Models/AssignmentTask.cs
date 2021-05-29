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

		public bool CanBeStartedNow => DateTimeOffset.UtcNow >= UTCStart
			&& !Started && UnixDayEnd > Date.Now.UnixDays;

		private int? _unixDayEnd;

		public int UnixDayEnd {
			get {
				if (_unixDayEnd != null)
					return _unixDayEnd.Value;

				foreach (var act in Actions) {
					if (act.Type == AssignmentActionType.TaskEnd) {
						_unixDayEnd = int.Parse(act.Value);
						break;
					}
				}

				return _unixDayEnd.Value;
			}
		}
	}
}
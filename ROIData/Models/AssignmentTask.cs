using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.Models
{
	public class AssignmentTask
	{
        public int ID { get; set; }
        public bool Started { get; set; }
        public string Name { get; set; }
		public DateTimeOffset UTCStart { get; set; }
		public IEnumerable<AssignmentAction> Actions { get; set; }
	}
}
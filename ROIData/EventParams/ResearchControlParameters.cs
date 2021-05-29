using ProjectAutomata;
using ROIData.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.EventParams {
	public class ResearchControlParameters : EventParameters {
		private IEnumerable<TechTreeUnlock> Unlockables => Reflection.GetField<Dictionary<TechTreeUnlock, bool>>
			(typeof(TechTreeAgent), "_unlockStates", ROIDataMod.Player.techTree).Keys;

		public TechTreeUnlock Unlock { get; private set; }

		public ResearchControlParameters(string value) : base(value) { }

		protected override void Parse() {
			if (TryParseTechTreeUnlock(Data[0], out var unlock)) {
				Unlock = unlock;
			}
		}

		private bool TryParseTechTreeUnlock(string value, out TechTreeUnlock unlock) {
			unlock = null;
			if (value == null)
				return false;

			foreach (var u in Unlockables) {
				if (u.name != value)
					continue;

				unlock = u;
				return true;
			}

			return false;
		}
	}
}
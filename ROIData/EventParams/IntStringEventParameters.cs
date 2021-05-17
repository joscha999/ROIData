using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.EventParams {
    public class IntStringEventParameters : EventParameters {

        public int Amount { get; set; }
        public string Message { get; set; }

        public IntStringEventParameters(string value) : base(value) {
        }

        protected override void Parse() {
            Amount = int.Parse(Data[0]);
            Message = Data[1];
        }
    }
}

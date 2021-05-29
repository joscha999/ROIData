using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.EventParams {
    public class MessageEventParameters : EventParameters {
        public string Title { get; set; }
        public string Description { get; set; }

        public MessageEventParameters(string value) : base(value) { }

        protected override void Parse() {
            Title = Data[0];
            Description = Data[1];
        }
    }
}

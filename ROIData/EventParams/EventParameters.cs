using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROIData.EventParams
{
    public abstract class EventParameters
    {
        protected Dictionary<int, string> Data = new Dictionary<int, string>();

        public EventParameters(string value) => Parse(value);

        private void Parse(string value)
        {
            string[] subStrings = value.Split(',');

            for (int i = 0; i < subStrings.Length; i++)
            {
                Data.Add(i, subStrings[i]);
            }

            Parse();
        }

        protected abstract void Parse();
    }
}

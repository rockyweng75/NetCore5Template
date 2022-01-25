using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore5_Domain
{
    public class KeyValue
    {
        public KeyValue() { }

        public KeyValue(string Key, object Value)
        {
            this.Key = Key;
            this.Value = Value;
        }


        public string Key { get; set; }

        public object Value { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore5_Domain
{
    public class Config
    {
        public const string Mode = "Config";

        public string Type { get; set; }
        public string SecurityKey { get; set; }
        public string AppId { get; set; }

    }
}

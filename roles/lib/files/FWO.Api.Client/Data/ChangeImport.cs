using System;
using System.Collections.Generic;
using System.Linq;
 
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FWO.Api.Data
{
    public class ChangeImport
    {
        [JsonProperty("time")]
        public DateTime Time { get; set; }

        public ChangeImport() {}
        
        public ChangeImport(ChangeImport changeImport)
        {
            Time = changeImport.Time;
        }
    }
}

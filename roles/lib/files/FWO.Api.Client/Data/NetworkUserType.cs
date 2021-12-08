﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization; 
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FWO.Api.Data
{
    public class NetworkUserType
    {
        [JsonProperty("usr_typ_name"), JsonPropertyName("usr_typ_name")]
        public string Name { get; set; }
    }
}

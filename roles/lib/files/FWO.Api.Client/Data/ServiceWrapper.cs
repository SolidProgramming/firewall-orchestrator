﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization; 
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FWO.Api.Data
{
    public class ServiceWrapper
    {
        [JsonProperty("service"), JsonPropertyName("service")]
        public NetworkService Content { get; set; }
    }
    // public class ServiceObjectRecursiveWrapper
    // {
    //     [JsonProperty("service"), JsonPropertyName("service")]
    //     public ServiceObjectRecursiveFlatsWrapper Content { get; set; }
    // }
}

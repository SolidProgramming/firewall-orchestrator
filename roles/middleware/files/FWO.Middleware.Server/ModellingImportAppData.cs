 
using Newtonsoft.Json;


namespace FWO.Middleware.Server
{
    /// <summary>
    /// Structure for imported owner data 
    /// </summary>
    public class ModellingImportOwnerData
    {
        /// <summary>
        /// List of all Owners
        /// </summary>
        [JsonProperty("owners")]
        public List<ModellingImportAppData>? Owners { get; set; }
    }

    /// <summary>
    /// Structure for imported app data 
    /// </summary>
    public class ModellingImportAppData
    {
        /// <summary>
        /// App Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// External Id of App
        /// </summary>
        [JsonProperty("app_id_external")]
        public string ExtAppId { get; set; } = "";

        /// <summary>
        /// Main User (Dn)
        /// </summary>
        [JsonProperty("main_user")]
        public string? MainUser { get; set; } = "";

        /// <summary>
        /// List of allowed modellers (Dn)
        /// </summary>
        [JsonProperty("modellers")]
        public List<string>? Modellers { get; set; } = [];

        /// <summary>
        /// List of Ldap Groups of allowed modellers (Dn): (currently handled same as modellers)
        /// </summary>
        [JsonProperty("modeller_groups")]
        public List<string>? ModellerGroups { get; set; } = [];

        /// <summary>
        /// Criticality of App
        /// </summary>
        [JsonProperty("criticality")]
        public string? Criticality { get; set; }

        /// <summary>
        /// Source of App import
        /// </summary>
        [JsonProperty("import_source")]
        public string ImportSource { get; set; } = "";

        /// <summary>
        /// App Servers of App
        /// </summary>
        [JsonProperty("app_servers")]
        public List<ModellingImportAppServer> AppServers { get; set; } = [];
    }
    
    /// <summary>
    /// Structure for imported app server 
    /// </summary>
    public class ModellingImportAppServer
    {
        /// <summary>
        /// App Server Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        // /// <summary>
        // /// App Server Subnet
        // /// </summary>
        // [JsonProperty("subnet")]
        // public string Subnet { get; set; } = "";

        /// <summary>
        /// App Server Ip
        /// </summary>
        [JsonProperty("ip")]
        public string Ip { get; set; } = "";

        /// <summary>
        /// App Server IpEnd
        /// </summary>
        [JsonProperty("ip_end")]
        public string IpEnd { get; set; } = "";
    }
}

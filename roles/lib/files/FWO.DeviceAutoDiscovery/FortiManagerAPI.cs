using RestSharp;
using System.Text.Json;
using FWO.Basics;
using FWO.Api.Data;
using System.Text;

using System.Text.Encodings.Web;
using Newtonsoft.Json;
using FWO.Logging;
using RestSharp.Serializers.NewtonsoftJson;
using RestSharp.Serializers;

namespace FWO.Rest.Client
{
    public class FortiManagerClient
    {
        readonly RestClient restClient;

        public FortiManagerClient(Management fortiManager)
        {
            RestClientOptions restClientOptions = new RestClientOptions();
            restClientOptions.RemoteCertificateValidationCallback += (_, _, _, _) => true;
            restClientOptions.BaseUrl = new Uri("https://" + fortiManager.Hostname + ":" + fortiManager.Port + "/jsonrpc");
            restClient = new RestClient(restClientOptions, null, ConfigureRestClientSerialization);
        }

        private void ConfigureRestClientSerialization(SerializerConfig config)
        {
            JsonNetSerializer serializer = new JsonNetSerializer(); // Case insensivitive is enabled by default
            config.UseSerializer(() => serializer);
        }


        public async Task<RestResponse<SessionAuthInfo>> AuthenticateUser(string? user, string pwd)
        {
            List<object> dataList = new List<object>();
            dataList.Add(new { passwd = pwd, user = user });

            List<object> paramList = new List<object>();
            paramList.Add(new { data = dataList, url = "/sys/login/user" });

            var body = new
            {
                method = "exec",
                id = 1,
                @params = paramList // because "params" is a c# keyword, we have to escape it here with @
            };
            RestRequest request = new RestRequest("", Method.Post);
            request.AddJsonBody(body);
            return await restClient.ExecuteAsync<SessionAuthInfo>(request);
        }

        public async Task<RestResponse<SessionAuthInfo>> DeAuthenticateUser(string session)
        {
            List<object> paramList = new List<object>();
            paramList.Add(new { session = session, url = "/sys/logout" });

            var body = new
            {
                method = "exec",
                id = 1,
                @params = paramList // because "params" is a c# keyword, we have to escape it here with @
            };
            RestRequest request = new RestRequest("", Method.Post);
            request.AddJsonBody(body);
            return await restClient.ExecuteAsync<SessionAuthInfo>(request);
        }

        public async Task<RestResponse<FmApiTopLevelHelper>> GetAdoms(string session)
        {
            string[] fieldArray = { "name", "oid", "uuid" };
            List<object> paramList = new List<object>();
            paramList.Add(new { fields = fieldArray, url = "/dvmdb/adom" });

            var body = new
            {
                @params = paramList,
                method = "get",
                id = 1,
                session = session
            };
            RestRequest request = new RestRequest("", Method.Post);
            request.AddJsonBody(body);
            Log.WriteDebug("Autodiscovery", $"using FortiManager REST API call with body='{body.ToString()}' and paramList='{paramList.ToString()}'");
            return await restClient.ExecuteAsync<FmApiTopLevelHelper>(request);
        }

        public async Task<RestResponse<FmApiTopLevelHelperDev>> GetDevices(string session)
        {
            string[] fieldArray = { "name", "desc", "hostname", "vdom", "ip", "mgmt_id", "mgt_vdom", "os_type", "os_ver", "platform_str", "dev_status" };
            List<object> paramList = new List<object>();
            paramList.Add(new { fields = fieldArray, url = "/dvmdb/device" });

            var body = new
            {
                @params = paramList,
                method = "get",
                id = 1,
                session = session
            };
            RestRequest request = new RestRequest("", Method.Post);
            request.AddJsonBody(body);
            return await restClient.ExecuteAsync<FmApiTopLevelHelperDev>(request);
        }

        public async Task<RestResponse<FmApiTopLevelHelperAssign>> GetPackageAssignmentsPerAdom(string session, string adomName)
        {
            List<object> paramList = new List<object>();
            string urlString = "/pm/config/";

            if (adomName=="global")
                urlString += "global/_package/status";
            else
                urlString += "adom/" + adomName + "/_package/status";
            paramList.Add(new { url = urlString });

            var body = new
            {
                @params = paramList,
                method = "get",
                id = 1,
                session = session
            };
            RestRequest request = new RestRequest("", Method.Post);
            request.AddJsonBody(body);
            return await restClient.ExecuteAsync<FmApiTopLevelHelperAssign>(request);
        }
    }

    public class SessionAuthInfo
    {
        [JsonProperty("session")]
        public string SessionId { get; set; } = "";
    }

    public class FmApiStatus
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; } = "";
    }

///////////////////////////////////////////////////////////////////////////////////////////////////
    public class FmApiTopLevelHelper
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public FmApiStatus Status { get; set; } = new FmApiStatus();

        [JsonProperty("result")]
        public List<FmApiDataHelper> Result { get; set; } = new List<FmApiDataHelper>();
    }

    public class FmApiDataHelper
    {
        [JsonProperty("data")]
        public List<Adom> AdomList { get; set; } = new List<Adom>();
    }

    public class Adom
    {
        [JsonProperty("oid")]
        public int Oid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("uuid")]
        public string Uid { get; set; } = "";

        // public List<Package> Packages = new List<Package>();
        public List<Assignment> Assignments = new List<Assignment>();
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    public class FmApiTopLevelHelperDev
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public FmApiStatus Status { get; set; } = new FmApiStatus();

        [JsonProperty("result")]
        public List<FmApiDataHelperDev> Result { get; set; } = new List<FmApiDataHelperDev>();
    }

    public class FmApiDataHelperDev
    {
        [JsonProperty("data")]
        public List<FortiGate> DeviceList { get; set; } = new List<FortiGate>();
    }
    public class FortiGate
    {
        [JsonProperty("oid")]
        public int Oid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("hostname")]
        public string Hostname { get; set; } = "";

        // [JsonProperty("ip")]
        // public string Ip { get; set; } = "";

        [JsonProperty("mgt_vdom")]
        public string MgtVdom { get; set; } = "";
        
        // [JsonProperty("os_ver")]
        // public string OsVer { get; set; } = "";
        
        // [JsonProperty("dev_status")]
        // public string DevStatus { get; set; } = "";
        
        [JsonProperty("vdom")]
        public List<Vdom> VdomList { get; set; } = new List<Vdom>();

        // "name", "desc", "hostname", "vdom", "ip", "mgmt_id", "mgt_vdom", "os_type", "os_ver", "platform_str", "dev_status" 
    }
   public class Vdom
    {
        [JsonProperty("oid")]
        public int Oid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";
    }

///////////////////////////////////////////////////////////////////////////////////////////////////

    public class FmApiTopLevelHelperAssign
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public FmApiStatus Status { get; set; } = new FmApiStatus();

        [JsonProperty("result")]
        public List<FmApiDataHelperAssign> Result { get; set; } = new List<FmApiDataHelperAssign>();
    }

    public class FmApiDataHelperAssign
    {
        [JsonProperty("data")]
        public List<Assignment> AssignmentList { get; set; } = new List<Assignment>();
    }
    public class Assignment
    {
        [JsonProperty("oid")]
        public int Oid { get; set; }

        [JsonProperty("dev")]
        public string DeviceName { get; set; } = "";

        [JsonProperty("vdom")]
        public string VdomName { get; set; } = "";

        [JsonProperty("pkg")]
        public string PackageName { get; set; } = "";
    }
}

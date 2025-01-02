using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FWO.Api.Data
{
    public class ImportCredential
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("credential_name")]
        public string Name { get; set; } = "";

        [JsonProperty("is_key_pair")]
        public Boolean IsKeyPair { get; set; } = false;

        [JsonProperty("user")]
        public string? ImportUser { get; set; }

        [JsonProperty("secret")]
        public string Secret { get; set; } = "";

        [JsonProperty("sshPublicKey")]
        public string? PublicKey { get; set; }

        [JsonProperty("cloud_client_id")]
        public string? CloudClientId { get; set; }

        [JsonProperty("cloud_client_secret")]
        public string? CloudClientSecret { get; set; }

        public ImportCredential()
        {}

        public ImportCredential(ImportCredential cred)
        {
            Id = cred.Id;
            Name = cred.Name;
            IsKeyPair = cred.IsKeyPair;
            ImportUser = cred.ImportUser;
            Secret = cred.Secret;
            PublicKey = cred.PublicKey;
            CloudClientId = cred.CloudClientId;
            CloudClientSecret = cred.CloudClientSecret;
        }
        public ImportCredential(string username, string password)
        {
            ImportUser = username;
            Secret = password;
        }
        public bool Sanitize()
        {
            bool shortened = false;
            Name = Sanitizer.SanitizeMand(Name, ref shortened);
            ImportUser = Sanitizer.SanitizeOpt(ImportUser, ref shortened);
            PublicKey = Sanitizer.SanitizeKeyOpt(PublicKey, ref shortened);
            Secret = Sanitizer.SanitizeKeyMand(Secret, ref shortened);
            CloudClientId = Sanitizer.SanitizeOpt(CloudClientId, ref shortened);
            CloudClientSecret = Sanitizer.SanitizeKeyOpt(CloudClientSecret, ref shortened);
            return shortened;
        }
    }
}

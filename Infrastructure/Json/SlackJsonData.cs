using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace KintaiSystem.Infrastructure.Json
{
    public class SlackJsonData
    {
        [JsonPropertyName("ok")]
        public bool Result { get; set; }

        [JsonPropertyName("members")]
        public List<MembersData> Members { get; set; }
    }

    public class MembersData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("profile")]
        public ProfileData Profile { get; set; }
    }

    public class ProfileData
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}

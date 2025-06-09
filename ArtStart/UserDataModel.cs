using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArtStart.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsAuthenticated { get; set; }
    }

    public class UserDataModel
    {
        [JsonProperty("users")]
        public List<User> Users { get; set; } = new List<User>();
    }
}

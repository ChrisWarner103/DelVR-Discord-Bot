using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DelVRBot
{
    public struct ConfigJson
    {
        [JsonProperty("discordRoles")]
        public JArray roles;

        [JsonProperty("")]
        public JArray emojis;

        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }

        [JsonProperty("reactionRolesMesage")]
        public ulong ReactionMessage { get; private set; }
    }
}

using System;
using System.Net;
using System.Text.Json.Serialization;

namespace SanAndreasUnityMasterServer.Models
{
    public class ServerListing
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public int NumPlayersOnline { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public int MaxPlayers { get; set; }
        [JsonIgnore]
        public DateTime LastUpdate { get; set; }
    }
}
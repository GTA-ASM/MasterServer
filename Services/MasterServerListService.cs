using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SanAndreasUnityMasterServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace SanAndreasUnityMasterServer.Services
{
    public class MasterServerListService
    {
        private List<ServerListing> _servers;
        private ILogger<MasterServerListService> _logger;
        private IConfiguration _config;

        public MasterServerListService(ILogger<MasterServerListService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _servers = new List<ServerListing>();
            _config = configuration;

            Timer timer = new Timer(5000)
            {
                AutoReset = true
            };
            timer.Elapsed += (obj, args) => _servers.RemoveAll(x => (DateTime.Now - x.LastUpdate) >= TimeSpan.FromSeconds(_config.GetValue<double>("MaxLastUpdateTime")));

            timer.Start();
        }

        public void AddServer(ServerListing serverListing)
        {
            serverListing.LastUpdate = DateTime.Now;

            int existingListingIndex = _servers.FindIndex(x => x.IP == serverListing.IP && x.Port == serverListing.Port);

            if (existingListingIndex == -1)
            {
                _servers.Add(serverListing);
                _logger.LogInformation($"New server registered {serverListing.Name} {serverListing.IP}:{serverListing.Port}");
            }
            else
            {
                _servers[existingListingIndex] = serverListing;
                _logger.LogInformation($"Server updated {serverListing.Name} {serverListing.IP}:{serverListing.Port}");
            }
        }

        public bool RemoveServer(ServerListing serverListing)
        {
            int serverIndex = _servers.FindIndex(x => x.IP == serverListing.IP && x.Port == serverListing.Port);

            if (serverIndex != -1)
            {
                _servers.RemoveAt(serverIndex);
                _logger.LogInformation($"Server {serverListing.Name} {serverListing.IP}:{serverListing.Port} removed");
                return true;
            }
            return false;
        }

        public object GetAllServerListings() => _servers.Select(x => new { x.Name, x.IP, x.Port, x.NumPlayersOnline, x.MaxPlayers });
    }
}
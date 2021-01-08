using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SanAndreasUnityMasterServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            timer.Elapsed += (_, _) =>
            {
                for (int i = 0; i < _servers.Count; i++)
                {
                    ServerListing server = _servers[i];
                    if ((DateTime.Now - server.LastUpdate) >= TimeSpan.FromSeconds(_config.GetValue<double>("MaxLastUpdateTime")))
                    {
                        RemoveServer(server.Id);
                    }
                }
            };
            timer.Start();
        }

        public void AddServer(ServerListing serverListing)
        {
            serverListing.Id = Guid.NewGuid();
            serverListing.LastUpdate = DateTime.Now;

            _servers.Add(serverListing);
            _logger.LogInformation($"New server registered {serverListing.Name} {serverListing.IP}:{serverListing.Port}");
        }

        public bool UpdateServer(ServerListing serverListing)
        {
            int serverIndex = _servers.FindIndex(x => x.Id == serverListing.Id);
            if (serverIndex != -1)
            {

                _servers[serverIndex] = serverListing;
                _servers[serverIndex].LastUpdate = DateTime.Now;
                return true;

            }
            return false;
        }

        public bool RemoveServer(Guid id)
        {
            int serverIndex = _servers.FindIndex(x => x.Id == id);

            if (serverIndex != -1)
            {
                _servers.RemoveAt(_servers.FindIndex(x => x.Id == id));
                _logger.LogInformation($"Server {id} removed");
                return true;
            }
            return false;
        }

        public object GetAllServerListings() => _servers.Select(x => new { x.Name, x.IP, x.Port, x.NumPlayersOnline, x.MaxPlayers });
    }
}
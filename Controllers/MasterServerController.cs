using Microsoft.AspNetCore.Mvc;
using SanAndreasUnityMasterServer.Models;
using SanAndreasUnityMasterServer.Services;

namespace SanAndreasUnityMasterServer.Controllers
{
    [ApiController]
    [Route("")]
    public class MasterServerController : ControllerBase
    {
        private MasterServerListService _masterServerListService;

        public MasterServerController(MasterServerListService masterServerListService)
        {
            _masterServerListService = masterServerListService;
        }

        [HttpPost("/register")]
        public IActionResult RegisterServer([FromBody] ServerListing serverListing)
        {
            if (string.IsNullOrEmpty(serverListing.Name) || string.IsNullOrEmpty(serverListing.IP))
            {
                return BadRequest("Invaild server details");
            }

            _masterServerListService.AddServer(serverListing);

            return Ok();
        }

        [HttpPost("/unregister")]
        public IActionResult UnregisterServer([FromBody] ServerListing serverListing)
        {
            return _masterServerListService.RemoveServer(serverListing) ? Ok("Removed the server.") : BadRequest("Failed to remove the server");
        }

        [HttpGet]
        public IActionResult GetAllServers()
        {
            return Ok(_masterServerListService.GetAllServerListings());
        }
    }
}
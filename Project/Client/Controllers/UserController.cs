using Client.Extensions;
using Common.DTO;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.ComponentModel.DataAnnotations;

namespace Client.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Register(UserAuthDto model)
        {
            var result = await ServiceProxy.Create<IValidator>(new Uri("fabric:/Project/Validator")).RegisterAsync(model);
            return StatusCodeCheck.HandleStatusCode(result);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserAuthDto model)
        {
            var result = await ServiceProxy.Create<IValidator>(new Uri("fabric:/Project/Validator")).LoginAsync(model);
            return StatusCodeCheck.HandleStatusCode(result);
        }
    }
}

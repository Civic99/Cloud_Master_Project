using Client.Extensions;
using Common.DTO;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Client.Controllers
{
    [Route("api/[controller]/[action]")]
    public class OrderController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]OrderDto model)
        {
            var result = await ServiceProxy.Create<IValidator>(new Uri("fabric:/Project/Validator")).CreateOrderAsync(model);
            return StatusCodeCheck.HandleStatusCode(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromRoute]Guid userId)
        {
            var result = await ServiceProxy.Create<IValidator>(new Uri("fabric:/Project/Validator")).GetAllOrderAsync(userId);
            return Ok(result);
        }
    }
}

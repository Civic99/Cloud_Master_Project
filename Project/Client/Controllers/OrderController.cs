using Client.Extensions;
using Common.DTO;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Client.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]OrderDto model)
        {
            var result = await ServiceProxy.Create<IValidator>(new Uri("fabric:/Project/Validator")).CreateOrderAsync(model);
            return StatusCodeCheck.HandleStatusCode(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromHeader]Guid userId)
        {
            var result = await ServiceProxy.Create<IValidator>(new Uri("fabric:/Project/Validator")).GetAllOrderAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Pay([FromHeader] Guid orderId, [FromHeader] OrderType orderType)
        {
            var result = await ServiceProxy.Create<IValidator>(new Uri("fabric:/Project/Validator")).Pay(orderId, orderType);
            return StatusCodeCheck.HandleStatusCode(result);
        }
    }
}

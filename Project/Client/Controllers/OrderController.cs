using Client.Extensions;
using Common;
using Common.DTO;
using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Client.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public OrderController(IHubContext<NotificationHub> hubContext) { _hubContext = hubContext; }

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
            Thread.Sleep(1000);
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Order is paid");
            return StatusCodeCheck.HandleStatusCode(result);
        }
    }
}

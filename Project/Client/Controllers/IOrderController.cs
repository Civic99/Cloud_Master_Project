using Common.DTO;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public interface IOrderController
    {
        Task<IActionResult> Create([FromBody] OrderDto model);
        Task<IActionResult> GetAll([FromHeader] Guid userId);
        Task<IActionResult> Pay([FromHeader] Guid orderId, [FromHeader] OrderType orderType);
    }
}
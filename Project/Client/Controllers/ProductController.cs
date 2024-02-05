using Client.Extensions;
using Common.DTO;
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Client.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ProductController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await ServiceProxy.Create<IValidator>(new Uri("fabric:/Project/Validator")).GetAllProductsAsync();
            return Ok(result);
        }
    }
}

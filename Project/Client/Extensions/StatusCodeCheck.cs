using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Extensions
{
    public static class StatusCodeCheck
    {
        public static IActionResult HandleStatusCode(StatusCode statusCode)
        {
            return statusCode switch
            {
                StatusCode.Success => new OkObjectResult("All good"),
                StatusCode.Conflict => new ConflictObjectResult("There is already an item"),
                StatusCode.Unoathorized => new UnauthorizedObjectResult("Unauthorized"),
                StatusCode.Forbidden => new ObjectResult(403),
                StatusCode.NotFound => new NotFoundObjectResult("Not found"),
                StatusCode.BadRequest => new BadRequestObjectResult("Bad request"),
                _ => new BadRequestObjectResult("Something went wrong")
            }; 
        }
    }
}

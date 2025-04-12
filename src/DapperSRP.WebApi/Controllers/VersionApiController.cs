using Microsoft.AspNetCore.Mvc;

namespace DapperSRP.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class VersionApiController : ControllerBase
    {
    }
}

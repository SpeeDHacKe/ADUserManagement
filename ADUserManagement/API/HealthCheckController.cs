using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADUserManagement.API
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet(Name = "APIHealthCheck")]
        public IActionResult APIHealthCheck()
        {
            try
            {
                return Ok(new { status = 200, success = true, message = $"Connect {AppSetting.AssemblyName} Success!" });
            }
            catch (Exception ex)
            {
                return NotFound(new { status = 500, success = false, message = $"{ex.Message} | Inner: {ex.InnerException?.Message}" });
            }
        }
    }
}

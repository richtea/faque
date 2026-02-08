using Microsoft.AspNetCore.Mvc;

namespace HookNorton.Controllers;

/// <summary>
/// Controller for health check endpoints.
/// </summary>
[ApiController]
[Route("$$/api/health")]
[Produces("application/json")]
[EndpointGroupName("v1")]
[Tags("Health")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Returns the health status of the service.
    /// </summary>
    /// <returns>A health status object.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTimeOffset.UtcNow,
        });
    }
}

using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using Serilog.Events;

namespace PoemApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoggingController : ControllerBase
{
    private readonly LoggingLevelSwitch _levelSwitch;

    public LoggingController(LoggingLevelSwitch levelSwitch)
    {
        _levelSwitch = levelSwitch;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Level = _levelSwitch.MinimumLevel.ToString() });
    }

    [HttpPost("level")]
    public IActionResult Set([FromQuery] string level)
    {
        if (!Enum.TryParse<LogEventLevel>(level, true, out var parsed))
            return BadRequest("Invalid level");

        _levelSwitch.MinimumLevel = parsed;
        return Ok(new { Level = _levelSwitch.MinimumLevel.ToString() });
    }
}

using EstatePulse.Common.Caching;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Common.Controllers;

[ApiController]
[Route("api/debug/cache")]
public class DebugCacheController : ControllerBase
{
    private readonly ICacheService _cacheService;
    private readonly IWebHostEnvironment _env;

    public DebugCacheController(ICacheService cacheService, IWebHostEnvironment env)
    {
        _cacheService = cacheService;
        _env = env;
    }

    // GET: api/debug/cache (Development only)
    [HttpGet]
    public IActionResult GetCacheState()
    {
        if (!_env.IsDevelopment())
        {
            return Forbid();
        }

        return Ok(_cacheService.GetStats());
    }

    // POST: api/debug/cache/clear (Development only)
    [HttpPost("clear")]
    public IActionResult ClearAll()
    {
        if (!_env.IsDevelopment())
        {
            return Forbid();
        }

        _cacheService.Clear();
        return Ok(new { message = "Entire in-memory cache has been cleared." });
    }

    // POST: api/debug/cache/clear/{prefix} (Development only)
    [HttpPost("clear/{prefix}")]
    public IActionResult ClearPrefix(string prefix)
    {
        if (!_env.IsDevelopment())
        {
            return Forbid();
        }

        _cacheService.RemoveByPrefix(prefix);
        return Ok(new { message = $"Evicted cache keys matching prefix '{prefix}'." });
    }
}

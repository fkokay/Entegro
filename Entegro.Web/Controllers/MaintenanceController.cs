using Entegro.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Entegro.Web.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICacheManager _cache;
        public MaintenanceController(IMemoryCache memCache,ICacheManager cacheManager)
        {
            _memoryCache = memCache;
            _cache = cacheManager;
        }
        public IActionResult ClearCache()
        {
            _cache.Clear();
            _memoryCache.RemoveByPattern(_memoryCache.BuildScopedKey("*"));

            return new JsonResult(new { Success = true, Message = "Cachce temizlendi" });
        }
    }
}

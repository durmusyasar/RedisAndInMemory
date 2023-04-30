using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache memoryCache;
        public ProductController(IMemoryCache _memoryCache)
        {
            memoryCache = _memoryCache;
        }

        public IActionResult Index()
        {
            //// 1. Way
            if (String.IsNullOrEmpty(memoryCache.Get<string>("time")))
                memoryCache.Set<string>("time", DateTime.Now.ToString());

            // 2. Way
            if (memoryCache.TryGetValue<string>("timeExpiration", out string timecache))
            {
                MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(20),
                };

                MemoryCacheEntryOptions cacheOptionsTwo = new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromSeconds(20),
                };

                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1),
                    SlidingExpiration = TimeSpan.FromSeconds(20),
                    Priority = CacheItemPriority.Normal
                };

                options.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    memoryCache.Set("callback", $"{key} -> {value} => reason: {reason}");
                });

                memoryCache.Set<string>("timeExpiration", DateTime.Now.ToString(), options);
            }

            if (String.IsNullOrEmpty(timecache))
                memoryCache.Remove("time");

            Product product = new()
            {
                Id = 1,
                Name = "Test",
                Price = 12
            };
            memoryCache.Set("product:1", product);

            return View();
        }

        public IActionResult Show()
        {
            memoryCache.GetOrCreate("time", entry =>
            {
                return DateTime.Now.ToString();
            });
            memoryCache.TryGetValue<string>("timeExpiration", out string timecache);
            ViewBag.time = timecache;
            memoryCache.TryGetValue<string>("callback", out string callbackcache);
            ViewBag.callback = callbackcache;

            ViewBag.product = memoryCache.Get("product");
            return View();
        }
    }
}

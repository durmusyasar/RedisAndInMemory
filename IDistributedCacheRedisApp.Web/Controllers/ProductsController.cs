using IDistributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace IDistributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IDistributedCache distributedCache;

        public ProductsController(IDistributedCache _distributedCache)
        {
            distributedCache = _distributedCache;
        }

        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions options = new()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1),
            };

            // String caching
            distributedCache.SetString("cache", "Test 1 Cahing", options);

            Product product = new()
            {
                Id = 1,
                Name = "Test Product",
                Price = 15
            };
            // Json to String
            string jsonProduct = JsonConvert.SerializeObject(product);

            // string to Binary
            Byte[] byteProduct = Encoding.UTF8.GetBytes(jsonProduct);

            // Object caching (Json Data)
            await distributedCache.SetStringAsync("product-json:1", jsonProduct, options);
            // Object caching (Byte Binary)
            await distributedCache.SetAsync("product-binary:1", byteProduct, options);

            return View();
        }

        public async Task<IActionResult> Show()
        {
            // Get String
            string cache = distributedCache.GetString("cache") ?? "";
            ViewBag.Cache = cache;

            // Get String Object (Json)
            string productjson = await distributedCache.GetStringAsync("product-json:1") ?? "";
            Product product = JsonConvert.DeserializeObject<Product>(productjson) ?? new();
            ViewBag.Product = product;

            // Get String Object (Binary)
            Byte[] productBinary = await distributedCache.GetAsync("product-binary:1");
            string productString = Encoding.UTF8.GetString(productBinary);
            Product productByte = JsonConvert.DeserializeObject<Product>(productString) ?? new();
            ViewBag.ProductByte = productByte;


            return View();
        }

        public async Task<IActionResult> Remove()
        {
            // Remove String
            distributedCache.Remove("cache");

            // Remove String Object
            await distributedCache.RemoveAsync("product:1");

            return View();
        }

        public async Task<IActionResult> ImageCache()
        {
            DistributedCacheEntryOptions options = new()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1),
            };

            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/redis.png");

            byte[] imagebyte = System.IO.File.ReadAllBytes(path);

            await distributedCache.SetAsync("image", imagebyte, options);

            return View();
        }

        public async Task<IActionResult> ImageUrl()
        {
            byte[] imagebyte = await distributedCache.GetAsync("image");

            return File(imagebyte, "image/png");
        }
    }
}

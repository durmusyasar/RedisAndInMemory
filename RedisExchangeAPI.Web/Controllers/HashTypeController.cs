using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;

namespace RedisExchangeAPI.Web.Controllers
{
    public class HashTypeController : BaseController
    {
        public HashTypeController(RedisService _redisService) : base(_redisService)
        {
            Key = "hashnames";
        }

        public IActionResult Index()
        {
            Dictionary<string, string> keys = new();
            if (Db.KeyExistsAsync(Key).Result)
                Db.HashGetAllAsync(Key).Result.ToList().ForEach(r => keys.Add(r.Name.ToString(), r.Value.ToString()));

            return View(keys);
        }

        [HttpPost]
        public IActionResult Add(string name, string value)
        {
            // SlidingExpiration
            if (!Db.KeyExists(Key))
                Db.KeyExpire(Key, DateTime.Now.AddMinutes(5));

            Db.HashSet(Key, name, value);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string name)
        {
            await Db.HashDeleteAsync(Key, name);
            return RedirectToAction("Index");
        }
    }
}

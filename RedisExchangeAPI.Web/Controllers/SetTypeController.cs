using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class SetTypeController : BaseController
    {
        public SetTypeController(RedisService _redisService) : base(_redisService)
        {
            Key = "setnames";
            Db = _redisService.GetDb(2);
        }

        public IActionResult Index()
        {
            HashSet<string> keys = new();
            if (Db.KeyExistsAsync(Key).Result)
                Db.SetMembersAsync(Key).Result.ToList().ForEach(r => keys.Add(r.ToString()));
            
            return View(keys);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            // SlidingExpiration
            if (!Db.KeyExists(Key))
                Db.KeyExpire(Key, DateTime.Now.AddMinutes(5));

            Db.SetAdd(Key, name);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string name)
        {
            await Db.SetRemoveAsync(Key, name);
            return RedirectToAction("Index");
        }

    }
}

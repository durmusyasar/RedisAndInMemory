using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class SortedSetTypeController : BaseController
    {
        public SortedSetTypeController(RedisService _redisService) : base(_redisService)
        {
            Key = "sortedsetnames";
            Db = _redisService.GetDb(3);
        }

        public IActionResult Index()
        {
            HashSet<string> keys = new();
            //if (Db.KeyExistsAsync(Key).Result)
            //    Db.SortedSetScanAsync(Key).ToBlockingEnumerable().ToList().ForEach(r => keys.Add(r.ToString()));

            Db.SortedSetRangeByRankAsync(Key, order: Order.Ascending).Result.ToList().ForEach(r => keys.Add(r.ToString()));
            return View(keys);
        }

        [HttpPost]
        public IActionResult Add(string name, int score)
        {
            // SlidingExpiration
            if (!Db.KeyExists(Key))
                Db.KeyExpire(Key, DateTime.Now.AddMinutes(5));

            Db.SortedSetAdd(Key, name, score);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string name)
        {
            await Db.SortedSetRemoveAsync(Key, name);
            return RedirectToAction("Index");
        }
    }
}

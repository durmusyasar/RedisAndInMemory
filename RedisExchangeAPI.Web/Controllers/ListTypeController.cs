using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;

namespace RedisExchangeAPI.Web.Controllers
{
    public class ListTypeController : BaseController
    {
        public ListTypeController(RedisService _redisService) : base(_redisService)
        {
            Key = "listnames";
            Db = _redisService.GetDb(1);
        }

        public IActionResult Index()
        {
            List<string> namesList = new List<string>();
            if(Db.KeyExists(Key))
                Db.ListRange(Key).ToList().ForEach(r => namesList.Add(r.ToString()));
            return View(namesList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            Db.ListRightPush(Key, name);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string name)
        {
            await Db.ListRemoveAsync(Key, name);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteFirst()
        {
            await Db.ListLeftPopAsync(Key);
            return RedirectToAction("Index");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;

namespace RedisExchangeAPI.Web.Controllers
{
    public class StringTypeController : BaseController
    {
        public StringTypeController(RedisService _redisService) : base(_redisService)
        {
            Db = _redisService.GetDb(0);
        }

        public IActionResult Index()
        {
            Db.StringSet("name", "Durmus YASAR");
            Db.StringSet("visitor", "100");

            return View();
        }

        public IActionResult Show()
        {
            var value = Db.StringGet("name");
            Db.StringIncrement("visitor", 1);
            if (value.HasValue || Db.StringLength("name") > 5)
                ViewBag.Name = value.ToString();
            else
                value = Db.StringGetRangeAsync("name", 0, 5).Result;

            if ((int)Db.StringGetAsync("visitor").Result > 150)
                Db.StringDecrementAsync("visitor", 10).Wait();

            ViewBag.Visitor = Db.StringGet("visitor");
            return View();
        }
    }
}

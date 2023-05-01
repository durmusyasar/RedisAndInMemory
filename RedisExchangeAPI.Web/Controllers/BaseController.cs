using Microsoft.AspNetCore.Mvc;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly RedisService redisService;
        protected IDatabase Db;

        public string Key { get; set; }

        public BaseController(RedisService _redisService)
        {
            redisService = _redisService;
            Db = redisService.GetDb(0);
        }

    }
}

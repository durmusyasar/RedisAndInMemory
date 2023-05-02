
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Services
{
    public class RedisService
    {
        private readonly string redisHost;
        private readonly string redisPort;
        public IDatabase Db { get; set; }
        public string Url { get; set; }

        private ConnectionMultiplexer redis;

        public RedisService(string url)
        {
            Url = url;
        }

        public void Connect()
        {
            redis = ConnectionMultiplexer.Connect(Url);
        }

        public IDatabase GetDb(int db)
        {
            if (redis == null)
                Connect();
            return redis.GetDatabase(db);
        }
    }
}


using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Services
{
    public class RedisService
    {
        private readonly string redisHost;
        private readonly string redisPort;
        public IDatabase Db { get; set; }

        private ConnectionMultiplexer redis;

        public RedisService(IConfiguration configuration)
        {
            redisHost = configuration["Redis:Host"];
            redisPort = configuration["Redis:Port"];
        }

        public void Connect()
        {
            var configString = $"{redisHost}:{redisPort}";
            redis = ConnectionMultiplexer.Connect(configString);
        }

        public IDatabase GetDb(int db)
        {
            if (redis == null)
                Connect();
            return redis.GetDatabase(db);
        }
    }
}

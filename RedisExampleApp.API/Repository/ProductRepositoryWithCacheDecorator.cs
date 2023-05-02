using RedisExampleApp.API.Models;
using RedisExchangeAPI.Web.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisExampleApp.API.Repository
{
    public class ProductRepositoryWithCacheDecorator : IProductRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly RedisService _redisService;
        private const string productKey = "productCaches";
        private readonly IDatabase _cacheDatabase;

        public ProductRepositoryWithCacheDecorator(IProductRepository productRepository, RedisService redisService)
        {
            _redisService = redisService;
            _productRepository = productRepository;
            _cacheDatabase = _redisService.GetDb(3);
        }

        public async Task<Product> CreateASync(Product product)
        {
            var newProduct = await _productRepository.CreateASync(product);
            if (await _cacheDatabase.KeyExistsAsync(productKey))
                await _cacheDatabase.HashSetAsync(productKey, newProduct.Id, JsonSerializer.Serialize<Product>(newProduct));
            return newProduct;
        }

        public async Task<List<Product>> GetAsync()
        {
            if (!await _cacheDatabase.KeyExistsAsync(productKey))
                return await LoadToCacheFromDbAsync();
            List<Product> products = new();
            var cacheProducts = await _cacheDatabase.HashGetAllAsync(productKey);
            foreach (var item in cacheProducts.ToList())
            {
                Product product = JsonSerializer.Deserialize<Product>(item.Value.ToString()) ?? new();
                products.Add(product);
            }
            return products;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            if (await _cacheDatabase.KeyExistsAsync(productKey))
            {
                var product = await _cacheDatabase.HashGetAsync(productKey, id);
                return product.HasValue ? JsonSerializer.Deserialize<Product>(product.ToString()) : null;
            }
            var products = await LoadToCacheFromDbAsync();
            return products.FirstOrDefault(r => r.Id == id);
        }

        private async Task<List<Product>> LoadToCacheFromDbAsync()
        {
            List<Product> products = await _productRepository.GetAsync();
            products.ForEach(product =>
            {
                _cacheDatabase.HashSetAsync(productKey, product.Id, JsonSerializer.Serialize(product));
            });

            return products;
        }
    }
}

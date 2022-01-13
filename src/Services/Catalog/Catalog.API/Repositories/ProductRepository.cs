using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var products = await _context
                .Products
                .Find(product => true)
                .ToListAsync();
            
            return products;
        }

        public async Task<Product> GetProductById(string id)
        {
            var product = await _context
                .Products
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
            
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Name, name);
            var products = await _context
                .Products
                .Find(filter)
                .ToListAsync();
            
            return products;
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string category)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Category, category);
            var products = await _context
                .Products
                .Find(filter)
                .ToListAsync();
            
            return products;
        }

        public async Task CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> UpdateProduct(Product product)
        {
           var result = await _context
               .Products
               .ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);
           
           var updatedStatus = result.IsAcknowledged && result.ModifiedCount > 0;
          
           return updatedStatus;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);

            var result = await _context
                .Products
                .DeleteOneAsync(filter:filter);
           
            var deleteStatus = result.IsAcknowledged && result.DeletedCount > 0;
            
            return deleteStatus;
        }
    }
}
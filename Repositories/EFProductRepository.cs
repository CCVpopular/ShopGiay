using Microsoft.EntityFrameworkCore;
using ShopGiay.EF;
using ShopGiay.Models;

namespace ShopGiay.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(x=>x.Category).Include(p => p.Promotions).Include(p => p.ProductColor).ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.AsNoTracking().Include(p => p.ProductColor).Include(x => x.Category).Include(p => p.Promotions).Include(y => y.Images).SingleOrDefaultAsync(x => x.Id == id);
        }
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Product>> GetByNameAsync(string SearchString)
        {
            return await _context.Products.Include(x => x.Category).Include(p => p.ProductColor).Include(p => p.Promotions).Where(n => n.Name.Contains(SearchString)).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetByNameCategoryAsync(string SearchString)
        {
            return await _context.Products.Include(x => x.Category).Include(p => p.ProductColor).Include(p => p.Promotions).Where(n => n.Category.Name.Contains(SearchString)).ToListAsync();
        }
        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddImageAsync(ProductImage ProductImage)
        {
            _context.ProductImages.Add(ProductImage);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Brand>> GetBrandsAsync()
        {
            return await _context.Brands.ToListAsync();
        }
        public async Task<IEnumerable<Supplier>> GetSuppliersAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }
        public async Task<IEnumerable<Promotion>> GetPromotionAsync()
        {
            return await _context.Promotions.ToListAsync();
        }
        public async Task<IEnumerable<ProductColor>> GetProductColorAsync()
        {
            return await _context.ProductColors.ToListAsync();
        }
        public async Task<IEnumerable<ProductSize>> GetProductSizeAsync()
        {
            return await _context.ProductSizes.Include(p => p.ProductQuantities).ToListAsync();
        }
        public async Task<IEnumerable<ProductQuantity>> GetProductQuantityAsync(int id)
        {
            return await _context.ProductQuantities.Where(q => q.ProductId == id).ToListAsync();
        }
        public async Task<ProductQuantity> GetProductQuantityByProductIdAndBySizeIdAsync(int id, int size)
        {
            return await _context.ProductQuantities.Include(pq => pq.ProductSize).SingleOrDefaultAsync(q => q.ProductId == id && q.ProductSizeId == size);
        }
        public async Task UpdateQuantiesAsync(ProductQuantity productQuantity)
        {
            _context.ProductQuantities.Update(productQuantity);
            await _context.SaveChangesAsync();
        }
        public async Task<ProductQuantity> GetQuantityBySizeAsync(int size,int id)
        {
            return await _context.ProductQuantities
                                 .Include(pq => pq.ProductSize)
                                 .FirstOrDefaultAsync(pq => pq.ProductSize.Size == size && pq.ProductId == id);
        }
        public async Task<List<ProductQuantity>> GetQuantitiesForProducts(List<int> productIds)
        {
             return await _context.ProductQuantities.Where(pq => productIds.Contains(pq.ProductId)).ToListAsync(); ;
        }
        public async Task<IQueryable<Product>> GetProductsWithSizes()
        {
            return _context.Products
                .Include(q => q.ProductQuantities)
                .ThenInclude(s => s.ProductSize)
                .Include(p => p.Promotions)
                .Include(c => c.ProductColor)               
                .AsQueryable();
        }
        // Trong ProductRepository.cs
        public async Task<List<string>> GetSuggestionsAsync(string searchString)
        {
            // Lấy các sản phẩm có tên chứa searchString
            var products = await _context.Products
                .Where(p => p.Name.Contains(searchString))
                .Select(p => p.Name)
                .Distinct()
                .ToListAsync();

            return products;
        }
    }
}
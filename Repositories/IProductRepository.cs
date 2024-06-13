using ShopGiay.Models;

namespace ShopGiay.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<IEnumerable<Product>> GetByNameAsync(string SearchString);
        Task<IEnumerable<Product>> GetByNameCategoryAsync(string SearchString);
        Task DeleteImageAsync(int imageId);
        Task AddImageAsync(ProductImage ProductImage);
        Task<IEnumerable<Brand>> GetBrandsAsync();
        Task<IEnumerable<Supplier>> GetSuppliersAsync();
        Task<IEnumerable<ProductColor>> GetProductColorAsync();
        Task<IEnumerable<ProductSize>> GetProductSizeAsync();
        Task<IEnumerable<ProductQuantity>> GetProductQuantityAsync(int id);
        Task<IEnumerable<Promotion>> GetPromotionAsync();
        Task<ProductQuantity> GetProductQuantityByProductIdAndBySizeIdAsync(int id, int size);
        Task UpdateQuantiesAsync(ProductQuantity productQuantity);
        Task<ProductQuantity> GetQuantityBySizeAsync(int size,int id);
        Task<List<string>> GetSuggestionsAsync(string searchString);
        Task<IQueryable<Product>> GetProductsWithSizes();
        Task<List<ProductQuantity>> GetQuantitiesForProducts(List<int> productIds);
    }
}

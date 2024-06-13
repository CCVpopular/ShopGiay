namespace ShopGiay.Models
{
    public class ProductQuantity
    {
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ProductSizeId { get; set; }
        public ProductSize ProductSize { get; set; }
    }
}

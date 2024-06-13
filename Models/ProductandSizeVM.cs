namespace ShopGiay.Models
{
    public class ProductandSizeVM
    {
        public int? CategoryId { get; set; }
        public int? ProductColorId { get; set; }
        public int? BrandId { get; set; }
        public int? SizeId { get; set; }
        public bool IsDiscount { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }  
        public Product Product { get; set; }
        public ProductSize ProductSize { get; set; }
    }
}

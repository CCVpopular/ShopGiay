using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;

namespace ShopGiay.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal OriginPrice { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<ProductImage>? Images { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int ProductColorId { get; set; }
        public ProductColor? ProductColor { get; set; }
        public int BrandId { get; set; }
        public Brand? Brands { get; set; }
        public int? PromotionId { get; set; }
        public Promotion? Promotions { get; set; }
        public int SupplierId { get; set; }
        public Supplier? Suppliers { get; set; }
        public List<ProductQuantity>? ProductQuantities { get; set; }
        [NotMapped]
        public decimal DiscountedPrice => PromotionId != null && Promotions != null ? Price - (Price * Promotions.DiscountPercent / 100) : Price;
    }
}

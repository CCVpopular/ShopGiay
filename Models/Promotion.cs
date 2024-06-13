namespace ShopGiay.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public decimal DiscountPercent { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ShopGiay.Models
{
    public class CartItem
    {
        public string Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public int SizeId { get; set; }
    }
}

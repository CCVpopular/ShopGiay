using System.ComponentModel.DataAnnotations;
namespace ShopGiay.Models
{
    public class ProductColor
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public List<Product>? Products { get; set; }
    }
}

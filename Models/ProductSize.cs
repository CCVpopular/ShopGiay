namespace ShopGiay.Models
{
    public class ProductSize
    {
        public int Id { get; set; }
        public int Size { get; set; }    
        public List<ProductQuantity>? ProductQuantities { get; set; }
    }
}
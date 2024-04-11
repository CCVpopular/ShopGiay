using Microsoft.CodeAnalysis;

namespace ShopGiay.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public CartModel Cart { get; set; } = new CartModel();
        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.ProductId == productId);
        }
        public void UpdateQuantity(int productId ,int quantity)
        {
            if (quantity > 0) 
            {
				var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
				if (existingItem != null)
				{
					existingItem.Quantity = quantity;
				}
			}
		}


        // Các phương thức khác..
    }
}

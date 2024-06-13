using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ShopGiay.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public CartModel Cart { get; set; } = new CartModel();
        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId && i.SizeId == item.SizeId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;                          
            }
            else
            {
                Items.Add(item);
            }
        }
        public void RemoveItem(string productId)
        {
            Items.RemoveAll(i => i.Id == productId);
        }
        public void UpdateQuantity(int productId ,int quantity,string cartId)
        {
            if (quantity > 0) 
            {
				var existingItem = Items.FirstOrDefault(i => i.ProductId == productId && i.Id == cartId);
				if (existingItem != null)
				{
					existingItem.Quantity = quantity;
				}
			}
		}

        public void UpdateSize(string cartId, int sizeId)
        {
            var existingItem = Items.FirstOrDefault(i => i.Id == cartId);
                if (existingItem != null)
                {
                    existingItem.Id = existingItem.ProductId + "-" + sizeId;
                    existingItem.SizeId = sizeId;
                    existingItem.Quantity = 1;
                }
            
        }

        // Các phương thức khác..
    }
}

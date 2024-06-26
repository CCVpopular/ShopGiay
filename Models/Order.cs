﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ShopGiay.Models
{
	public class Order
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal Total { get; set; }
		public string? PhoneNumber { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
		public string? ShippingAddress { get; set; }
		public bool IsShipped { get; set; }	
		public OrderStatus OrderStatus { get; set; }
		public string? Notes { get; set; }
		public ApplicationUser User { get; set; }
		public List<OrderDetail> OrderDetails { get; set; }
	}
    public enum OrderStatus
    {
        Chưa_Thanh_Toán,
        Đã_Thanh_Toán,
        Bị_Hủy
    }
		
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Models;

namespace ShopGiay.EF
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductQuantity> ProductQuantities { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}

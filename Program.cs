using Microsoft.EntityFrameworkCore;
using ShopGiay.EF;
using ShopGiay.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ShopGiay.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddDefaultTokenProviders().AddDefaultUI().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=ProductManager}/{action=Index}/{id?}"
    );
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Member" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManeger = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string email = "admin@gmail.com";
    string password = "Text12!@";
    string fullname = "Admin";

    if (await userManeger.FindByEmailAsync(email) == null)
    {
        var user = new ApplicationUser();
        user.UserName = email;
        user.Email = email;
        user.FullName = fullname;

        await userManeger.CreateAsync(user, password);

        await userManeger.AddToRoleAsync(user, "Admin");

    }
}

app.Run();

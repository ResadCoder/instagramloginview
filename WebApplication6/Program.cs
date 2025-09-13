using Microsoft.EntityFrameworkCore;
using WebApplication6.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext for MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33)) // MySQL version
    );
});

var app = builder.Build();

// Middleware pipeline

// Error handling & security
if (!app.Environment.IsDevelopment())
{
    // Show friendly error page instead of stack trace
    app.UseExceptionHandler("/Home/Error");

    // Enforce HTTPS for production
    app.UseHsts();
}
else
{
    // Developer-friendly error page
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
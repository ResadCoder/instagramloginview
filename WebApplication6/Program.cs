using WebApplication6.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Get connection string from Railway environment variable
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Parse DATABASE_URL from format: mysql://user:password@host:port/database
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':'); // ["user", "password"]
    var server = uri.Host;
    var port = uri.Port;
    var database = uri.AbsolutePath.TrimStart('/');

    connectionString = $"Server={server};Port={port};Database={database};User={userInfo[0]};Password={userInfo[1]};";
}
else
{
    // Fallback for local development
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration["ConnectionStrings:DefaultConnection"], 
        new MySqlServerVersion(new Version(8, 0, 33))
    )
);

var app = builder.Build();

// Configure middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
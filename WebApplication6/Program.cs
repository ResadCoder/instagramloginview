using Microsoft.EntityFrameworkCore;
using WebApplication6.Context;

var builder = WebApplication.CreateBuilder(args);

// 1. Read MySQL URL from environment variable
var mysqlUrl = Environment.GetEnvironmentVariable("MYSQL_URL1");

if (string.IsNullOrEmpty(mysqlUrl))
{
    throw new InvalidOperationException("Database connection string not found in environment variable 'MYSQL_URL1'.");
}

// 2. Convert mysql:// URI to EF Core connection string
// mysql://user:password@host:port/database
var uri = new Uri(mysqlUrl);
var userInfo = uri.UserInfo.Split(':');

var efConnectionString = $"Server={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};User={userInfo[0]};Password={userInfo[1]}";

// 3. Add services to the container
builder.Services.AddControllersWithViews();

// 4. Configure DbContext with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(efConnectionString, new MySqlServerVersion(new Version(8, 0, 33)))
);

var app = builder.Build();

// 5. Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 6. Default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
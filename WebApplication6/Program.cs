using Microsoft.EntityFrameworkCore;
using WebApplication6.Context;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Get MySQL URL from environment variable
var mysqlUrl = Environment.GetEnvironmentVariable("MYSQL_URL11");

if (string.IsNullOrEmpty(mysqlUrl))
{
    throw new InvalidOperationException("MYSQL_URL11 environment variable is required.");
}

// Parse URL to EF Core connection string
var uri = new Uri(mysqlUrl);
var userInfo = uri.UserInfo.Split(':');
var efConnectionString = $"Server={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};User={userInfo[0]};Password={userInfo[1]}";

// 2️⃣ Add services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(efConnectionString, new MySqlServerVersion(new Version(8, 0, 33)))
);
builder.Services.AddAuthorization(); // required for UseAuthorization

var app = builder.Build();

// Optional: run migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// 3️⃣ Middleware pipeline
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
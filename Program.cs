using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using smartmetercms.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with SQLite configuration
builder.Services.AddDbContext<smartmetercmsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("smartmetercmsContext") ?? throw new InvalidOperationException("Connection string 'smartmetercmsContext' not found.")));

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Cookie is accessible only through HTTP requests
    options.Cookie.IsEssential = true; // Essential for the application to run
});

// Add services to the container
builder.Services.AddControllersWithViews();

//signalR registering
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


// Use session middleware
app.UseSession();

// Authorization and authentication middleware
app.UseAuthorization();

// Map controller routes
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

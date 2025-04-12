using HaloHair.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnectionString")));


// Add services to the container.
builder.Services.AddControllersWithViews();



// Add session services
builder.Services.AddDistributedMemoryCache();  // You can use other cache options like Redis or SQL Server
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
    options.Cookie.HttpOnly = true; // Ensure cookie is accessible only by the server
    options.Cookie.IsEssential = true; // Mark cookie as essential for your app
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session middleware
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Men}/{action=LoginUserMen}/{id?}");
//pattern: "{controller=Men}/{action=RegisterUserMen}/{id?}");

//pattern: "{controller=Barber}/{action=RegisterBarber}/{id?}");
//pattern: "{controller=Barber}/{action=LoginBarberMen}/{id?}");



app.Run();

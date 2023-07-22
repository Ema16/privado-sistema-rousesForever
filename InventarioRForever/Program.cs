using Microsoft.EntityFrameworkCore;
using InventarioRForever.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Conexión a la base de datos
builder.Services.AddDbContext<InventarioRfContext>(
context => context.UseMySQL(builder.Configuration.GetConnectionString("conexion")));

//Par a el Login_______________________________________________________
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.AccessDeniedPath = "/Usuario/AccessDenided"; //Busca el controlador Home y el metodo Privacy
        options.Cookie.Name = "Identity.Cookie";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Usuario/Login";       //Busca el controlador Usuario y la vista el metodo Login
        options.LogoutPath = "/Home";
        // ReturnUrlParameter requires 
        //using Microsoft.AspNetCore.Authentication.Cookies;
        options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
        options.SlidingExpiration = true;
    });

// Force Identity's security stamp to be validated every minute.
//builder.Services.Configure<SecurityStampValidatorOptions>(o =>
//o.ValidationInterval = TimeSpan.FromMinutes(1));
//______________________________________________________

//Para lo Roles____________________________________________
builder.Services.AddAuthorization(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Administrador", "Ventas", "Cliente")
                .Build();
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

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


IWebHostEnvironment env = app.Environment;
Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "../Rotativa/Windows");

app.Run();

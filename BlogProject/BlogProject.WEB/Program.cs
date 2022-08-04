using BlogProject.DAL.Context;
using BlogProject.DAL.Repositories.Concrete;
using BlogProject.DAL.Repositories.Interfaces.Concrete;
using BlogProject.WEB.Models.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database ba�lant� yolu tan�mlan�r.
builder.Services.AddDbContext<ProjectContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});

// Identity k�t�phanesini kullanmak i�in
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ProjectContext>();

// Kimlik do�rulama i�in
builder.Services.AddAuthentication();

// Cookieler aktif edilir
builder.Services.ConfigureApplicationCookie(a =>
{
    a.LoginPath = new PathString("/Home/Login");
    a.ExpireTimeSpan = TimeSpan.FromDays(1);
    a.Cookie = new CookieBuilder { Name = "KullaniciCookie", SecurePolicy = CookieSecurePolicy.Always };
});


// IOC pattern deseni CORE i�in kullan�l�r. AddScope denerek bu durum tan�mlan�r. Yani kodu yazarken interface olarak yazd���m�z halde kendisi repository s�n�f�nda g�vdeleri olan metotlar� tan�yacakt�r.
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUserFollowedCategoryRepository, UserFollowedCategoryRepository>();

// AutoMapper s�n�f�n�n komutlar�n�n kullan�lmas� i�in komutlar yaz�l�r.
builder.Services.AddAutoMapper(typeof(Mapping));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

// Kimlik do�rulama i�in
app.UseAuthentication();

// Yetkilendirme
app.UseAuthorization();

// Arealar�n rotas� yani yollar� belirtilir. Burada �nemli nokta �nce arean�n yolu kontrol edilmesi i�in default route komutunun �st�ne yaz�lmal�d�r.
app.MapControllerRoute(
    name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

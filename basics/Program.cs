
using basics.Models; 
using basics.Services; 
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Controller ve View desteği ekliyoruz
builder.Services.AddControllersWithViews();

// Add DbContext service to the container
// Connection stringi "appsettings.json" dosyasından alıyoruz.
var connectionString = builder.Configuration.GetConnectionString("ArabaParkSistemiDb");
builder.Services.AddDbContext<ArabaParkSistemiContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<BolgeService>(); // Bölge ile ilgili iş mantığı
builder.Services.AddScoped<ArabaKayitService>(); // Araç kayıt işlemleri için
builder.Services.AddScoped<BolgeGirisService>();
builder.Services.AddScoped<GirisCikisRaporService>();// 
builder.Services.AddScoped<OtoparkService>();
// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Hata durumunda kullanıcıya özel hata sayfası göstermek
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Üretim ortamı için HSTS ekleyerek güvenliği artırmak
}

app.UseHttpsRedirection(); // HTTP taleplerini HTTPS'e yönlendirir
app.UseStaticFiles(); // Statik dosyalara erişimi sağlar (wwwroot klasöründeki dosyalar)

// Rota yapılandırması
app.UseRouting();

app.UseAuthorization(); // Yetkilendirme middleware'i

// Varsayılan rota yapılandırması
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Uygulamayı başlat
app.Run();


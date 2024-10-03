# OtoparkSystem

Bu proje, sınırlı alanı olan bir otoparkın yönetimini sağlamak için geliştirilmiş bir **Otopark Yönetim Sistemi**'dir. Kullanıcılar araçlarını uygun alanlara park edebilir ve sistem her bir aracın park yerini yönetir.

## Özellikler

- Park alanı durumu takibi
- MVC yapısında geliştirilmiş web arayüzü
- 
## Çalışma Mantığı
Öncelikli Olarak A otoparkından başlayarak sırasıyla B ve C otoparkları dolmaya başlar. Her otopark farklı boyutlardaki bögelerden oluşmaktadır.
## Gereksinimler
- .NET Core SDK 6.0 ve üzeri
- SQL Server (Veritabanı dosyası `.bacpac` formatındadır)
- Microsoft.EntityFrameworkCore 8.0.8
- Microsoft.EntityFrameworkCore.InMemory 8.0.8
- Microsoft.EntityFrameworkCore.Tools 8.0.8
- Microsoft.EntityFrameworkCore.SqlServer8.0.8
- xunit
- xunit.runner.visualstudio (Opsiyonel)
## Kurulum

1. Projeyi GitHub'dan klonlayın:
   ```bash
   git clone https://github.com/borakgul/OtoparkSystem.git
   
2.SQLSERVER'da **.bacpac** dosyasını açtıktan sonra Database SQLSERVER bağlantısı kurulmuş olur. Database ile sistemin(basics) bağlantısı için basics içerisindeki **appsettings.json** dosyasasında SQL bağlantı path'i ve DB adını giriniz. Böylelikle database'e bağlanmış olursunuz.

3.**ArabaParkSistemiContext.cs** içerisinde bulunan **OnConfiguring** methodunun içerisindeki optionsBuilder.UseSqlServer("Server=*XXX*;Database=*XXXX*;Trusted_Connection=True;TrustServerCertificate=True;"); alanı database ismi ve server'a göre ayarlayınız.

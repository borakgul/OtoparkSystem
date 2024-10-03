using basics.Models;
using basics.Services;
using basics.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OtoparkTest
{
    public class GirisCikisRaporServiceTests
    {
        [Fact]
        public void GetGirisCikisRapor_RaporToplama()
        {
            // In-Memory veritabanı için DbContext yapılandırması.
            var options = new DbContextOptionsBuilder<ArabaParkSistemiContext>()
                .UseInMemoryDatabase(databaseName: "GirisCikisRaporTestDb")
                .Options;

          
            using (var context = new ArabaParkSistemiContext(options))
            {
                // Test verilerini ekliyoruz
                var araba = new Araba { ArabaPlaka = "34XYZ123", Boyut = "Küçük" };
                context.Arabas.Add(araba);

                var otopark = new Otopark { Id = 1, KücükArabaPara = 10, OrtaArabaPara = 15, BüyükArabaPara = 20 };
                context.Otoparks.Add(otopark);

                var bolge = new Bölge { Id = 1, OtoparkId = 1, KücükBos = 5, OrtaBos = 3, BüyükBos = 2 };
                context.Bölges.Add(bolge);

                var bolgeGiris = new BölgeGiris
                {
                    ArabaId = araba.Id,
                    BölgeId = bolge.Id,
                    GirisZamani = System.DateTime.Now.AddHours(-3), // 3 saat önce giriş
                    CikisZamani = System.DateTime.Now // Şimdi çıkış
                };
                context.BölgeGirises.Add(bolgeGiris);

                context.SaveChanges();

                // Servisi test etmek için yaratıyoruz
                var raporService = new GirisCikisRaporService(context);

                // Giriş-çıkış raporunu alıyoruz
                var rapor = raporService.GetGirisCikisRapor();

                // Raporun doğruluğunu kontrol ediyoruz
                Assert.NotNull(rapor);
                Assert.Single(rapor);
                var item = rapor.First();
                Assert.Equal("34XYZ123", item.ArabaPlaka);
                Assert.Equal("Küçük", item.ArabaBoyut);
                Assert.True(item.Ucret > 0); 
            }
        }

        [Fact]
        public void GetOtoparkKazancRaporu_Fatura_Kontrol()
        {
            // In-Memory veritabanı için DbContext yapılandırması.
            var options = new DbContextOptionsBuilder<ArabaParkSistemiContext>()
                .UseInMemoryDatabase(databaseName: "OtoparkKazancRaporTestDb")
                .Options;

            
            using (var context = new ArabaParkSistemiContext(options))
            {
                // Test verilerini ekliyoruz
                var otopark = new Otopark { Id = 1, KücükArabaPara = 10, OrtaArabaPara = 15, BüyükArabaPara = 20 };
                context.Otoparks.Add(otopark);

                var bolge = new Bölge { Id = 1, OtoparkId = 1, KücükBos = 5, OrtaBos = 3, BüyükBos = 2 };
                context.Bölges.Add(bolge);

                var araba1 = new Araba { ArabaPlaka = "34XYZ123", Boyut = "Küçük" };
                var araba2 = new Araba { ArabaPlaka = "34ABC456", Boyut = "Orta" };
                context.Arabas.AddRange(araba1, araba2);

                var bolgeGiris1 = new BölgeGiris
                {
                    ArabaId = araba1.Id,
                    BölgeId = bolge.Id,
                    GirisZamani = System.DateTime.Now.AddHours(-5), // 5 saat önce giriş
                    CikisZamani = System.DateTime.Now // Şimdi çıkış
                };

                var bolgeGiris2 = new BölgeGiris
                {
                    ArabaId = araba2.Id,
                    BölgeId = bolge.Id,
                    GirisZamani = System.DateTime.Now.AddHours(-2), // 2 saat önce giriş
                    CikisZamani = System.DateTime.Now // Şimdi çıkış
                    //geçen süre 3 saat 
                };
                context.BölgeGirises.AddRange(bolgeGiris1, bolgeGiris2);

                context.SaveChanges();

               
                var raporService = new GirisCikisRaporService(context);

               
                var kazancRaporu = raporService.GetOtoparkKazancRaporu();

                // Kazanç raporunun doğruluğunu kontrol ediyoruz
                Assert.NotNull(kazancRaporu);
  

                var item = kazancRaporu.First();
                Assert.True(item.ToplamKazanc > 0); // Beklenen kazanç 6 saat * 10 + 3 saat * 15

                // Beklenen kazancı hesaplıyoruz, 6 ve 3 yaptık geçen süre saniye olarak geçse bile bir üst saate yuvarlanır
                double beklenenKazanc = (6 * 10) + (3 * 15);
                Assert.Equal(beklenenKazanc, item.ToplamKazanc);
            }
        }
    }
}

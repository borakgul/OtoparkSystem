using basics.Models;
using basics.Services;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace OtoparkTest
{
    public class ParkUcretHesaplamaTests
    {
        [Fact]
        public void CalculateUcret_B_araba()
        {
            // In-Memory veritabanı için DbContext yapılandırması.
            var options = new DbContextOptionsBuilder<ArabaParkSistemiContext>()
                .UseInMemoryDatabase(databaseName: "CalculateUcretTestttDb")
                .Options;

            // Test verilerini oluşturma ve servislerin yaratılması.
            using (var context = new ArabaParkSistemiContext(options))
            {
                // Otopark verisi 
                var otopark = new Otopark { Id = 1, KücükArabaPara = 10, OrtaArabaPara = 15, BüyükArabaPara = 20 };
                context.Otoparks.Add(otopark);

                // Bölge 
                var bolge = new Bölge { Id = 1, OtoparkId = 1 };
                context.Bölges.Add(bolge);

                // Araba 
                var araba = new Araba { Id = 1, ArabaPlaka = "34ABC123", Boyut = "Büyük" };
                context.Arabas.Add(araba);

                // BölgeGiris 
                var bolgeGiris = new BölgeGiris
                {
                    Id = 1,
                    ArabaId = araba.Id,
                    BölgeId = bolge.Id,
                    GirisZamani = DateTime.Now.AddHours(-3), // 3 saat önce giriş yapmış
                    CikisZamani = DateTime.Now // Şu an çıkış yapıyor
                    //Geçen süre 4 saat !
                };
                context.BölgeGirises.Add(bolgeGiris);

                context.SaveChanges();

                // Servisi test etmek için yaratıyoruz
                var raporService = new GirisCikisRaporService(context);

                // Ücret hesaplamayı test ediyoruz
                var hesaplananUcret = raporService.CalculateUcret(bolgeGiris);
                TimeSpan parkSuresi = bolgeGiris.CikisZamani.Value - bolgeGiris.GirisZamani.Value;
                double toplamSaat = Math.Ceiling(parkSuresi.TotalHours);

                //( 4 saat olmasının sebebi double alındığı için 4 saate yuvarlıyor olmasıdır)
                double beklenenUcret = 4 * 20;

                // Ücretin beklenen değere eşit olup olmadığını kontrol ediyoruz
                Assert.Equal(beklenenUcret, hesaplananUcret);
            }
        }

        [Fact]
        public void CalculateUcret__For_Kucuk_Araba()
        {
            // In-Memory veritabanı için DbContext yapılandırması.
            var options = new DbContextOptionsBuilder<ArabaParkSistemiContext>()
                .UseInMemoryDatabase(databaseName: "CalculateUcretKucukArabaTestDb")
                .Options;

          
            using (var context = new ArabaParkSistemiContext(options))
            {
                // Otopark  
                var otopark = new Otopark { Id = 1, KücükArabaPara = 5, OrtaArabaPara = 10, BüyükArabaPara = 15 };
                context.Otoparks.Add(otopark);

                // Bölge 
                var bolge = new Bölge { Id = 1, OtoparkId = 1 };
                context.Bölges.Add(bolge);

                // Araba 
                var araba = new Araba { Id = 1, ArabaPlaka = "34XYZ456", Boyut = "Küçük" };
                context.Arabas.Add(araba);

                // BölgeGiris 
                var bolgeGiris = new BölgeGiris
                {
                    Id = 2,
                    ArabaId = araba.Id,
                    BölgeId = bolge.Id,
                    GirisZamani = DateTime.Now.AddHours(-4), // 4 saat önce giriş yapmış
                    CikisZamani = DateTime.Now // Şu an çıkış yapıyor
                };
                context.BölgeGirises.Add(bolgeGiris);

                context.SaveChanges();

               
                var raporService = new GirisCikisRaporService(context);

              
                var hesaplananUcret = raporService.CalculateUcret(bolgeGiris);

                //( 5 saat olmasının sebebi double alındığı için 4 saate yuvarlıyor olmasıdır)
                double beklenenUcret = 5 * 5;

                // Ücretin beklenen değere eşit olup olmadığını kontrol ediyoruz
                Assert.Equal(beklenenUcret, hesaplananUcret);
            }
        }
    }
}

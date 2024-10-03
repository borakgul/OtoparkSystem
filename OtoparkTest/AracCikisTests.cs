using basics.Models;
using basics.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace OtoparkTest
{
    public class AracCikisServiceTests
    {
        [Fact]
        public void AracCikis_Should_Update_BolgeBosYer_And_Remove_Arac()
        {
            // In-Memory veritabaný için DbContext yapýlandýrmasý.
            var options = new DbContextOptionsBuilder<ArabaParkSistemiContext>()
                .UseInMemoryDatabase(databaseName: "AracCikisTestDb")
                .Options;

            // Testin baþýnda context ve servis yaratýlýr.
            using (var context = new ArabaParkSistemiContext(options))
            {
               
                var bolgeGirisService = new BolgeGirisService(context);

                // araba ekliyoruz
                var yeniAraba = new Araba
                {
                    ArabaPlaka = "34DEF123",
                    Boyut = "Orta"
                };
                context.Arabas.Add(yeniAraba);
                context.SaveChanges();

                // Bir bölge ekleyip araba için giriþ kaydý oluþturulur
                var bolge = new Bölge
                {
                    Id = 1,
                    OtoparkId = 1,
                    KücükBos = 10,
                    OrtaBos = 10,
                    BüyükBos = 10
                };
                context.Bölges.Add(bolge);
                context.SaveChanges();

                var bolgeGiris = new BölgeGiris
                {
                    ArabaId = yeniAraba.Id,
                    BölgeId = bolge.Id,
                    GirisZamani = DateTime.Now.AddHours(-2) // Giriþ zamanýný geçmiþ bir saate ayarlýyoruz.
                };
                context.BölgeGirises.Add(bolgeGiris);
                context.SaveChanges();

                // Araç çýkýþ iþlemi yapýlýr.
                bool cikisBasarili = bolgeGirisService.AracCikisYap(yeniAraba.Id);

                // Çýkýþ iþlemi baþarýlý olmalý (Assert.True ile doðruluyoruz).
                Assert.True(cikisBasarili, "Araç çýkýþ iþlemi baþarýsýz oldu.");

                // Araç çýkýþýndan sonra bölgedeki boþ yer sayýsýnýn artmýþ olmasý gerekiyor.
                var updatedBolge = context.Bölges.FirstOrDefault(b => b.Id == bolge.Id);
                Assert.NotNull(updatedBolge);
                Assert.Equal(11, updatedBolge.OrtaBos);

                // Ayrýca araç kaydýnýn güncellenip güncellenmediðini kontrol ediyoruz.
                var cikisYapilanGiris = context.BölgeGirises.FirstOrDefault(bg => bg.ArabaId == yeniAraba.Id);
                Assert.NotNull(cikisYapilanGiris);
                Assert.NotNull(cikisYapilanGiris.CikisZamani); // Çýkýþ zamaný null olmamalý.
            }
        }
    }
}

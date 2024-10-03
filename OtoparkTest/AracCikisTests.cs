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
            // In-Memory veritaban� i�in DbContext yap�land�rmas�.
            var options = new DbContextOptionsBuilder<ArabaParkSistemiContext>()
                .UseInMemoryDatabase(databaseName: "AracCikisTestDb")
                .Options;

            // Testin ba��nda context ve servis yarat�l�r.
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

                // Bir b�lge ekleyip araba i�in giri� kayd� olu�turulur
                var bolge = new B�lge
                {
                    Id = 1,
                    OtoparkId = 1,
                    K�c�kBos = 10,
                    OrtaBos = 10,
                    B�y�kBos = 10
                };
                context.B�lges.Add(bolge);
                context.SaveChanges();

                var bolgeGiris = new B�lgeGiris
                {
                    ArabaId = yeniAraba.Id,
                    B�lgeId = bolge.Id,
                    GirisZamani = DateTime.Now.AddHours(-2) // Giri� zaman�n� ge�mi� bir saate ayarl�yoruz.
                };
                context.B�lgeGirises.Add(bolgeGiris);
                context.SaveChanges();

                // Ara� ��k�� i�lemi yap�l�r.
                bool cikisBasarili = bolgeGirisService.AracCikisYap(yeniAraba.Id);

                // ��k�� i�lemi ba�ar�l� olmal� (Assert.True ile do�ruluyoruz).
                Assert.True(cikisBasarili, "Ara� ��k�� i�lemi ba�ar�s�z oldu.");

                // Ara� ��k���ndan sonra b�lgedeki bo� yer say�s�n�n artm�� olmas� gerekiyor.
                var updatedBolge = context.B�lges.FirstOrDefault(b => b.Id == bolge.Id);
                Assert.NotNull(updatedBolge);
                Assert.Equal(11, updatedBolge.OrtaBos);

                // Ayr�ca ara� kayd�n�n g�ncellenip g�ncellenmedi�ini kontrol ediyoruz.
                var cikisYapilanGiris = context.B�lgeGirises.FirstOrDefault(bg => bg.ArabaId == yeniAraba.Id);
                Assert.NotNull(cikisYapilanGiris);
                Assert.NotNull(cikisYapilanGiris.CikisZamani); // ��k�� zaman� null olmamal�.
            }
        }
    }
}

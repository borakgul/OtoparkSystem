namespace OtoparkTest
{
    using basics.Models;
    using basics.Services;
    using Microsoft.EntityFrameworkCore;

    public class ArabaKayitServiceTests
    {
        [Fact]
        public void Araba_Ekleme_Test()
        {
            // In-Memory veritabanı için DbContext yapılandırması.
            var options = new DbContextOptionsBuilder<ArabaParkSistemiContext>()
                .UseInMemoryDatabase(databaseName: "ArabaKayitTestDb")
                .Options;

            // Testin başında context ve servis yaratılır.
            using (var context = new ArabaParkSistemiContext(options))
            {
                var arabaService = new ArabaKayitService(context);

                // Yeni bir araba oluşturulur.
                var yeniAraba = new Araba
                {
                    ArabaPlaka = "34XYZ123",
                    Boyut = "Küçük"
                };

                // Araba kaydetme işlemi yapılır.
                bool kayitBasarili = arabaService.AddAraba(yeniAraba);

                // Kayıt başarılı olmalı (Assert.True ile doğruluyoruz).
                Assert.True(kayitBasarili, "Araba kaydı veritabanına eklenemedi.");

                // Kaydın gerçekten veritabanında olup olmadığını kontrol ediyoruz.
                var addedAraba = context.Arabas.FirstOrDefault(a => a.ArabaPlaka == "34XYZ123");
                Assert.NotNull(addedAraba);
                Assert.Equal("34XYZ123", addedAraba.ArabaPlaka);
                Assert.Equal("Küçük", addedAraba.Boyut);
            }
        }
    }
}
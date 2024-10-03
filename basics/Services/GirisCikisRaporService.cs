using basics.Models;
using basics.ViewModels; // Kazanç raporuyla ilgili ViewModel'in bulunduğu yer
using Microsoft.EntityFrameworkCore;

namespace basics.Services
{
    public class GirisCikisRaporService
    {
        private readonly ArabaParkSistemiContext _context;

        public GirisCikisRaporService(ArabaParkSistemiContext context)
        {
            _context = context;
        }

        // Giriş çıkış yapmış araçların raporunu oluşturma metodu
        public IEnumerable<GirisCikisRaporViewModel> GetGirisCikisRapor()
        {
            // Daha önce tanımlanan rapor işlemi
            var rapor = _context.BölgeGirises
                   .Include(bg => bg.Araba)
                   .Include(bg => bg.Bölge)
                   .ThenInclude(b => b.Otopark)
                   .Where(bg => bg.CikisZamani != null)
                   .AsEnumerable() // Bu noktada veritabanı sorgusu çalıştırılır ve sonuçlar belleğe alınır
                   .Select(bg => new GirisCikisRaporViewModel
                   {
                       ArabaId = bg.ArabaId ?? 0,
                       ArabaPlaka = bg.Araba?.ArabaPlaka ?? "Bilinmiyor",
                       ArabaBoyut = bg.Araba?.Boyut ?? "Bilinmiyor",
                       OtoparkAdi = GetOtoparkAdi(bg.Bölge.OtoparkId), // Otopark Adını GetOtoparkAdi metodu ile belirliyoruz
                       GirisZamani = bg.GirisZamani ?? DateTime.MinValue,
                       CikisZamani = bg.CikisZamani ?? DateTime.MinValue,
                       Ucret = CalculateUcret(bg) // Ücret hesaplama metodunu çağırıyoruz
                   })
                   .ToList();

            return rapor;
        }

        // Otopark Adını Belirleyen Metod
        private string GetOtoparkAdi(int? otoparkId)
        {
            if (otoparkId == 1)
                return "A OTOPARKI";
            else if (otoparkId == 2)
                return "B OTOPARKI";
            else if (otoparkId == 3)
                return "C OTOPARKI";
            else
                return "Belirtilmemiş";
        }

        // Ücret hesaplama metodu
        public double CalculateUcret(BölgeGiris bolgeGiris)
        {
            if (bolgeGiris == null || bolgeGiris.GirisZamani == null || bolgeGiris.CikisZamani == null)
            {
                Console.WriteLine("GirisZamani veya CikisZamani null. Ücret hesaplanamayacak.");
                return 0;
            }

            // Zaman Hesabı
            TimeSpan parkSuresi = bolgeGiris.CikisZamani.Value - bolgeGiris.GirisZamani.Value;
            double toplamSaat = Math.Ceiling(parkSuresi.TotalHours);

            
            if (toplamSaat <= 0)
            {
                Console.WriteLine("Toplam saat 0 veya daha az");
                return 0;
            }

            // Ücret hesaplamak için boyut ve otopark bilgisi
            var arabaBoyut = bolgeGiris.Araba.Boyut?.Trim().ToLower();//trim ve ToLower ile yaziyi kücültüyoruz.

            var otopark = bolgeGiris.Bölge?.Otopark;

            if (otopark == null)
            {
                Console.WriteLine("Otopark bilgisi null");
                return 0;
            }

            double saatlikUcret = 0;

          
            if (arabaBoyut == "küçük")
            {
                saatlikUcret = otopark.KücükArabaPara ?? 0;
            }
            else if (arabaBoyut == "orta")
            {
                saatlikUcret = otopark.OrtaArabaPara ?? 0;
            }
            else if (arabaBoyut == "büyük")
            {
                saatlikUcret = otopark.BüyükArabaPara ?? 0;
            }
            else
            {
              
                return 0;
            }

            if (saatlikUcret == 0)
            {
                
            }

            double toplamUcret = toplamSaat * saatlikUcret;
          

            return toplamUcret;
        }


        public IEnumerable<OtoparkKazancRaporuViewModel> GetOtoparkKazancRaporu()
        {
            var kazancRaporu = _context.BölgeGirises
                .Include(bg => bg.Bölge)
                .ThenInclude(b => b.Otopark)
                .Where(bg => bg.CikisZamani != null)
                .AsEnumerable()  
                .GroupBy(bg => bg.Bölge.OtoparkId)
                .Select(g => new OtoparkKazancRaporuViewModel
                {
                    OtoparkAdi = GetOtoparkAdi(g.Key), // g.Key ile Otopark adı alınır.
                    ToplamKazanc = g.Sum(bg => CalculateUcret(bg))
                })
                .ToList();

            return kazancRaporu;
        }
    }
}

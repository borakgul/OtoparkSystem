using basics.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace basics.Services
{
    public class BolgeGirisService
    {
        private readonly ArabaParkSistemiContext _context;

        public BolgeGirisService(ArabaParkSistemiContext context)
        {
            _context = context;
        }

        // Aracın park alanına giriş kaydı
        public void BolgeyeGirisYap(BölgeGiris bolgeGiris)
        {
            
            _context.BölgeGirises.Add(bolgeGiris);

            // boş yer sayısını 1 azaltılır
            var bolge = _context.Bölges.FirstOrDefault(b => b.Id == bolgeGiris.BölgeId);
            if (bolge != null)
            {
                switch (bolgeGiris.Araba.Boyut)
                {
                    case "Küçük":
                        bolge.KücükBos -= 1;
                        break;
                    case "Orta":
                        bolge.OrtaBos -= 1;
                        break;
                    case "Büyük":
                        bolge.BüyükBos -= 1;
                        break;
                }
            }

            _context.SaveChanges(); 
        }

        public List<BölgeGiris> GetParkHalindekiArabalar()
        {
            return _context.BölgeGirises
                .Include(bg => bg.Araba)  
                .Include(bg => bg.Bölge)  
                .Where(bg => bg.CikisZamani == null)  // Çıkış yapmamış kayıtları filtreliyoruz
                .ToList();
        }
        [HttpPost]
        public bool AracCikisYap(int arabaId)
        {
            // Çıkış yapılmamış aracı bul
            var bolgeGiris = _context.BölgeGirises
                .Include(bg => bg.Araba)
                .Include(bg => bg.Bölge)
                .FirstOrDefault(bg => bg.ArabaId == arabaId && bg.CikisZamani == null);

            if (bolgeGiris == null)
            {
                return false; // Çıkış yapılacak araç bulunamadı
            }

            // Çıkış zamanını güncelle
            bolgeGiris.CikisZamani = DateTime.Now;

            // Bölgedeki uygun yeri güncelle (boş yer sayısını 1 artır)
            var bolge = bolgeGiris.Bölge;
            if (bolge != null)
            {
                switch (bolgeGiris.Araba.Boyut?.Trim().ToLower())
                {
                    case "küçük":
                        bolge.KücükBos += 1;
                        break;
                    case "orta":
                        bolge.OrtaBos += 1;
                        break;
                    case "büyük":
                        bolge.BüyükBos += 1;
                        break;
                }
            }

            _context.SaveChanges(); // Veritabanına yapılan değişiklikleri kaydediyoruz
            return true;
        }
    }
}

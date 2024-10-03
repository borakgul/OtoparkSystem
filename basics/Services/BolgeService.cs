using basics.Models;
using basics.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace basics.Services
{
    public class BolgeService
    {
        private readonly ArabaParkSistemiContext _context;

        public BolgeService(ArabaParkSistemiContext context)
        {
            _context = context;
        }

        // En uygun bölgeyi bulmak için kullanılan metot (örnek olarak daha önce tanımladığımız)
        public Bölge EnUygunBolgeyiBul(string boyut)
        {
            return _context.Bölges
                .Where(b =>
                    (boyut == "Küçük" && b.KücükBos > 0) ||
                    (boyut == "Orta" && b.OrtaBos > 0) ||
                    (boyut == "Büyük" && b.BüyükBos > 0)
                )
                .OrderBy(b => b.OtoparkId) // Öncelikli olarak belirli otoparklardan tercih edebiliriz
                .FirstOrDefault(); // İlk uygun olan bölgeyi döndür
        }

        public IEnumerable<BosKapasiteViewModel> GetBosKapasiteler()
        {
            return _context.Bölges
                .Include(b => b.Otopark) // Otopark bilgilerini de dahil ediyoruz
                .Select(b => new BosKapasiteViewModel
                {
                    BölgeID = b.Id,
                    OtoparkAdi = b.Otopark.Id == 1 ? "A OTOPARKINDA" :
                                 b.Otopark.Id == 2 ? "B OTOPARKINDA" :
                                 b.Otopark.Id == 3 ? "C OTOPARKINDA" : "Belirtilmemiş",
                    KücükBos = b.KücükBos ?? 0,
                    OrtaBos = b.OrtaBos ?? 0,
                    BüyükBos = b.BüyükBos ?? 0
                })
                .OrderBy(b => b.OtoparkAdi)
                .ToList();
        }

        
    }

}

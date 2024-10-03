using basics.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;

namespace basics.Services
{
    public class ArabaKayitService
    {
        private readonly ArabaParkSistemiContext _context;

        public ArabaKayitService(ArabaParkSistemiContext context)
        {
            _context = context;
        }

        // Yeni bir araba kaydı eklemek için kullanılan metod
        public bool AddAraba(Araba yeniAraba)
        {
            try
            {
                // Yeni araba nesnesini veritabanına ekliyoruz
                _context.Arabas.Add(yeniAraba);
                int affectedRows = _context.SaveChanges();
                Console.WriteLine($"Affected Rows: {affectedRows}");
                // Kayıt başarılıysa true döner
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                // Hata durumunda loglama yapılabilir
                Console.WriteLine($"Araç kaydı sırasında bir hata oluştu: {ex.Message}");
                return false;
            }
        }
    }
}

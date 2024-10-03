using basics.Models;
using System.Collections.Generic;
using System.Linq;

namespace basics.Services { 
    public class OtoparkService
    {
        private readonly ArabaParkSistemiContext _context;

        public OtoparkService(ArabaParkSistemiContext context)
        {
            _context = context;
        }

        public IEnumerable<Otopark> GetAllOtoparkFiyatlari()
        {
            // Otopark tablosundan tüm otoparkları alıyoruz
            return _context.Otoparks.ToList();
        }
    }
}

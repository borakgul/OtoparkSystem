using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using basics.Models;
using basics.Services;
using basics.ViewModels;

namespace basics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly basics.Services.OtoparkService _otoparkService;  
        private readonly ArabaKayitService _arabaKayitService;
        private readonly BolgeService _bolgeService;
        private readonly BolgeGirisService _bolgeGirisService;
        private readonly GirisCikisRaporService _girisCikisRaporService;

        // Constructor
        public HomeController(
            ILogger<HomeController> logger,
            basics.Services.OtoparkService otoparkService, 
            ArabaKayitService arabaKayitService,
            BolgeService bolgeService,
            BolgeGirisService bolgeGirisService,
            GirisCikisRaporService girisCikisRaporService)
        {
            _logger = logger;
            _otoparkService = otoparkService;
            _arabaKayitService = arabaKayitService;
            _bolgeService = bolgeService;
            _bolgeGirisService = bolgeGirisService;
            _girisCikisRaporService = girisCikisRaporService;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            int saat = DateTime.Now.Hour;
            ViewData["selamlama"] = saat > 12 ? "�yi G�nler" : "G�nayd�n";
            ViewData["UserName"] = "Bora";

            // Otopark fiyat bilgilerini al ve ViewBag'e ekle
            var otoparkFiyatlari = _otoparkService.GetAllOtoparkFiyatlari(); // Servisten fiyat bilgilerini al
            var otoparkViewModels = otoparkFiyatlari.Select(o => new OtoparkViewModel
            {
                OtoparkAdi = o.Id switch
                {
                    1 => "A OTOPARKI",
                    2 => "B OTOPARKI",
                    3 => "C OTOPARKI",
                    _ => "Bilinmiyor"
                },
                K�c�kArabaPara = o.K�c�kArabaPara,
                OrtaArabaPara = o.OrtaArabaPara,
                B�y�kArabaPara = o.B�y�kArabaPara
            }).ToList();

            return View(otoparkViewModels);
        }


        public IActionResult ParkHalindekiArabalar()
        {
            // Park halindeki ara�lar� getiriyoruz
            var parkHalindekiArabalar = _bolgeGirisService.GetParkHalindekiArabalar();

            // Bo� kapasiteleri getiriyoruz
            var bosKapasiteler = _bolgeService.GetBosKapasiteler();

            // ViewModel olu�turup verileri bu modele yerle�tiriyoruz
            var model = new ParkHalindekiArabalarViewModel
            {
                ParkHalindekiArabalar = parkHalindekiArabalar,
                BosKapasiteler = bosKapasiteler
            };

            return View(model);
        }

        public IActionResult GirisCikisRapor()
        {
            // Giri� ��k�� raporu ve otopark kazan� raporunu al�yoruz
            var girisCikisRapor = _girisCikisRaporService.GetGirisCikisRapor();
            var otoparkKazancRaporu = _girisCikisRaporService.GetOtoparkKazancRaporu();

            // ViewBag kullanarak verileri View'a g�nderiyoruz
            ViewBag.GirisCikisRapor = girisCikisRapor;
            ViewBag.OtoparkKazancRaporu = otoparkKazancRaporu;

            return View();
        }
    }
}

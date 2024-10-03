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
            ViewData["selamlama"] = saat > 12 ? "Ýyi Günler" : "Günaydýn";
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
                KücükArabaPara = o.KücükArabaPara,
                OrtaArabaPara = o.OrtaArabaPara,
                BüyükArabaPara = o.BüyükArabaPara
            }).ToList();

            return View(otoparkViewModels);
        }


        public IActionResult ParkHalindekiArabalar()
        {
            // Park halindeki araçlarý getiriyoruz
            var parkHalindekiArabalar = _bolgeGirisService.GetParkHalindekiArabalar();

            // Boþ kapasiteleri getiriyoruz
            var bosKapasiteler = _bolgeService.GetBosKapasiteler();

            // ViewModel oluþturup verileri bu modele yerleþtiriyoruz
            var model = new ParkHalindekiArabalarViewModel
            {
                ParkHalindekiArabalar = parkHalindekiArabalar,
                BosKapasiteler = bosKapasiteler
            };

            return View(model);
        }

        public IActionResult GirisCikisRapor()
        {
            // Giriþ çýkýþ raporu ve otopark kazanç raporunu alýyoruz
            var girisCikisRapor = _girisCikisRaporService.GetGirisCikisRapor();
            var otoparkKazancRaporu = _girisCikisRaporService.GetOtoparkKazancRaporu();

            // ViewBag kullanarak verileri View'a gönderiyoruz
            ViewBag.GirisCikisRapor = girisCikisRapor;
            ViewBag.OtoparkKazancRaporu = otoparkKazancRaporu;

            return View();
        }
    }
}

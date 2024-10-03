using basics.Models;
using basics.Services;
using Microsoft.AspNetCore.Mvc;
namespace basics.Controllers
{
	public class AracKayitController : Controller
	{
        private readonly ArabaKayitService _arabaKayitService;
        private readonly BolgeService _bolgeService;
        private readonly BolgeGirisService _bolgeGirisService;
        private readonly ILogger<AracKayitController> _logger;

		public AracKayitController(ILogger<AracKayitController> logger, ArabaKayitService arabaKayitService, BolgeService bolgeService, BolgeGirisService bolgeGirisService)
		{
            _arabaKayitService = arabaKayitService;
            _bolgeService = bolgeService;
            _bolgeGirisService = bolgeGirisService;
            _logger = logger;
		}
		
        [HttpGet]
        public IActionResult AracKayit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AracKayit(ArabaKayitİnfo model)
        {
            if (ModelState.IsValid)
            {
                // 1. Yeni bir Araba nesnesi oluştur
                var yeniAraba = new Araba
                {
                    ArabaPlaka = model.Plaka,
                    Boyut = model.Size
                };

                //  Yeni arabayı veritabanına kaydet
                bool kayitBasarili = _arabaKayitService.AddAraba(yeniAraba);

                if (kayitBasarili)
                {
                    //  Aracın boyutuna uygun en iyi bölgeyi bul
                    var uygunBolge = _bolgeService.EnUygunBolgeyiBul(model.Size);

                    if (uygunBolge != null)
                    {
                        //  Bölgeye araç giriş kaydı yap
                        var bolgeGiris = new BölgeGiris
                        {
                            ArabaId = yeniAraba.Id,
                            BölgeId = uygunBolge.Id,
                            GirisZamani = DateTime.Now
                        };

                        _bolgeGirisService.BolgeyeGirisYap(bolgeGiris);

                       // Kullanıcıya aracın park edildiği yeri bildiren bir mesaj göster ===> yeni bir view'a yollanacak
                        TempData["SuccessMessage"] = $"Araç, {uygunBolge.OtoparkId} numaralı otoparkta, {uygunBolge.Id} numaralı bölgeye park edilmiştir.";
                        return RedirectToAction("AracKayitSonuc");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Uygun bir park yeri bulunamadı.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Araç kaydı yapılamadı. Lütfen tekrar deneyin.");
                }
            }

            // Model geçerli değilse veya uygun bölge bulunamadıysa form tekrar gösterilir
            return View(model);
        }
        public IActionResult AracKayitSonuc()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();
        }
        [HttpGet]
        public IActionResult AracCikis()
        {
            // Henüz çıkış yapmamış park halindeki araçların listesini alıyoruz
            var parkHalindekiArabalar = _bolgeGirisService.GetParkHalindekiArabalar();
            return View(parkHalindekiArabalar);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult AracCikis(int arabaId)
        {
            // Aracın çıkışını gerçekleştir
            bool cikisBasarili = _bolgeGirisService.AracCikisYap(arabaId);

            if (cikisBasarili)
            {
                TempData["SuccessMessage"] = "Araç çıkışı başarıyla gerçekleşti ve bölgedeki boş yer sayısı güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Araç çıkışı yapılamadı. Araç parkta bulunamadı.";
            }

            return RedirectToAction("AracCikisSonuc");
        }

        // Araç Çıkış Sonucu View'ı için Action Method
        public IActionResult AracCikisSonuc()
        {
            return View();
        }

    }

}

using Microsoft.AspNetCore.Mvc;
using AracTakipSistemi.Models;
using Microsoft.EntityFrameworkCore;

namespace AracTakipSistemi.Controllers
{
    public class AnasayfaController : Controller
    {
        private readonly VeriTabani _context;

        public AnasayfaController(VeriTabani context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Kullanıcı giriş kontrolü
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            ViewBag.KullaniciGirisYapti = kullaniciId != null;
            
            // Eğer kullanıcı giriş yaptıysa randevularını getir
            if (kullaniciId != null)
            {
                var randevular = await _context.Randevu
                    .Include(r => r.Arac)
                    .Where(r => r.KullaniciID == kullaniciId.Value)
                    .OrderByDescending(r => r.BaslangicTarihi)
                    .Take(5) // Son 5 randevu
                    .ToListAsync();
                
                return View(randevular);
            }
            
            // Giriş yapmamışsa boş liste döndür
            return View(new List<Randevu>());
        }

        public IActionResult AracDetay(int id)
        {
            // Statik araç detayı - sadece arayüz için
            var arac = new { ID = id, Marka = "Toyota", Model = "Corolla", Plaka = "34 ABC 123", AracTipi = "Sedan", Durumu = "Müsait", VitesTipi = "Manuel", YakitTuru = "Benzin" };
            
            // Statik randevu verileri
            ViewBag.Randevular = new List<dynamic>();
            ViewBag.KullaniciGirisYapti = HttpContext.Session.GetInt32("KullaniciID") != null;
            
            return View(arac);
        }

        [HttpGet]
        public IActionResult AracAra(string marka, string model, string yakitTuru, string vitesTipi)
        {
            // Kullanıcı giriş kontrolü
            ViewBag.KullaniciGirisYapti = HttpContext.Session.GetString("KullaniciId") != null;
            
            // Boş liste döndür (arama sonucu yok)
            var araclar = new List<dynamic>();
            
            return View("Index", araclar);
        }

        public IActionResult UygunTarihler(int aracId, DateTime baslangic, DateTime bitis)
        {
            // Statik kontrol - sadece arayüz için, her zaman uygun döner
            return Json(new { uygun = true, cakisanRandevuSayisi = 0 });
        }

        public IActionResult NasilCalisir()
        {
            return View();
        }

        public IActionResult Hakkimizda()
        {
            return View();
        }
    }
}

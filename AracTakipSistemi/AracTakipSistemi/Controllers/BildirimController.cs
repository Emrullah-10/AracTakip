using AracTakipSistemi.Models;
using AracTakipSistemi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AracTakipSistemi.Controllers
{
    public class BildirimController : Controller
    {
        private readonly BildirimService _bildirimService;

        public BildirimController(BildirimService bildirimService)
        {
            _bildirimService = bildirimService;
        }

        // Bildirimler sayfası
        public async Task<IActionResult> Index()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return RedirectToAction("Login", "Kullanici");
            }

            var bildirimler = await _bildirimService.GetKullaniciBildirimleri(kullaniciId.Value);
            return View(bildirimler);
        }

        // AJAX: Okunmamış bildirim sayısını getir
        [HttpGet]
        public async Task<IActionResult> GetOkunmamisSayisi()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return Json(new { sayi = 0 });
            }

            var sayi = await _bildirimService.GetOkunmamisBildirimSayisi(kullaniciId.Value);
            return Json(new { sayi });
        }

        // AJAX: Okunmamış bildirimleri getir
        [HttpGet]
        public async Task<IActionResult> GetOkunmamisBildirimler()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return Json(new List<object>());
            }

            var bildirimler = await _bildirimService.GetOkunmamisBildirimler(kullaniciId.Value);
            var sonuc = bildirimler.Select(b => new
            {
                id = b.ID,
                baslik = b.Baslik,
                mesaj = b.Mesaj,
                tip = b.Tip,
                tarih = b.OlusturmaTarihi.ToString("dd.MM.yyyy HH:mm")
            }).ToList();

            return Json(sonuc);
        }

        // AJAX: Bildirimi okundu olarak işaretle
        [HttpPost]
        public async Task<IActionResult> BildirimOkundu(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return Json(new { basarili = false });
            }

            await _bildirimService.BildirimOkundu(id);
            return Json(new { basarili = true });
        }

        // AJAX: Tüm bildirimleri okundu olarak işaretle
        [HttpPost]
        public async Task<IActionResult> TumBildirimleriOkundu()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return Json(new { basarili = false });
            }

            await _bildirimService.TumBildirimleriOkundu(kullaniciId.Value);
            return Json(new { basarili = true });
        }

        // AJAX: Bildirimi sil
        [HttpPost]
        public async Task<IActionResult> BildirimSil(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
            {
                return Json(new { basarili = false, mesaj = "Oturum bulunamadı!" });
            }

            try
            {
                await _bildirimService.BildirimSil(id);
                return Json(new { basarili = true, mesaj = "Bildirim başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { basarili = false, mesaj = "Bildirim silinirken bir hata oluştu: " + ex.Message });
            }
        }
    }
} 
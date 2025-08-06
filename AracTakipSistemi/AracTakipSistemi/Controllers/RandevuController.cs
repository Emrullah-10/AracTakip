using Microsoft.AspNetCore.Mvc;
using AracTakipSistemi.Models;
using AracTakipSistemi.Services;
using Microsoft.EntityFrameworkCore;

namespace AracTakipSistemi.Controllers
{
    public class RandevuController : Controller
    {
        private readonly VeriTabani _context;
        private readonly BildirimService _bildirimService;

        public RandevuController(VeriTabani context, BildirimService bildirimService)
        {
            _context = context;
            _bildirimService = bildirimService;
        }

        // Kullanıcı giriş kontrolü
        private bool KullaniciGirisKontrol()
        {
            return HttpContext.Session.GetInt32("KullaniciID") != null;
        }

        public IActionResult Index()
        {
            if (!KullaniciGirisKontrol())
            {
                return RedirectToAction("Login", "Kullanici");
            }

            // Kullanıcının randevularını çek
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            var randevular = _context.Randevu
                .Include(r => r.Arac)
                .Include(r => r.Kullanici)
                .Where(r => r.KullaniciID == kullaniciId)
                .OrderByDescending(r => r.BaslangicTarihi)
                .ToList();

            return View(randevular);
        }

        [HttpGet]
        public IActionResult Olustur()
        {
            if (!KullaniciGirisKontrol())
            {
                return RedirectToAction("Login", "Kullanici");
            }

            return View(new Randevu());
        }

        [HttpPost]
        public async Task<IActionResult> Olustur(Randevu randevu)
        {
            if (!KullaniciGirisKontrol())
            {
                return RedirectToAction("Login", "Kullanici");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kullanıcı ID'sini session'dan al
                    randevu.KullaniciID = HttpContext.Session.GetInt32("KullaniciID").Value;
                    
                    // AracID'yi null olarak ayarla - Admin atasın diye
                    randevu.AracID = null;

                    _context.Randevu.Add(randevu);
                    await _context.SaveChangesAsync();

                    // Admin'lere bildirim gönder
                    await _bildirimService.YeniRandevuTalebiBildirim(randevu.ID);

                    TempData["Basari"] = "Araç talebiniz başarıyla oluşturuldu! Admin onayı ve araç ataması bekleniyor.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Hata"] = "Araç talebi oluşturulurken bir hata oluştu: " + ex.Message;
                }
            }

            return View(randevu);
        }

        public IActionResult Detay(int id)
        {
            if (!KullaniciGirisKontrol())
            {
                return RedirectToAction("Login", "Kullanici");
            }

            // Kullanıcının kendi randevusunu kontrol et
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            var randevu = _context.Randevu
                .Include(r => r.Kullanici)
                .Include(r => r.Arac)
                .FirstOrDefault(r => r.ID == id && r.KullaniciID == kullaniciId);

            if (randevu == null)
            {
                TempData["Hata"] = "Randevu bulunamadı veya bu randevuyu görüntüleme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            return View(randevu);
        }

        [HttpPost]
        public async Task<IActionResult> Iptal(int id)
        {
            if (!KullaniciGirisKontrol())
            {
                return Json(new { success = false, message = "Giriş yapmalısınız!" });
            }

            try
            {
                // Kullanıcının kendi randevusunu kontrol et
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
                var randevu = await _context.Randevu
                    .FirstOrDefaultAsync(r => r.ID == id && r.KullaniciID == kullaniciId);

                if (randevu == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı veya iptal etme yetkiniz yok!" });
                }

                // Sadece bekleyen randevular iptal edilebilir
                if (randevu.OnayDurumu != "Bekliyor")
                {
                    return Json(new { success = false, message = "Sadece onay bekleyen randevular iptal edilebilir!" });
                }

                _context.Randevu.Remove(randevu);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Randevu başarıyla iptal edildi!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Hata oluştu!" });
            }
        }

        // AJAX ile araç müsaitlik kontrolü - statik
        [HttpPost]
        public IActionResult AracMusaitlikKontrol(int aracId, DateTime baslangic, DateTime bitis)
        {
            // Statik kontrol - sadece arayüz için, her zaman müsait döner
            return Json(new { musait = true });
        }

        // Kullanıcının randevu geçmişi - statik
        public IActionResult Gecmis()
        {
            if (!KullaniciGirisKontrol())
            {
                return RedirectToAction("Login", "Kullanici");
            }

            // Statik geçmiş randevu verileri - sadece arayüz için
            var gecmisRandevular = new List<Randevu>();

            return View(gecmisRandevular);
        }

        // En çok kullanılan talep nedenlerini getir
        [HttpGet]
        public IActionResult EnCokKullanilanNedenler()
        {
            try
            {
                // Veritabanından en çok kullanılan 5 talep nedenini getir
                var enCokKullanilanNedenler = _context.Randevu
                    .Where(r => !string.IsNullOrEmpty(r.TalepNedeni))
                    .GroupBy(r => r.TalepNedeni.Trim())
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new { 
                        neden = g.Key, 
                        kullanilmaSayisi = g.Count() 
                    })
                    .ToList();

                // Eğer hiç veri yoksa boş liste döndür
                if (enCokKullanilanNedenler.Count == 0)
                {
                    return Json(new List<object>());
                }

                return Json(enCokKullanilanNedenler);
            }
            catch (Exception)
            {
                // Hata durumunda boş liste döndür
                return Json(new List<object>());
            }
        }
    }
}

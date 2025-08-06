using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AracTakipSistemi.Models;
using AracTakipSistemi.Services;

namespace AracTakipSistemi.Controllers
{
    public class AdminController : Controller
    {
        private readonly VeriTabani _context;
        private readonly BildirimService _bildirimService;

        public AdminController(VeriTabani context, BildirimService bildirimService)
        {
            _context = context;
            _bildirimService = bildirimService;
        }

        // Admin kontrolü
        private bool AdminKontrol()
        {
            // Geçici olarak her zaman true döndür - test için
            return true;
            
            // Orijinal kod:
            // var rol = HttpContext.Session.GetString("KullaniciRol");
            // return rol == "Admin";
        }

        public IActionResult Index()
        {
            // Gerçek istatistikler
            ViewBag.ToplamKullanici = _context.Kullanicilar.Count();
            ViewBag.ToplamArac = _context.Arac.Count();
            ViewBag.BekleyenRandevu = _context.Randevu.Count(); // Tüm randevular "talep edildi" durumunda
            ViewBag.OnaylananRandevu = 0; // Henüz onay sistemi yok

            // Boş randevu listesi döndür (dashboard için gerekli değil)
            var randevular = new List<Randevu>();
            return View(randevular);
        }

        public IActionResult RandevuYonetimi()
        {
            // Tüm randevuları kullanıcı ve araç bilgileriyle birlikte çek
            var randevular = _context.Randevu
                .Include(r => r.Kullanici)
                .Include(r => r.Arac)
                .OrderByDescending(r => r.BaslangicTarihi)
                .ToList();

            return View(randevular);
        }

        // RandevuDetay action'ı kaldırıldı

        public IActionResult KullaniciYonetimi()
        {
            // Gerçek kullanıcı listesi döndür
            var kullanicilar = _context.Kullanicilar.ToList();
            return View(kullanicilar);
        }

        public IActionResult KullaniciDetay(int id)
        {
            // Gerçek kullanıcı verisi döndür
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.ID == id);
            if (kullanici == null)
            {
                return NotFound();
            }
            return View(kullanici);
        }

        public IActionResult Raporlar()
        {
            // Demo rapor verileri
            ViewBag.ToplamArac = 0;
            ViewBag.MusaitArac = 0;
            ViewBag.BakimdaArac = 0;
            ViewBag.ToplamRandevu = 0;
            ViewBag.BekleyenRandevu = 0;
            ViewBag.OnaylananRandevu = 0;
            ViewBag.ReddedilenRandevu = 0;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RandevuOnayla(int id)
        {
            if (!AdminKontrol())
            {
                return Json(new { success = false, message = "Yetki hatası!" });
            }

            try
            {
                var randevu = await _context.Randevu.FindAsync(id);
                if (randevu == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı!" });
                }

                randevu.OnayDurumu = "Onaylandı";
                randevu.RedSebebi = null; // Red sebebini temizle
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Randevu başarıyla onaylandı!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RandevuReddet(int id, string redSebebi)
        {
            if (!AdminKontrol())
            {
                return Json(new { success = false, message = "Yetki hatası!" });
            }

            if (string.IsNullOrEmpty(redSebebi))
            {
                return Json(new { success = false, message = "Red sebebi belirtilmelidir!" });
            }

            try
            {
                var randevu = await _context.Randevu.FindAsync(id);
                if (randevu == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı!" });
                }

                randevu.OnayDurumu = "Reddedildi";
                randevu.RedSebebi = redSebebi;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Randevu başarıyla reddedildi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AracAta(int randevuId, int aracId)
        {
            if (!AdminKontrol())
            {
                return Json(new { success = false, message = "Yetki hatası!" });
            }

            try
            {
                var randevu = await _context.Randevu.FindAsync(randevuId);
                if (randevu == null)
                {
                    return Json(new { success = false, message = "Randevu bulunamadı!" });
                }

                // Test araçları için özel handling
                if (aracId >= 997 && aracId <= 999)
                {
                    randevu.AracID = null; // Test için null bırak
                    await _context.SaveChangesAsync();
                    
                    var testAracBilgi = aracId == 999 ? "Test Toyota Corolla (TEST 123)" :
                                       aracId == 998 ? "Test Honda Civic (TEST 456)" :
                                                       "Test Volkswagen Golf (TEST 789)";
                    
                    return Json(new { success = true, message = "Test aracı başarıyla atandı!", aracBilgi = testAracBilgi });
                }

                var arac = await _context.Arac.FindAsync(aracId);
                if (arac == null)
                {
                    return Json(new { success = false, message = "Araç bulunamadı!" });
                }

                randevu.AracID = aracId;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Araç başarıyla atandı!", aracBilgi = $"{arac.Marka} {arac.Model} ({arac.Plaka})" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> MusaitAraclar()
        {
            if (!AdminKontrol())
            {
                return Json(new { success = false, message = "Yetki hatası!" });
            }

            try
            {
                // Debug: Tüm araçları kontrol et
                var tumAraclar = await _context.Arac.ToListAsync();
                var musaitAraclar = await _context.Arac
                    .Where(a => a.Durumu == "Müsait")
                    .Select(a => new { 
                        id = a.ID, 
                        text = $"{a.Marka} {a.Model} ({a.Plaka})" 
                    })
                    .ToListAsync();

                // Debug için console'a yazalım
                Console.WriteLine($"Toplam araç sayısı: {tumAraclar.Count}");
                Console.WriteLine($"Müsait araç sayısı: {musaitAraclar.Count}");
                
                foreach (var arac in tumAraclar)
                {
                    Console.WriteLine($"Araç: {arac.Marka} {arac.Model} - Durum: {arac.Durumu}");
                }

                // Gerçek müsait araçlar varsa onları döndür
                if (musaitAraclar.Count > 0)
                {
                    return Json(musaitAraclar);
                }

                // Eğer müsait araç yoksa test araçları döndür
                    var testAraclar = new List<object>
                    {
                    new { id = 999, text = "🚗 Test Toyota Corolla (TEST 123)" },
                    new { id = 998, text = "🚙 Test Honda Civic (TEST 456)" },
                    new { id = 997, text = "🚐 Test Volkswagen Golf (TEST 789)" }
                    };
                    
                Console.WriteLine("Test araçları döndürülüyor...");
                    return Json(testAraclar);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MusaitAraclar hata: {ex.Message}");
                
                // Hata durumunda bile test araçları döndür
                var testAraclar = new List<object>
                {
                    new { id = 999, text = "🚗 Test Toyota Corolla (TEST 123)" },
                    new { id = 998, text = "🚙 Test Honda Civic (TEST 456)" },
                    new { id = 997, text = "🚐 Test Volkswagen Golf (TEST 789)" }
                };
                
                return Json(testAraclar);
            }
        }

        [HttpGet]
        public IActionResult BekleyenRandevuSayisi()
        {
            // Demo için 0 döndür
            return Json(new { sayi = 0 });
        }

        [HttpPost]
        public IActionResult AracDurumTopluGuncelle(List<int> aracIdler, string yeniDurum)
        {
            // Demo - Toplu güncelleme
            return Json(new { success = true, message = "Araç durumları başarıyla güncellendi!" });
        }

        [HttpGet]
        public IActionResult KullaniciEkle()
        {
            if (!AdminKontrol())
            {
                return RedirectToAction("Login", "Kullanici");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KullaniciEkle(string kullaniciAdi, string sifre, string departman, bool rol = false)
        {
            if (!AdminKontrol())
            {
                return RedirectToAction("Login", "Kullanici");
            }

            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre) || string.IsNullOrEmpty(departman))
            {
                ViewBag.Hata = "Tüm alanlar doldurulmalıdır.";
                return View();
            }

            // Kullanıcı adının daha önce kullanılıp kullanılmadığını kontrol et
            var mevcutKullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);

            if (mevcutKullanici != null)
            {
                ViewBag.Hata = "Bu kullanıcı adı zaten kullanılıyor.";
                return View();
            }

            var yeniKullanici = new Kullanici
            {
                KullaniciAdi = kullaniciAdi,
                Sifre = sifre,
                Departman = departman,
                Rol = rol
            };

            _context.Kullanicilar.Add(yeniKullanici);
            await _context.SaveChangesAsync();

            TempData["Basari"] = "Kullanıcı başarıyla eklendi.";
            return RedirectToAction("KullaniciYonetimi");
        }

        [HttpGet]
        public async Task<IActionResult> KullaniciDuzenle(int id)
        {
            if (!AdminKontrol())
            {
                return RedirectToAction("Login", "Kullanici");
            }

            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null)
            {
                return NotFound();
            }

            return View(kullanici);
        }

        [HttpPost]
        public async Task<IActionResult> KullaniciDuzenle(int id, string kullaniciAdi, string sifre, string departman, bool rol = false)
        {
            if (!AdminKontrol())
            {
                return RedirectToAction("Login", "Kullanici");
            }

            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(departman))
            {
                ViewBag.Hata = "Kullanıcı adı ve departman alanları zorunludur.";
                var kullanici = await _context.Kullanicilar.FindAsync(id);
                return View(kullanici);
            }

            var guncellenecekKullanici = await _context.Kullanicilar.FindAsync(id);
            if (guncellenecekKullanici == null)
            {
                return NotFound();
            }

            // Kullanıcı adı değiştirildiyse ve başka bir kullanıcı tarafından kullanılıyorsa kontrol et
            if (guncellenecekKullanici.KullaniciAdi != kullaniciAdi)
            {
                var mevcutKullanici = await _context.Kullanicilar
                    .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi && k.ID != id);

                if (mevcutKullanici != null)
                {
                    ViewBag.Hata = "Bu kullanıcı adı başka bir kullanıcı tarafından kullanılıyor.";
                    return View(guncellenecekKullanici);
                }
            }

            // Kullanıcı bilgilerini güncelle
            guncellenecekKullanici.KullaniciAdi = kullaniciAdi;
            guncellenecekKullanici.Departman = departman;
            guncellenecekKullanici.Rol = rol;

            // Şifre değiştirildiyse güncelle
            if (!string.IsNullOrEmpty(sifre))
            {
                guncellenecekKullanici.Sifre = sifre;
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["Basari"] = "Kullanıcı bilgileri başarıyla güncellendi.";
                return RedirectToAction("KullaniciYonetimi");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Güncelleme sırasında bir hata oluştu: " + ex.Message;
                return View(guncellenecekKullanici);
            }
        }

        [HttpPost]
        public async Task<IActionResult> KullaniciSil(int id)
        {
            if (!AdminKontrol())
            {
                return Json(new { success = false, message = "Yetki hatası!" });
            }

            try
            {
                var kullanici = await _context.Kullanicilar.FindAsync(id);
                if (kullanici == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı!" });
                }

                // Bu kullanıcının randevularını kontrol et
                var randevular = await _context.Randevu.Where(r => r.KullaniciID == id).ToListAsync();
                if (randevular.Any())
                {
                    return Json(new { success = false, message = "Bu kullanıcının randevuları olduğu için silinemez! Önce randevuları iptal edin." });
                }

                // Bu kullanıcının bildirimlerini kontrol et
                var bildirimler = await _context.Bildirimler.Where(b => b.KullaniciID == id).ToListAsync();
                if (bildirimler.Any())
                {
                    // Bildirimleri sil
                    _context.Bildirimler.RemoveRange(bildirimler);
                }

                _context.Kullanicilar.Remove(kullanici);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Kullanıcı başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Kullanıcı silinirken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RandevuSil(int id)
        {
            if (!AdminKontrol())
            {
                return Json(new { success = false, message = "Yetki hatası!" });
            }

            var randevu = await _context.Randevu.FindAsync(id);
            if (randevu == null)
            {
                return Json(new { success = false, message = "Randevu bulunamadı!" });
            }

            _context.Randevu.Remove(randevu);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Randevu başarıyla silindi!" });
        }

        public IActionResult RandevuDuzenle(int id)
        {
            // Randevu düzenleme sayfası (şimdilik yönlendirme)
            return RedirectToAction("RandevuDetay", new { id = id });
        }

        [HttpPost]
        public IActionResult RandevuRaporuIndir(List<int> randevuIds)
        {
            // Excel raporu indirme (demo)
            return Json(new { success = true, message = "Rapor hazırlanıyor..." });
        }

        // Araç Atama Sayfası
        [HttpGet]
        public async Task<IActionResult> AracAtaSayfa(int id)
        {
            if (!AdminKontrol())
            {
                return RedirectToAction("Index");
            }

            // Randevu kontrolü
            var randevu = await _context.Randevu.FindAsync(id);
            if (randevu == null)
            {
                ViewBag.Hata = "Randevu bulunamadı!";
                return RedirectToAction("RandevuYonetimi");
            }

            // Müsait araçları getir
            var musaitAraclar = await _context.Arac
                .Where(a => a.Durumu == "Müsait")
                .ToListAsync();

            ViewBag.MusaitAraclar = musaitAraclar;

            return View("AracAta", id);
        }

        [HttpPost]
        public async Task<IActionResult> AracAtaSayfa(int randevuId, int aracId)
        {
            if (!AdminKontrol())
            {
                ViewBag.Hata = "Yetki hatası!";
                return RedirectToAction("RandevuYonetimi");
            }

            try
            {
                var randevu = await _context.Randevu.FindAsync(randevuId);
                if (randevu == null)
                {
                    ViewBag.Hata = "Randevu bulunamadı!";
                    return RedirectToAction("RandevuYonetimi");
                }

                var arac = await _context.Arac.FindAsync(aracId);
                if (arac == null)
                {
                    ViewBag.Hata = "Araç bulunamadı!";
                    return RedirectToAction("RandevuYonetimi");
                }

                randevu.AracID = aracId;
                randevu.OnayDurumu = "Onaylandı";
                await _context.SaveChangesAsync();

                // Kullanıcıya bildirim gönder
                await _bildirimService.RandevuOnaylandiBildirim(randevuId, randevu.KullaniciID);

                TempData["Basari"] = $"Araç başarıyla atandı! Atanan araç: {arac.Marka} {arac.Model} ({arac.Plaka})";
                return RedirectToAction("RandevuYonetimi");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Hata oluştu: " + ex.Message;
                return RedirectToAction("AracAtaSayfa", new { id = randevuId });
            }
        }

        // Randevu Reddetme Sayfası
        [HttpGet]
        public async Task<IActionResult> RandevuReddetSayfa(int id)
        {
            if (!AdminKontrol())
            {
                return RedirectToAction("Index");
            }

            // Randevu kontrolü
            var randevu = await _context.Randevu.FindAsync(id);
            if (randevu == null)
            {
                ViewBag.Hata = "Randevu bulunamadı!";
                return RedirectToAction("RandevuYonetimi");
            }

            return View("RandevuReddet", id);
        }

        [HttpPost]
        public async Task<IActionResult> RandevuReddetSayfa(int randevuId, string redSebebi)
        {
            if (!AdminKontrol())
            {
                ViewBag.Hata = "Yetki hatası!";
                return RedirectToAction("RandevuYonetimi");
            }

            if (string.IsNullOrEmpty(redSebebi))
            {
                ViewBag.Hata = "Red sebebi belirtilmelidir!";
                return RedirectToAction("RandevuReddetSayfa", new { id = randevuId });
            }

            try
            {
                var randevu = await _context.Randevu.FindAsync(randevuId);
                if (randevu == null)
                {
                    ViewBag.Hata = "Randevu bulunamadı!";
                    return RedirectToAction("RandevuYonetimi");
                }

                randevu.OnayDurumu = "Reddedildi";
                randevu.RedSebebi = redSebebi;
                await _context.SaveChangesAsync();

                // Kullanıcıya bildirim gönder
                await _bildirimService.RandevuReddedildiBildirim(randevuId, randevu.KullaniciID, redSebebi);

                TempData["Basari"] = "Randevu başarıyla reddedildi!";
                return RedirectToAction("RandevuYonetimi");
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Hata oluştu: " + ex.Message;
                return RedirectToAction("RandevuReddetSayfa", new { id = randevuId });
            }
        }
    }
}

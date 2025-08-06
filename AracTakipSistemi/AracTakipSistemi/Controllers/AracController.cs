using Microsoft.AspNetCore.Mvc;
using AracTakipSistemi.Models;
using Microsoft.EntityFrameworkCore;

namespace AracTakipSistemi.Controllers
{
    public class AracController : Controller
    {
        private readonly VeriTabani _context;

        public AracController(VeriTabani context)
        {
            _context = context;
        }

        // Admin kontrolü
        private bool AdminKontrol()
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            return rol == "Admin";
        }

        public IActionResult Index()
        {
            // Veri tabanından araçları çek
            var araclar = _context.Arac.ToList();
            return View(araclar);
        }

        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(Arac arac)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Arac.Add(arac);
                    _context.SaveChanges();
                    TempData["Basari"] = "Araç başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Detaylı hata mesajını almak için inner exception'ları da dahil et
                    var hataDetay = ex.Message;
                    if (ex.InnerException != null)
                    {
                        hataDetay += " Inner Exception: " + ex.InnerException.Message;
                        if (ex.InnerException.InnerException != null)
                        {
                            hataDetay += " Inner Inner Exception: " + ex.InnerException.InnerException.Message;
                        }
                    }
                    TempData["Hata"] = "Araç eklenirken bir hata oluştu: " + hataDetay;
                    return View(arac);
                }
            }
            return View(arac);
        }

        public IActionResult Duzenle(int id)
        {
            var arac = _context.Arac.Find(id);
            if (arac == null)
            {
                TempData["Hata"] = "Araç bulunamadı!";
                return RedirectToAction("Index");
            }
            return View(arac);
        }

        [HttpPost]
        public IActionResult Duzenle(Arac arac)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Arac.Update(arac);
                    _context.SaveChanges();
                    TempData["Basari"] = "Araç başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Hata"] = "Araç güncellenirken bir hata oluştu: " + ex.Message;
                    return View(arac);
                }
            }
            return View(arac);
        }



        [HttpPost]
        public IActionResult Sil(int id)
        {
            try
            {
                var arac = _context.Arac.Find(id);
                if (arac == null)
                {
                    return Json(new { success = false, message = "Araç bulunamadı!" });
                }
                
                // Bu aracın kullanıldığı randevuları kontrol et
                var randevular = _context.Randevu.Where(r => r.AracID == id).ToList();
                if (randevular.Any())
                {
                    return Json(new { success = false, message = "Bu araç randevularda kullanıldığı için silinemez! Önce randevuları iptal edin." });
                }
                
                _context.Arac.Remove(arac);
                _context.SaveChanges();
                return Json(new { success = true, message = "Araç başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Araç silinirken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DurumDegistir(int id, string yeniDurum)
        {
            try
            {
                var arac = _context.Arac.Find(id);
                if (arac == null)
                {
                    return Json(new { success = false, message = "Araç bulunamadı!" });
                }
                
                arac.Durumu = yeniDurum;
                _context.SaveChanges();
                return Json(new { success = true, message = "Araç durumu başarıyla güncellendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Araç durumu güncellenirken bir hata oluştu: " + ex.Message });
            }
        }
    }
}

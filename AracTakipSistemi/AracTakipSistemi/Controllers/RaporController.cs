using Microsoft.AspNetCore.Mvc;
using AracTakipSistemi.Models;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace AracTakipSistemi.Controllers
{
    public class RaporController : Controller
    {
        private readonly VeriTabani _context;

        public RaporController(VeriTabani context)
        {
            _context = context;
        }

        // Admin kontrolü
        private bool AdminKontrol()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
                return false;

            var kullanici = _context.Kullanicilar.Find(kullaniciId);
            return kullanici != null && kullanici.Rol == true;
        }

        public IActionResult Index()
        {
            if (!AdminKontrol())
            {
                TempData["Hata"] = "Bu sayfaya erişim yetkiniz bulunmamaktadır.";
                return RedirectToAction("Login", "Kullanici");
            }

            var model = new RaporIndexViewModel
            {
                ToplamAracSayisi = _context.Arac.Count(),
                MusaitAracSayisi = _context.Arac.Count(a => a.Durumu == "Müsait"),
                KullanimdaAracSayisi = _context.Arac.Count(a => a.Durumu != "Müsait"),
                ToplamKullaniciSayisi = _context.Kullanicilar.Count(),
                ToplamRandevuSayisi = _context.Randevu.Count(),
                BekleyenRandevuSayisi = _context.Randevu.Count(r => r.OnayDurumu == "Bekliyor"),
                OnaylananRandevuSayisi = _context.Randevu.Count(r => r.OnayDurumu == "Onaylandı"),
                ReddedilenRandevuSayisi = _context.Randevu.Count(r => r.OnayDurumu == "Reddedildi")
            };

            return View(model);
        }

        public IActionResult AracKullanimRaporu(RaporFiltre? filtre)
        {
            if (!AdminKontrol())
            {
                TempData["Hata"] = "Bu sayfaya erişim yetkiniz bulunmamaktadır.";
                return RedirectToAction("Login", "Kullanici");
            }

            var query = _context.Arac
                .GroupJoin(_context.Randevu, a => a.ID, r => r.AracID, (arac, randevular) => new { arac, randevular })
                .SelectMany(x => x.randevular.DefaultIfEmpty(), (x, randevu) => new { x.arac, randevu });

            // Filtreleme
            if (filtre != null)
            {
                if (filtre.BaslangicTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi >= filtre.BaslangicTarihi);
                if (filtre.BitisTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi <= filtre.BitisTarihi);
                if (!string.IsNullOrEmpty(filtre.OnayDurumu))
                    query = query.Where(x => x.randevu == null || x.randevu.OnayDurumu == filtre.OnayDurumu);
            }

            var rapor = query
                .GroupBy(x => new { x.arac.ID, x.arac.Marka, x.arac.Model, x.arac.Plaka, x.arac.Durumu })
                .Select(g => new AracKullanimRaporu
                {
                    AracID = g.Key.ID,
                    AracBilgisi = g.Key.Marka + " " + g.Key.Model + " (" + g.Key.Plaka + ")",
                    Marka = g.Key.Marka,
                    Model = g.Key.Model,
                    Plaka = g.Key.Plaka,
                    Durumu = g.Key.Durumu,
                    ToplamRandevuSayisi = g.Count(x => x.randevu != null),
                    ToplamMesafe = g.Where(x => x.randevu != null).Sum(x => x.randevu.GidilecekMesafe),
                    ToplamKisiSayisi = g.Where(x => x.randevu != null).Sum(x => x.randevu.KisiSayisi),
                    SonKullanimTarihi = g.Where(x => x.randevu != null).Max(x => (DateTime?)x.randevu.BaslangicTarihi)
                })
                .OrderByDescending(x => x.ToplamRandevuSayisi)
                .ToList();

            ViewBag.Filtre = filtre ?? new RaporFiltre();
            ViewBag.Araclar = _context.Arac.ToList();
            
            return View(rapor);
        }

        public IActionResult KullaniciAktiviteRaporu(RaporFiltre? filtre)
        {
            if (!AdminKontrol())
            {
                TempData["Hata"] = "Bu sayfaya erişim yetkiniz bulunmamaktadır.";
                return RedirectToAction("Login", "Kullanici");
            }

            var query = _context.Kullanicilar
                .GroupJoin(_context.Randevu, k => k.ID, r => r.KullaniciID, (kullanici, randevular) => new { kullanici, randevular })
                .SelectMany(x => x.randevular.DefaultIfEmpty(), (x, randevu) => new { x.kullanici, randevu });

            // Filtreleme
            if (filtre != null)
            {
                if (filtre.BaslangicTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi >= filtre.BaslangicTarihi);
                if (filtre.BitisTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi <= filtre.BitisTarihi);
                if (!string.IsNullOrEmpty(filtre.Departman))
                    query = query.Where(x => x.kullanici.Departman == filtre.Departman);
                if (!string.IsNullOrEmpty(filtre.OnayDurumu))
                    query = query.Where(x => x.randevu == null || x.randevu.OnayDurumu == filtre.OnayDurumu);
            }

            var rapor = query
                .GroupBy(x => new { x.kullanici.ID, x.kullanici.KullaniciAdi, x.kullanici.Departman })
                .Select(g => new KullaniciAktiviteRaporu
                {
                    KullaniciID = g.Key.ID,
                    KullaniciAdi = g.Key.KullaniciAdi,
                    Departman = g.Key.Departman,
                    ToplamRandevuSayisi = g.Count(x => x.randevu != null),
                    OnaylananRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Onaylandı"),
                    BekleyenRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Bekliyor"),
                    ReddedilenRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Reddedildi"),
                    ToplamMesafe = g.Where(x => x.randevu != null).Sum(x => x.randevu.GidilecekMesafe),
                    SonRandevuTarihi = g.Where(x => x.randevu != null).Max(x => (DateTime?)x.randevu.BaslangicTarihi)
                })
                .OrderByDescending(x => x.ToplamRandevuSayisi)
                .ToList();

            ViewBag.Filtre = filtre ?? new RaporFiltre();
            ViewBag.Departmanlar = _context.Kullanicilar.Select(k => k.Departman).Distinct().ToList();
            ViewBag.Kullanicilar = _context.Kullanicilar.ToList();
            
            return View(rapor);
        }

        public IActionResult DepartmanRaporu(RaporFiltre? filtre)
        {
            if (!AdminKontrol())
            {
                TempData["Hata"] = "Bu sayfaya erişim yetkiniz bulunmamaktadır.";
                return RedirectToAction("Login", "Kullanici");
            }

            var query = _context.Kullanicilar
                .GroupJoin(_context.Randevu, k => k.ID, r => r.KullaniciID, (kullanici, randevular) => new { kullanici, randevular })
                .SelectMany(x => x.randevular.DefaultIfEmpty(), (x, randevu) => new { x.kullanici, randevu });

            // Filtreleme
            if (filtre != null)
            {
                if (filtre.BaslangicTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi >= filtre.BaslangicTarihi);
                if (filtre.BitisTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi <= filtre.BitisTarihi);
                if (!string.IsNullOrEmpty(filtre.OnayDurumu))
                    query = query.Where(x => x.randevu == null || x.randevu.OnayDurumu == filtre.OnayDurumu);
            }

            var rapor = query
                .GroupBy(x => x.kullanici.Departman)
                .Select(g => new DepartmanRaporu
                {
                    Departman = g.Key,
                    KullaniciSayisi = g.Select(x => x.kullanici.ID).Distinct().Count(),
                    ToplamRandevuSayisi = g.Count(x => x.randevu != null),
                    OnaylananRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Onaylandı"),
                    BekleyenRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Bekliyor"),
                    ReddedilenRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Reddedildi"),
                    ToplamMesafe = g.Where(x => x.randevu != null).Sum(x => x.randevu.GidilecekMesafe),
                    ToplamKisiSayisi = g.Where(x => x.randevu != null).Sum(x => x.randevu.KisiSayisi)
                })
                .OrderByDescending(x => x.ToplamRandevuSayisi)
                .ToList();

            ViewBag.Filtre = filtre ?? new RaporFiltre();
            ViewBag.Departmanlar = _context.Kullanicilar.Select(k => k.Departman).Distinct().ToList();
            
            return View(rapor);
        }

        public IActionResult ZamanBazliRapor(RaporFiltre? filtre)
        {
            if (!AdminKontrol())
            {
                TempData["Hata"] = "Bu sayfaya erişim yetkiniz bulunmamaktadır.";
                return RedirectToAction("Login", "Kullanici");
            }

            var baslangic = filtre?.BaslangicTarihi ?? DateTime.Now.AddMonths(-1);
            var bitis = filtre?.BitisTarihi ?? DateTime.Now;

            var query = _context.Randevu.Where(r => r.BaslangicTarihi >= baslangic && r.BaslangicTarihi <= bitis);

            if (filtre != null)
            {
                if (!string.IsNullOrEmpty(filtre.OnayDurumu))
                    query = query.Where(r => r.OnayDurumu == filtre.OnayDurumu);
                if (!string.IsNullOrEmpty(filtre.Departman))
                    query = query.Include(r => r.Kullanici).Where(r => r.Kullanici.Departman == filtre.Departman);
            }

            var rapor = query
                .GroupBy(r => r.BaslangicTarihi.Date)
                .Select(g => new ZamanBazliRaporu
                {
                    Tarih = g.Key,
                    TarihString = g.Key.ToString("dd/MM/yyyy"),
                    RandevuSayisi = g.Count(),
                    OnaylananSayisi = g.Count(r => r.OnayDurumu == "Onaylandı"),
                    BekleyenSayisi = g.Count(r => r.OnayDurumu == "Bekliyor"),
                    ReddedilenSayisi = g.Count(r => r.OnayDurumu == "Reddedildi"),
                    ToplamMesafe = g.Sum(r => r.GidilecekMesafe),
                    ToplamKisiSayisi = g.Sum(r => r.KisiSayisi)
                })
                .OrderBy(x => x.Tarih)
                .ToList();

            ViewBag.Filtre = filtre ?? new RaporFiltre { BaslangicTarihi = baslangic, BitisTarihi = bitis };
            ViewBag.Departmanlar = _context.Kullanicilar.Select(k => k.Departman).Distinct().ToList();
            
            return View(rapor);
        }

        [HttpPost]
        public IActionResult ExcelRapor(string raporTuru, RaporFiltre? filtre)
        {
            if (!AdminKontrol())
            {
                return Json(new { success = false, message = "Yetkiniz bulunmamaktadır." });
            }

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Rapor");
                    
                    switch (raporTuru)
                    {
                        case "AracKullanimRaporu":
                            ExcelAracKullanimRaporu(worksheet, filtre);
                            break;
                        case "KullaniciAktiviteRaporu":
                            ExcelKullaniciAktiviteRaporu(worksheet, filtre);
                            break;
                        case "DepartmanRaporu":
                            ExcelDepartmanRaporu(worksheet, filtre);
                            break;
                        case "ZamanBazliRapor":
                            ExcelZamanBazliRapor(worksheet, filtre);
                            break;
                        default:
                            return Json(new { success = false, message = "Geçersiz rapor türü." });
                    }

                    // Excel dosyasını byte array olarak hazırla
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var fileBytes = stream.ToArray();
                        var fileName = $"{raporTuru}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                        
                        // Excel dosyasını indirme olarak gönder
                        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Excel oluşturulurken hata oluştu: " + ex.Message });
            }
        }

        private void ExcelAracKullanimRaporu(IXLWorksheet worksheet, RaporFiltre? filtre)
        {
            worksheet.Cell(1, 1).Value = "Araç Kullanım Raporu";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 8).Merge();

            // Başlıklar
            worksheet.Cell(3, 1).Value = "Araç Bilgisi";
            worksheet.Cell(3, 2).Value = "Marka";
            worksheet.Cell(3, 3).Value = "Model";
            worksheet.Cell(3, 4).Value = "Plaka";
            worksheet.Cell(3, 5).Value = "Durumu";
            worksheet.Cell(3, 6).Value = "Toplam Randevu";
            worksheet.Cell(3, 7).Value = "Toplam Mesafe (km)";
            worksheet.Cell(3, 8).Value = "Son Kullanım";

            // Başlık satırını kalın yap
            worksheet.Row(3).Style.Font.Bold = true;
            worksheet.Row(3).Style.Fill.BackgroundColor = XLColor.LightGray;

            // Verileri al
            var query = _context.Arac
                .GroupJoin(_context.Randevu, a => a.ID, r => r.AracID, (arac, randevular) => new { arac, randevular })
                .SelectMany(x => x.randevular.DefaultIfEmpty(), (x, randevu) => new { x.arac, randevu });

            if (filtre != null)
            {
                if (filtre.BaslangicTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi >= filtre.BaslangicTarihi);
                if (filtre.BitisTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi <= filtre.BitisTarihi);
                if (!string.IsNullOrEmpty(filtre.OnayDurumu))
                    query = query.Where(x => x.randevu == null || x.randevu.OnayDurumu == filtre.OnayDurumu);
            }

            var rapor = query
                .GroupBy(x => new { x.arac.ID, x.arac.Marka, x.arac.Model, x.arac.Plaka, x.arac.Durumu })
                .Select(g => new AracKullanimRaporu
                {
                    AracBilgisi = g.Key.Marka + " " + g.Key.Model + " (" + g.Key.Plaka + ")",
                    Marka = g.Key.Marka,
                    Model = g.Key.Model,
                    Plaka = g.Key.Plaka,
                    Durumu = g.Key.Durumu,
                    ToplamRandevuSayisi = g.Count(x => x.randevu != null),
                    ToplamMesafe = g.Where(x => x.randevu != null).Sum(x => x.randevu.GidilecekMesafe),
                    SonKullanimTarihi = g.Where(x => x.randevu != null).Max(x => (DateTime?)x.randevu.BaslangicTarihi)
                })
                .OrderByDescending(x => x.ToplamRandevuSayisi)
                .ToList();

            // Verileri Excel'e yaz
            int row = 4;
            foreach (var item in rapor)
            {
                worksheet.Cell(row, 1).Value = item.AracBilgisi;
                worksheet.Cell(row, 2).Value = item.Marka;
                worksheet.Cell(row, 3).Value = item.Model;
                worksheet.Cell(row, 4).Value = item.Plaka;
                worksheet.Cell(row, 5).Value = item.Durumu;
                worksheet.Cell(row, 6).Value = item.ToplamRandevuSayisi;
                worksheet.Cell(row, 7).Value = item.ToplamMesafe;
                worksheet.Cell(row, 8).Value = item.SonKullanimTarihi?.ToString("dd/MM/yyyy") ?? "-";
                row++;
            }

            // Sütun genişliklerini ayarla
            worksheet.Columns().AdjustToContents();
        }

        private void ExcelKullaniciAktiviteRaporu(IXLWorksheet worksheet, RaporFiltre? filtre)
        {
            worksheet.Cell(1, 1).Value = "Kullanıcı Aktivite Raporu";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 8).Merge();

            // Başlıklar
            worksheet.Cell(3, 1).Value = "Kullanıcı Adı";
            worksheet.Cell(3, 2).Value = "Departman";
            worksheet.Cell(3, 3).Value = "Toplam Randevu";
            worksheet.Cell(3, 4).Value = "Onaylandı";
            worksheet.Cell(3, 5).Value = "Bekliyor";
            worksheet.Cell(3, 6).Value = "Reddedildi";
            worksheet.Cell(3, 7).Value = "Toplam Mesafe (km)";
            worksheet.Cell(3, 8).Value = "Son Randevu";

            // Başlık satırını kalın yap
            worksheet.Row(3).Style.Font.Bold = true;
            worksheet.Row(3).Style.Fill.BackgroundColor = XLColor.LightGray;

            // Verileri al
            var query = _context.Kullanicilar
                .GroupJoin(_context.Randevu, k => k.ID, r => r.KullaniciID, (kullanici, randevular) => new { kullanici, randevular })
                .SelectMany(x => x.randevular.DefaultIfEmpty(), (x, randevu) => new { x.kullanici, randevu });

            if (filtre != null)
            {
                if (filtre.BaslangicTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi >= filtre.BaslangicTarihi);
                if (filtre.BitisTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi <= filtre.BitisTarihi);
                if (!string.IsNullOrEmpty(filtre.Departman))
                    query = query.Where(x => x.kullanici.Departman == filtre.Departman);
                if (!string.IsNullOrEmpty(filtre.OnayDurumu))
                    query = query.Where(x => x.randevu == null || x.randevu.OnayDurumu == filtre.OnayDurumu);
            }

            var rapor = query
                .GroupBy(x => new { x.kullanici.ID, x.kullanici.KullaniciAdi, x.kullanici.Departman })
                .Select(g => new KullaniciAktiviteRaporu
                {
                    KullaniciAdi = g.Key.KullaniciAdi,
                    Departman = g.Key.Departman,
                    ToplamRandevuSayisi = g.Count(x => x.randevu != null),
                    OnaylananRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Onaylandı"),
                    BekleyenRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Bekliyor"),
                    ReddedilenRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Reddedildi"),
                    ToplamMesafe = g.Where(x => x.randevu != null).Sum(x => x.randevu.GidilecekMesafe),
                    SonRandevuTarihi = g.Where(x => x.randevu != null).Max(x => (DateTime?)x.randevu.BaslangicTarihi)
                })
                .OrderByDescending(x => x.ToplamRandevuSayisi)
                .ToList();

            // Verileri Excel'e yaz
            int row = 4;
            foreach (var item in rapor)
            {
                worksheet.Cell(row, 1).Value = item.KullaniciAdi;
                worksheet.Cell(row, 2).Value = item.Departman;
                worksheet.Cell(row, 3).Value = item.ToplamRandevuSayisi;
                worksheet.Cell(row, 4).Value = item.OnaylananRandevuSayisi;
                worksheet.Cell(row, 5).Value = item.BekleyenRandevuSayisi;
                worksheet.Cell(row, 6).Value = item.ReddedilenRandevuSayisi;
                worksheet.Cell(row, 7).Value = item.ToplamMesafe;
                worksheet.Cell(row, 8).Value = item.SonRandevuTarihi?.ToString("dd/MM/yyyy") ?? "-";
                row++;
            }

            // Sütun genişliklerini ayarla
            worksheet.Columns().AdjustToContents();
        }

        private void ExcelDepartmanRaporu(IXLWorksheet worksheet, RaporFiltre? filtre)
        {
            worksheet.Cell(1, 1).Value = "Departman Raporu";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 8).Merge();

            // Başlıklar
            worksheet.Cell(3, 1).Value = "Departman";
            worksheet.Cell(3, 2).Value = "Kullanıcı Sayısı";
            worksheet.Cell(3, 3).Value = "Toplam Randevu";
            worksheet.Cell(3, 4).Value = "Onaylandı";
            worksheet.Cell(3, 5).Value = "Bekliyor";
            worksheet.Cell(3, 6).Value = "Reddedildi";
            worksheet.Cell(3, 7).Value = "Toplam Mesafe (km)";
            worksheet.Cell(3, 8).Value = "Toplam Kişi";

            // Başlık satırını kalın yap
            worksheet.Row(3).Style.Font.Bold = true;
            worksheet.Row(3).Style.Fill.BackgroundColor = XLColor.LightGray;

            // Verileri al
            var query = _context.Kullanicilar
                .GroupJoin(_context.Randevu, k => k.ID, r => r.KullaniciID, (kullanici, randevular) => new { kullanici, randevular })
                .SelectMany(x => x.randevular.DefaultIfEmpty(), (x, randevu) => new { x.kullanici, randevu });

            if (filtre != null)
            {
                if (filtre.BaslangicTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi >= filtre.BaslangicTarihi);
                if (filtre.BitisTarihi.HasValue)
                    query = query.Where(x => x.randevu == null || x.randevu.BaslangicTarihi <= filtre.BitisTarihi);
                if (!string.IsNullOrEmpty(filtre.OnayDurumu))
                    query = query.Where(x => x.randevu == null || x.randevu.OnayDurumu == filtre.OnayDurumu);
            }

            var rapor = query
                .GroupBy(x => x.kullanici.Departman)
                .Select(g => new DepartmanRaporu
                {
                    Departman = g.Key,
                    KullaniciSayisi = g.Select(x => x.kullanici.ID).Distinct().Count(),
                    ToplamRandevuSayisi = g.Count(x => x.randevu != null),
                    OnaylananRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Onaylandı"),
                    BekleyenRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Bekliyor"),
                    ReddedilenRandevuSayisi = g.Count(x => x.randevu != null && x.randevu.OnayDurumu == "Reddedildi"),
                    ToplamMesafe = g.Where(x => x.randevu != null).Sum(x => x.randevu.GidilecekMesafe),
                    ToplamKisiSayisi = g.Where(x => x.randevu != null).Sum(x => x.randevu.KisiSayisi)
                })
                .OrderByDescending(x => x.ToplamRandevuSayisi)
                .ToList();

            // Verileri Excel'e yaz
            int row = 4;
            foreach (var item in rapor)
            {
                worksheet.Cell(row, 1).Value = item.Departman;
                worksheet.Cell(row, 2).Value = item.KullaniciSayisi;
                worksheet.Cell(row, 3).Value = item.ToplamRandevuSayisi;
                worksheet.Cell(row, 4).Value = item.OnaylananRandevuSayisi;
                worksheet.Cell(row, 5).Value = item.BekleyenRandevuSayisi;
                worksheet.Cell(row, 6).Value = item.ReddedilenRandevuSayisi;
                worksheet.Cell(row, 7).Value = item.ToplamMesafe;
                worksheet.Cell(row, 8).Value = item.ToplamKisiSayisi;
                row++;
            }

            // Sütun genişliklerini ayarla
            worksheet.Columns().AdjustToContents();
        }

        private void ExcelZamanBazliRapor(IXLWorksheet worksheet, RaporFiltre? filtre)
        {
            worksheet.Cell(1, 1).Value = "Zaman Bazlı Rapor";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 8).Merge();

            // Başlıklar
            worksheet.Cell(3, 1).Value = "Tarih";
            worksheet.Cell(3, 2).Value = "Randevu Sayısı";
            worksheet.Cell(3, 3).Value = "Onaylandı";
            worksheet.Cell(3, 4).Value = "Bekliyor";
            worksheet.Cell(3, 5).Value = "Reddedildi";
            worksheet.Cell(3, 6).Value = "Toplam Mesafe (km)";
            worksheet.Cell(3, 7).Value = "Toplam Kişi";

            // Başlık satırını kalın yap
            worksheet.Row(3).Style.Font.Bold = true;
            worksheet.Row(3).Style.Fill.BackgroundColor = XLColor.LightGray;

            // Verileri al
            var baslangic = filtre?.BaslangicTarihi ?? DateTime.Now.AddMonths(-1);
            var bitis = filtre?.BitisTarihi ?? DateTime.Now;

            var query = _context.Randevu.Where(r => r.BaslangicTarihi >= baslangic && r.BaslangicTarihi <= bitis);

            if (filtre != null)
            {
                if (!string.IsNullOrEmpty(filtre.OnayDurumu))
                    query = query.Where(r => r.OnayDurumu == filtre.OnayDurumu);
                if (!string.IsNullOrEmpty(filtre.Departman))
                    query = query.Include(r => r.Kullanici).Where(r => r.Kullanici.Departman == filtre.Departman);
            }

            var rapor = query
                .GroupBy(r => r.BaslangicTarihi.Date)
                .Select(g => new ZamanBazliRaporu
                {
                    Tarih = g.Key,
                    RandevuSayisi = g.Count(),
                    OnaylananSayisi = g.Count(r => r.OnayDurumu == "Onaylandı"),
                    BekleyenSayisi = g.Count(r => r.OnayDurumu == "Bekliyor"),
                    ReddedilenSayisi = g.Count(r => r.OnayDurumu == "Reddedildi"),
                    ToplamMesafe = g.Sum(r => r.GidilecekMesafe),
                    ToplamKisiSayisi = g.Sum(r => r.KisiSayisi)
                })
                .OrderBy(x => x.Tarih)
                .ToList();

            // Verileri Excel'e yaz
            int row = 4;
            foreach (var item in rapor)
            {
                worksheet.Cell(row, 1).Value = item.Tarih.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 2).Value = item.RandevuSayisi;
                worksheet.Cell(row, 3).Value = item.OnaylananSayisi;
                worksheet.Cell(row, 4).Value = item.BekleyenSayisi;
                worksheet.Cell(row, 5).Value = item.ReddedilenSayisi;
                worksheet.Cell(row, 6).Value = item.ToplamMesafe;
                worksheet.Cell(row, 7).Value = item.ToplamKisiSayisi;
                row++;
            }

            // Sütun genişliklerini ayarla
            worksheet.Columns().AdjustToContents();
        }
    }
} 
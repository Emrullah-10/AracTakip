using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AracTakipSistemi.Models;
using System.Text;

namespace AracTakipSistemi.Controllers
{
    public class IstatistikController : Controller
    {
        private readonly VeriTabani _context;

        public IstatistikController(VeriTabani context)
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
            if (!AdminKontrol())
            {
                return RedirectToAction("Login", "Kullanici");
            }

            // Araç İstatistikleri
            ViewBag.ToplamArac = _context.Arac.Count();
            ViewBag.AktifArac = _context.Arac.Count(a => a.Durumu == "Müsait");
            ViewBag.KullanimdaArac = _context.Arac.Count(a => a.Durumu == "Kullanımda");
            ViewBag.BakimdaArac = _context.Arac.Count(a => a.Durumu == "Bakımda");

            // Kullanıcı İstatistikleri
            ViewBag.ToplamKullanici = _context.Kullanicilar.Count();
            ViewBag.AktifKullanici = _context.Kullanicilar.Count(k => k.Durumu == "Aktif");
            ViewBag.PasifKullanici = _context.Kullanicilar.Count(k => k.Durumu == "Pasif");

            // Randevu İstatistikleri
            ViewBag.ToplamRandevu = _context.Randevu.Count();
            ViewBag.BekleyenRandevu = _context.Randevu.Count(r => r.OnayDurumu == "Bekliyor");
            ViewBag.OnaylananRandevu = _context.Randevu.Count(r => r.OnayDurumu == "Onaylandı");
            ViewBag.ReddedilenRandevu = _context.Randevu.Count(r => r.OnayDurumu == "Reddedildi");

            // Departman bazlı istatistikler
            var departmanIstatistikleri = _context.Randevu
                .Include(r => r.Kullanici)
                .GroupBy(r => r.Kullanici.Departman)
                .Select(g => new
                {
                    Departman = g.Key,
                    RandevuSayisi = g.Count(),
                    OnaylananSayisi = g.Count(r => r.OnayDurumu == "Onaylandı"),
                    BekleyenSayisi = g.Count(r => r.OnayDurumu == "Bekliyor")
                })
                .OrderByDescending(x => x.RandevuSayisi)
                .Take(5)
                .ToList();

            ViewBag.DepartmanIstatistikleri = departmanIstatistikleri;

            // En çok kullanılan araçlar - basitleştirilmiş sorgu
            var enCokKullanilanAraclar = _context.Randevu
                .Include(r => r.Arac)
                .GroupBy(r => r.AracID)
                .Select(g => new
                {
                    AracID = g.Key,
                    KullanimSayisi = g.Count()
                })
                .OrderByDescending(x => x.KullanimSayisi)
                .Take(5)
                .ToList();

            // Araç bilgilerini ayrıca çek
            var aracBilgileri = _context.Arac.ToDictionary(a => a.ID, a => a);
            
            var enCokKullanilanAraclarDetayli = enCokKullanilanAraclar.Select(x => new
            {
                Arac = aracBilgileri.ContainsKey(x.AracID ?? 0) ? aracBilgileri[x.AracID ?? 0] : null,
                KullanimSayisi = x.KullanimSayisi,
                ToplamSure = 0.0 // Basitlik için 0
            }).Where(x => x.Arac != null).ToList();

            ViewBag.EnCokKullanilanAraclar = enCokKullanilanAraclarDetayli;

            return View();
        }

        [HttpPost]
        public IActionResult ExcelIndir()
        {
            if (!AdminKontrol())
            {
                return Json(new { success = false, message = "Yetki hatası!" });
            }

            try
            {
                // Excel verilerini hazırla
                var excelData = new StringBuilder();
                
                // Başlık
                excelData.AppendLine("ARAÇ TAKİP SİSTEMİ İSTATİSTİKLERİ");
                excelData.AppendLine("Tarih: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                excelData.AppendLine();

                // Araç İstatistikleri
                excelData.AppendLine("ARAÇ İSTATİSTİKLERİ");
                excelData.AppendLine("Toplam Araç Sayısı," + _context.Arac.Count());
                excelData.AppendLine("Aktif Araç Sayısı," + _context.Arac.Count(a => a.Durumu == "Müsait"));
                excelData.AppendLine("Kullanımda Araç Sayısı," + _context.Arac.Count(a => a.Durumu == "Kullanımda"));
                excelData.AppendLine("Bakımda Araç Sayısı," + _context.Arac.Count(a => a.Durumu == "Bakımda"));
                excelData.AppendLine();

                // Kullanıcı İstatistikleri
                excelData.AppendLine("KULLANICI İSTATİSTİKLERİ");
                excelData.AppendLine("Toplam Kullanıcı Sayısı," + _context.Kullanicilar.Count());
                excelData.AppendLine("Aktif Kullanıcı Sayısı," + _context.Kullanicilar.Count(k => k.Durumu == "Aktif"));
                excelData.AppendLine("Pasif Kullanıcı Sayısı," + _context.Kullanicilar.Count(k => k.Durumu == "Pasif"));
                excelData.AppendLine();

                // Randevu İstatistikleri
                excelData.AppendLine("RANDEVU İSTATİSTİKLERİ");
                excelData.AppendLine("Toplam Randevu Sayısı," + _context.Randevu.Count());
                excelData.AppendLine("Bekleyen Randevu Sayısı," + _context.Randevu.Count(r => r.OnayDurumu == "Bekliyor"));
                excelData.AppendLine("Onaylanan Randevu Sayısı," + _context.Randevu.Count(r => r.OnayDurumu == "Onaylandı"));
                excelData.AppendLine("Reddedilen Randevu Sayısı," + _context.Randevu.Count(r => r.OnayDurumu == "Reddedildi"));
                excelData.AppendLine();

                // Departman İstatistikleri
                excelData.AppendLine("DEPARTMAN İSTATİSTİKLERİ");
                excelData.AppendLine("Departman,Randevu Sayısı,Onaylanan Sayısı,Bekleyen Sayısı");
                
                var departmanIstatistikleri = _context.Randevu
                    .Include(r => r.Kullanici)
                    .GroupBy(r => r.Kullanici.Departman)
                    .Select(g => new
                    {
                        Departman = g.Key,
                        RandevuSayisi = g.Count(),
                        OnaylananSayisi = g.Count(r => r.OnayDurumu == "Onaylandı"),
                        BekleyenSayisi = g.Count(r => r.OnayDurumu == "Bekliyor")
                    })
                    .OrderByDescending(x => x.RandevuSayisi)
                    .ToList();

                foreach (var dept in departmanIstatistikleri)
                {
                    excelData.AppendLine($"{dept.Departman},{dept.RandevuSayisi},{dept.OnaylananSayisi},{dept.BekleyenSayisi}");
                }
                excelData.AppendLine();

                // En Çok Kullanılan Araçlar
                excelData.AppendLine("EN ÇOK KULLANILAN ARAÇLAR");
                excelData.AppendLine("Araç Plakası,Marka Model,Kullanım Sayısı");
                
                var enCokKullanilanAraclarExcel = _context.Randevu
                    .GroupBy(r => r.AracID)
                    .Select(g => new
                    {
                        AracID = g.Key,
                        KullanimSayisi = g.Count()
                    })
                    .OrderByDescending(x => x.KullanimSayisi)
                    .Take(10)
                    .ToList();

                var aracBilgileriExcel = _context.Arac.ToDictionary(a => a.ID, a => a);
                
                foreach (var arac in enCokKullanilanAraclarExcel)
                {
                    if (arac.AracID.HasValue && aracBilgileriExcel.ContainsKey(arac.AracID.Value))
                    {
                        var aracDetay = aracBilgileriExcel[arac.AracID.Value];
                        excelData.AppendLine($"{aracDetay.Plaka},{aracDetay.Marka} {aracDetay.Model},{arac.KullanimSayisi}");
                    }
                }

                // Excel dosyasını döndür
                var fileName = $"Istatistikler_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var bytes = Encoding.UTF8.GetBytes(excelData.ToString());
                
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Excel oluşturulurken hata: " + ex.Message });
            }
        }
    }
} 
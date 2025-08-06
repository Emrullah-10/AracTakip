using Microsoft.AspNetCore.Mvc;
using AracTakipSistemi.Models;
using Microsoft.EntityFrameworkCore;

namespace AracTakipSistemi.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly VeriTabani _context;

        public KullaniciController(VeriTabani context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string kullaniciAdi, string sifre)
        {
            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            {
                ViewBag.Hata = "Kullanıcı adı ve şifre alanları boş olamaz.";
                return View();
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi && k.Sifre == sifre);

            if (kullanici != null)
            {
                HttpContext.Session.SetInt32("KullaniciID", kullanici.ID);
                HttpContext.Session.SetString("KullaniciAd", kullanici.KullaniciAdi);
                HttpContext.Session.SetString("KullaniciRol", kullanici.Rol ? "Admin" : "Kullanici");
                HttpContext.Session.SetString("KullaniciDepartman", kullanici.Departman);
                
                if (kullanici.Rol) // Admin ise
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Randevu");
                }
            }
            else
            {
                ViewBag.Hata = "Kullanıcı adı veya şifre hatalı.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Anasayfa");
        }

        public IActionResult Cikis()
        {
            HttpContext.Session.Clear();
            TempData["Basari"] = "Başarıyla çıkış yapıldı.";
            return RedirectToAction("Index", "Anasayfa");
        }

        public IActionResult Profil()
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciID == null)
            {
                return RedirectToAction("Login");
            }

            var kullanici = _context.Kullanicilar.Find(kullaniciID);
            return View(kullanici);
        }

        [HttpGet]
        public IActionResult SifreDegistir()
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciID == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SifreDegistir(string mevcutSifre, string yeniSifre, string yeniSifreTekrar)
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciID == null)
            {
                return RedirectToAction("Login");
            }

            var kullanici = await _context.Kullanicilar.FindAsync(kullaniciID);
            if (kullanici == null)
            {
                TempData["Hata"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Login");
            }

            if (kullanici.Sifre != mevcutSifre)
            {
                ViewBag.Hata = "Mevcut şifre hatalı.";
                return View();
            }

            if (yeniSifre != yeniSifreTekrar)
            {
                ViewBag.Hata = "Yeni şifreler eşleşmiyor.";
                return View();
            }

            if (string.IsNullOrEmpty(yeniSifre) || yeniSifre.Length < 6)
            {
                ViewBag.Hata = "Yeni şifre en az 6 karakter olmalıdır.";
                return View();
            }

            kullanici.Sifre = yeniSifre;
            await _context.SaveChangesAsync();

            TempData["Basari"] = "Şifreniz başarıyla değiştirildi.";
            return RedirectToAction("Profil");
        }
    }
}

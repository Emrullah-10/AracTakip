using AracTakipSistemi.Models;
using Microsoft.EntityFrameworkCore;

namespace AracTakipSistemi.Services
{
    public class BildirimService
    {
        private readonly VeriTabani _context;

        public BildirimService(VeriTabani context)
        {
            _context = context;
        }

        // Bildirim oluştur
        public async Task<Bildirim> BildirimOlustur(int kullaniciId, string baslik, string mesaj, string tip = "info")
        {
            var bildirim = new Bildirim
            {
                KullaniciID = kullaniciId,
                Baslik = baslik,
                Mesaj = mesaj,
                Tip = tip,
                OlusturmaTarihi = DateTime.Now
            };

            _context.Bildirimler.Add(bildirim);
            await _context.SaveChangesAsync();
            return bildirim;
        }

        // Kullanıcının okunmamış bildirimlerini getir
        public async Task<List<Bildirim>> GetOkunmamisBildirimler(int kullaniciId)
        {
            return await _context.Bildirimler
                .Where(b => b.KullaniciID == kullaniciId && !b.Okundu)
                .OrderByDescending(b => b.OlusturmaTarihi)
                .ToListAsync();
        }

        // Kullanıcının tüm bildirimlerini getir
        public async Task<List<Bildirim>> GetKullaniciBildirimleri(int kullaniciId, int sayfa = 1, int sayfaBoyutu = 10)
        {
            return await _context.Bildirimler
                .Where(b => b.KullaniciID == kullaniciId)
                .OrderByDescending(b => b.OlusturmaTarihi)
                .Skip((sayfa - 1) * sayfaBoyutu)
                .Take(sayfaBoyutu)
                .ToListAsync();
        }

        // Bildirimi okundu olarak işaretle
        public async Task BildirimOkundu(int bildirimId)
        {
            var bildirim = await _context.Bildirimler.FindAsync(bildirimId);
            if (bildirim != null)
            {
                bildirim.Okundu = true;
                bildirim.OkunmaTarihi = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // Tüm bildirimleri okundu olarak işaretle
        public async Task TumBildirimleriOkundu(int kullaniciId)
        {
            var bildirimler = await _context.Bildirimler
                .Where(b => b.KullaniciID == kullaniciId && !b.Okundu)
                .ToListAsync();

            foreach (var bildirim in bildirimler)
            {
                bildirim.Okundu = true;
                bildirim.OkunmaTarihi = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        // Okunmamış bildirim sayısını getir
        public async Task<int> GetOkunmamisBildirimSayisi(int kullaniciId)
        {
            return await _context.Bildirimler
                .CountAsync(b => b.KullaniciID == kullaniciId && !b.Okundu);
        }

        // Randevu onaylandığında bildirim oluştur
        public async Task RandevuOnaylandiBildirim(int randevuId, int kullaniciId)
        {
            var randevu = await _context.Randevu
                .Include(r => r.Arac)
                .FirstOrDefaultAsync(r => r.ID == randevuId);

            if (randevu != null)
            {
                var baslik = "Randevu Onaylandı";
                var mesaj = $"Araç talebiniz onaylandı. Araç: {randevu.Arac?.Marka} {randevu.Arac?.Model} ({randevu.Arac?.Plaka})";
                
                await BildirimOlustur(kullaniciId, baslik, mesaj, "success");
            }
        }

        // Randevu reddedildiğinde bildirim oluştur
        public async Task RandevuReddedildiBildirim(int randevuId, int kullaniciId, string redSebebi)
        {
            var baslik = "Randevu Reddedildi";
            var mesaj = $"Araç talebiniz reddedildi. Sebep: {redSebebi}";
            
            await BildirimOlustur(kullaniciId, baslik, mesaj, "danger");
        }

        // Yeni randevu talebi geldiğinde admin'e bildirim
        public async Task YeniRandevuTalebiBildirim(int randevuId)
        {
            var randevu = await _context.Randevu
                .Include(r => r.Kullanici)
                .FirstOrDefaultAsync(r => r.ID == randevuId);

            if (randevu != null)
            {
                // Admin kullanıcılarını bul
                var adminKullanicilar = await _context.Kullanicilar
                    .Where(k => k.Rol == true)
                    .ToListAsync();

                foreach (var admin in adminKullanicilar)
                {
                    var baslik = "Yeni Araç Talebi";
                    var mesaj = $"{randevu.Kullanici.KullaniciAdi} adlı kullanıcıdan yeni araç talebi geldi.";
                    
                    await BildirimOlustur(admin.ID, baslik, mesaj, "info");
                }
            }
        }

        // Bildirimi sil
        public async Task BildirimSil(int bildirimId)
        {
            var bildirim = await _context.Bildirimler.FindAsync(bildirimId);
            if (bildirim != null)
            {
                _context.Bildirimler.Remove(bildirim);
                await _context.SaveChangesAsync();
            }
        }
    }
} 
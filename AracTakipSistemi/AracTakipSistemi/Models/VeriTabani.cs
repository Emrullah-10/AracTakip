using Microsoft.EntityFrameworkCore;

namespace AracTakipSistemi.Models
{
    public class VeriTabani:DbContext
    {
        public VeriTabani(DbContextOptions<VeriTabani> options) : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Arac> Arac { get; set; }
        public DbSet<Randevu> Randevu { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }
    }
}

namespace AracTakipSistemi.Models
{
    public class Kullanici
    {
        public int ID { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string Departman { get; set; } = string.Empty;
        public string Durumu { get; set; } = "Aktif"; // Aktif, Pasif
        public bool Rol { get; set; } // false = User, true = Admin
        
        // Navigation property - Randevu tablosu kaldırıldığı için nullable
        public virtual ICollection<Randevu>? Randevular { get; set; }
    }
}

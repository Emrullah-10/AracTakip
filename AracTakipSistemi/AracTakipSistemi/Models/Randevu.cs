using System.ComponentModel.DataAnnotations;

namespace AracTakipSistemi.Models
{
    public class Randevu
    {
        public int ID { get; set; }
        public int KullaniciID { get; set; }
        public int? AracID { get; set; } // Nullable yapıldı, admin atasın diye
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public int GidilecekMesafe { get; set; }
        
        [Required(ErrorMessage = "Talep nedeni gereklidir.")]
        public string TalepNedeni { get; set; } = string.Empty; // Kullanıcının belirttiği talep nedeni
        
        [Required(ErrorMessage = "Kişi sayısı gereklidir.")]
        public int KisiSayisi { get; set; } = 1; // Randevudaki kişi sayısı
        
        // Randevu onay durumu
        public string OnayDurumu { get; set; } = "Bekliyor"; // Bekliyor, Onaylandı, Reddedildi
        public string? RedSebebi { get; set; } // Red sebebi (null olabilir)
        
        // Navigation properties
        public Kullanici? Kullanici { get; set; }
        public Arac? Arac { get; set; }
    }
}

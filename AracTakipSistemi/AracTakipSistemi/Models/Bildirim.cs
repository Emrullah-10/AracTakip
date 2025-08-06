using System.ComponentModel.DataAnnotations;

namespace AracTakipSistemi.Models
{
    public class Bildirim
    {
        public int ID { get; set; }
        
        [Required]
        public int KullaniciID { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Baslik { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Mesaj { get; set; }
        
        [Required]
        public string Tip { get; set; } // "success", "warning", "info", "danger"
        
        public bool Okundu { get; set; } = false;
        
        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        
        public DateTime? OkunmaTarihi { get; set; }
        
        // Navigation Property
        public virtual Kullanici Kullanici { get; set; }
    }
} 
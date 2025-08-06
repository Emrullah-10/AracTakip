using System.ComponentModel.DataAnnotations;

namespace AracTakipSistemi.Models
{
    public class Arac
    {
        public int ID { get; set; }
        
        [Required(ErrorMessage = "Marka gereklidir.")]
        public string Marka { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Model gereklidir.")]
        public string Model { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Plaka gereklidir.")]
        public string Plaka { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Araç tipi gereklidir.")]
        public string AracTipi { get; set; } = string.Empty;
        
        public string Durumu { get; set; } = "Müsait";
        
        [Required(ErrorMessage = "Vites tipi gereklidir.")]
        public string VitesTipi { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Yakıt türü gereklidir.")]
        public string YakitTuru { get; set; } = string.Empty;
        
        // Navigation property
        public virtual ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
    }
}

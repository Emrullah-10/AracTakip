namespace AracTakipSistemi.Models
{
    // Ana rapor sayfası için
    public class RaporIndexViewModel
    {
        public int ToplamAracSayisi { get; set; }
        public int MusaitAracSayisi { get; set; }
        public int KullanimdaAracSayisi { get; set; }
        public int ToplamKullaniciSayisi { get; set; }
        public int ToplamRandevuSayisi { get; set; }
        public int BekleyenRandevuSayisi { get; set; }
        public int OnaylananRandevuSayisi { get; set; }
        public int ReddedilenRandevuSayisi { get; set; }
    }

    // Araç kullanım raporu için
    public class AracKullanimRaporu
    {
        public int AracID { get; set; }
        public string AracBilgisi { get; set; } = string.Empty;
        public string Marka { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Plaka { get; set; } = string.Empty;
        public string Durumu { get; set; } = string.Empty;
        public int ToplamRandevuSayisi { get; set; }
        public int ToplamKullaniciSayisi { get; set; }
        public int ToplamMesafe { get; set; }
        public int ToplamKisiSayisi { get; set; }
        public DateTime? SonKullanimTarihi { get; set; }
    }

    // Kullanıcı aktivite raporu için
    public class KullaniciAktiviteRaporu
    {
        public int KullaniciID { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Departman { get; set; } = string.Empty;
        public int ToplamRandevuSayisi { get; set; }
        public int OnaylananRandevuSayisi { get; set; }
        public int BekleyenRandevuSayisi { get; set; }
        public int ReddedilenRandevuSayisi { get; set; }
        public int ToplamMesafe { get; set; }
        public DateTime? SonRandevuTarihi { get; set; }
    }

    // Departman bazlı rapor için
    public class DepartmanRaporu
    {
        public string Departman { get; set; } = string.Empty;
        public int KullaniciSayisi { get; set; }
        public int ToplamRandevuSayisi { get; set; }
        public int OnaylananRandevuSayisi { get; set; }
        public int BekleyenRandevuSayisi { get; set; }
        public int ReddedilenRandevuSayisi { get; set; }
        public int ToplamMesafe { get; set; }
        public int ToplamKisiSayisi { get; set; }
    }

    // Zaman bazlı rapor için
    public class ZamanBazliRaporu
    {
        public DateTime Tarih { get; set; }
        public string TarihString { get; set; } = string.Empty;
        public int RandevuSayisi { get; set; }
        public int OnaylananSayisi { get; set; }
        public int BekleyenSayisi { get; set; }
        public int ReddedilenSayisi { get; set; }
        public int ToplamMesafe { get; set; }
        public int ToplamKisiSayisi { get; set; }
    }

    // Rapor filtreleme için
    public class RaporFiltre
    {
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string? Departman { get; set; }
        public string? OnayDurumu { get; set; }
        public int? AracID { get; set; }
        public int? KullaniciID { get; set; }
    }
} 
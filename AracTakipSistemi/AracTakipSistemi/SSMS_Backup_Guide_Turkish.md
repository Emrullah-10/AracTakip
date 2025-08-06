# 📦 SQL Server Management Studio ile Backup Alma Rehberi

## 🎯 Adım Adım Backup Alma

### **1. SQL Server Management Studio'yu Açın**

### **2. Veritabanına Bağlanın**
- **Server name**: `.` (local) veya sunucu adınız
- **Authentication**: Windows Authentication veya SQL Server Authentication

### **3. AracTakip Veritabanını Seçin**
- Object Explorer'da **Databases** klasörünü genişletin
- **AracTakip** veritabanını bulun

### **4. Backup Oluşturma**

#### **Yöntem A: GUI ile Backup**
1. **AracTakip** veritabanına sağ tıklayın
2. **Tasks** → **Back Up** seçin
3. **Backup type**: Full
4. **Destination**: Disk
5. **Add** butonuna tıklayın
6. Backup dosyası adı: `AracTakip_Clean.bak`
7. **Options** sekmesine geçin:
   - ✅ **Compression** işaretleyin
   - ✅ **Verify backup when finished** işaretleyin
   - ✅ **Perform checksum before writing to media** işaretleyin
8. **OK** ile backup oluşturun

#### **Yöntem B: SQL Script ile Backup**
```sql
-- Küçük boyutlu backup oluştur
BACKUP DATABASE AracTakip TO DISK = 'C:\Backups\AracTakip_Clean.bak'
WITH 
    COMPRESSION,
    INIT,
    FORMAT,
    MEDIANAME = 'AracTakip_Clean_Backup',
    NAME = 'AracTakip_Clean_Backup',
    SKIP,
    STATS = 10;
```

### **5. Backup Boyutunu Kontrol Edin**

```sql
-- Backup dosyası boyutunu kontrol et
SELECT 
    backup_set_id,
    backup_size / 1024 / 1024 as 'Size (MB)',
    compressed_backup_size / 1024 / 1024 as 'Compressed Size (MB)',
    backup_start_date,
    backup_finish_date
FROM msdb.dbo.backupset 
WHERE database_name = 'AracTakip'
ORDER BY backup_start_date DESC;
```

## 🔧 **Backup Optimizasyonu**

### **1. Sıkıştırma Etkinleştirme**
```sql
-- Backup sıkıştırmasını etkinleştir
EXEC sp_configure 'backup compression default', 1;
RECONFIGURE;
```

### **2. Backup Dosyası Konumu**
- **Önerilen**: `C:\Backups\` klasörü
- **Alternatif**: Proje klasörü içinde

### **3. Backup Adlandırma**
- **Format**: `AracTakip_YYYYMMDD_HHMMSS.bak`
- **Örnek**: `AracTakip_20250107_143022.bak`

## 📊 **Beklenen Boyutlar**

| Durum | Tahmini Boyut |
|-------|---------------|
| Temizlenmiş Veritabanı | 10-25 MB |
| Sıkıştırılmış Backup | 5-15 MB |
| Hosting'e Uygun | ✅ |

## ✅ **Kontrol Listesi**

- [ ] Veritabanı temizlendi
- [ ] Log dosyası küçültüldü
- [ ] Sıkıştırma etkinleştirildi
- [ ] Backup dosyası oluşturuldu
- [ ] Backup boyutu kontrol edildi
- [ ] Backup dosyası test edildi

## 🚀 **Hosting'e Yükleme**

1. **Backup dosyasını** hosting'e yükleyin
2. **Hosting kontrol panelinde** restore edin
3. **Connection string'i** güncelleyin
4. **Test edin**

## 💡 **İpuçları**

- **Compression** her zaman etkinleştirin
- **Backup dosyasını** yüklemeden önce test edin
- **Boyutu** hosting sınırlarına uygun olmalı
- **Düzenli backup** alın

## 🔍 **Sorun Giderme**

### ❌ "Log file size" Hatası
**Çözüm**: Veritabanı zaten temizlendi, backup boyutu küçük olmalı

### ❌ "Access denied" Hatası
**Çözüm**: Backup klasörü izinlerini kontrol edin

### ❌ "Backup failed" Hatası
**Çözüm**: Disk alanını kontrol edin

## 📞 **Destek**

Sorun yaşarsanız:
1. Backup dosyası boyutunu kontrol edin
2. Disk alanını kontrol edin
3. SQL Server servisini yeniden başlatın 
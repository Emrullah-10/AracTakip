# 📦 SSMS ile Küçük Backup Oluşturma Rehberi

## 🎯 Adım Adım Küçük Backup

### **1. SQL Server Management Studio'yu Açın**

### **2. Veritabanına Bağlanın**
- Server name: `.` (local) veya sunucu adınız
- Authentication: Windows Authentication veya SQL Server Authentication

### **3. Backup Oluşturma**

#### **Adım 1: Veritabanını Seçin**
```sql
USE AracTakip;
```

#### **Adım 2: Gereksiz Verileri Temizleyin**
```sql
-- Eski log kayıtlarını temizle
DELETE FROM Bildirimler WHERE OlusturmaTarihi < DATEADD(day, -30, GETDATE());

-- Eski randevuları temizle (1 yıldan eski)
DELETE FROM Randevu WHERE OlusturmaTarihi < DATEADD(year, -1, GETDATE());

-- Log dosyasını küçült
DBCC SHRINKFILE (AracTakip_Log, 1);
```

#### **Adım 3: Küçük Backup Oluşturun**

**SSMS GUI ile:**
1. Veritabanına sağ tıklayın
2. **Tasks** → **Back Up**
3. **Backup type**: Full
4. **Destination**: Disk
5. **Add** → Backup dosyası adı: `AracTakip_Small.bak`
6. **Options** sekmesinde:
   - ✅ **Compression** işaretleyin
   - ✅ **Verify backup when finished** işaretleyin
7. **OK** ile backup oluşturun

**SQL Script ile:**
```sql
BACKUP DATABASE AracTakip TO DISK = 'C:\Backups\AracTakip_Small.bak'
WITH 
    COMPRESSION,
    INIT,
    FORMAT,
    MEDIANAME = 'AracTakip_Small_Backup',
    NAME = 'AracTakip_Small_Backup',
    SKIP,
    STATS = 10
```

### **4. Backup Boyutunu Kontrol Edin**

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

## 🔧 **Backup Boyutunu Daha Da Küçültme**

### **1. Sadece Schema Backup**
```sql
-- Sadece tablo yapılarını export et
USE AracTakip;
GO

-- Script oluştur
EXEC sp_help_revlogin;
GO

-- Tabloları script olarak export et
EXEC sp_help_revlogin;
GO
```

### **2. Minimal Veri ile Backup**
```sql
-- Sadece gerekli verileri bırak
DELETE FROM Bildirimler WHERE Okundu = 1;
DELETE FROM Randevu WHERE Durumu = 'Tamamlandı' AND OlusturmaTarihi < DATEADD(month, -6, GETDATE());

-- Log dosyasını küçült
DBCC SHRINKFILE (AracTakip_Log, 1);
```

### **3. Sıkıştırılmış Backup**
```sql
-- Maksimum sıkıştırma ile backup
BACKUP DATABASE AracTakip TO DISK = 'C:\Backups\AracTakip_Minimal.bak'
WITH 
    COMPRESSION,
    INIT,
    FORMAT,
    MEDIANAME = 'AracTakip_Minimal',
    NAME = 'AracTakip_Minimal_Backup',
    SKIP,
    STATS = 10
```

## 📊 **Beklenen Boyutlar**

| Backup Türü | Tahmini Boyut |
|-------------|---------------|
| Tam Backup | 50-100 MB |
| Küçük Backup | 10-25 MB |
| Minimal Backup | 5-15 MB |
| Schema Only | 1-5 MB |

## ✅ **Kontrol Listesi**

- [ ] Gereksiz veriler temizlendi
- [ ] Log dosyası küçültüldü
- [ ] Sıkıştırma etkinleştirildi
- [ ] Backup boyutu kontrol edildi
- [ ] Backup dosyası test edildi

## 🚀 **Hosting'e Yükleme**

1. Küçük backup dosyasını hosting'e yükleyin
2. Hosting kontrol panelinde restore edin
3. Connection string'i güncelleyin
4. Test edin

## 💡 **İpuçları**

- **Compression** her zaman etkinleştirin
- **Log dosyasını** düzenli olarak küçültün
- **Eski verileri** periyodik olarak temizleyin
- **Backup boyutunu** yüklemeden önce kontrol edin 
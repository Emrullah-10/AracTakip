-- AracTakip Veritabanı Temizleme ve Güncelleme Scripti
USE AracTakip;
GO

-- Gereksiz verileri temizle
PRINT 'Gereksiz veriler temizleniyor...';

-- Eski bildirimleri temizle
DELETE FROM Bildirimler WHERE Okundu = 1;
DELETE FROM Bildirimler WHERE OlusturmaTarihi < DATEADD(day, -30, GETDATE());

-- Eski randevuları temizle
DELETE FROM Randevu WHERE Durumu = 'Tamamlandı' AND BaslangicTarihi < DATEADD(month, -6, GETDATE());
DELETE FROM Randevu WHERE BaslangicTarihi < DATEADD(year, -1, GETDATE());

-- Log dosyasını küçült
PRINT 'Log dosyası küçültülüyor...';
DBCC SHRINKFILE (AracTakip_Log, 1);
GO

-- Veritabanını optimize et
PRINT 'Veritabanı optimize ediliyor...';
DBCC SHRINKDATABASE (AracTakip, 10);
GO

-- İstatistikleri güncelle
PRINT 'İstatistikler güncelleniyor...';
UPDATE STATISTICS Kullanicilar;
UPDATE STATISTICS Araclar;
UPDATE STATISTICS Randevu;
UPDATE STATISTICS Bildirimler;
GO

-- Veritabanı boyutunu kontrol et
PRINT 'Veritabanı boyutu kontrol ediliyor...';
SELECT 
    SUM(size * 8.0 / 1024) as 'Database Size (MB)',
    SUM(CASE WHEN type = 0 THEN size * 8.0 / 1024 ELSE 0 END) as 'Data Size (MB)',
    SUM(CASE WHEN type = 1 THEN size * 8.0 / 1024 ELSE 0 END) as 'Log Size (MB)'
FROM sys.database_files;
GO

-- Tablo kayıt sayılarını göster
PRINT 'Tablo kayıt sayıları:';
SELECT 'Kullanicilar' as Tablo, COUNT(*) as KayitSayisi FROM Kullanicilar
UNION ALL
SELECT 'Araclar' as Tablo, COUNT(*) as KayitSayisi FROM Araclar
UNION ALL
SELECT 'Randevu' as Tablo, COUNT(*) as KayitSayisi FROM Randevu
UNION ALL
SELECT 'Bildirimler' as Tablo, COUNT(*) as KayitSayisi FROM Bildirimler;
GO

PRINT 'Veritabanı temizleme ve güncelleme tamamlandı!';
PRINT 'Artık SQL Server Management Studio ile backup alabilirsiniz.'; 
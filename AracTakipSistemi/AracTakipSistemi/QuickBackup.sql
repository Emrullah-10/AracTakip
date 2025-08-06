-- AracTakip Hızlı Backup Alma Scripti
-- Bu script temizlenmiş veritabanından küçük backup oluşturur

USE AracTakip;
GO

-- Backup sıkıştırmasını etkinleştir
EXEC sp_configure 'backup compression default', 1;
RECONFIGURE;
GO

-- Küçük boyutlu backup oluştur
BACKUP DATABASE AracTakip TO DISK = 'C:\Users\emrul\OneDrive\Masaüstü\PROJELERİM\AracTakipSistemi\AracTakip_Clean.bak'
WITH 
    COMPRESSION,
    INIT,
    FORMAT,
    MEDIANAME = 'AracTakip_Clean_Backup',
    NAME = 'AracTakip_Clean_Backup',
    SKIP,
    STATS = 10;
GO

-- Backup boyutunu kontrol et
SELECT 
    backup_set_id,
    backup_size / 1024 / 1024 as 'Size (MB)',
    compressed_backup_size / 1024 / 1024 as 'Compressed Size (MB)',
    backup_start_date,
    backup_finish_date
FROM msdb.dbo.backupset 
WHERE database_name = 'AracTakip'
ORDER BY backup_start_date DESC;
GO

PRINT 'Backup başarıyla oluşturuldu!';
PRINT 'Dosya konumu: C:\Users\emrul\OneDrive\Masaüstü\PROJELERİM\AracTakipSistemi\AracTakip_Clean.bak'; 
-- AracTakip Küçük Backup Oluşturma Scripti
USE AracTakip;
GO

-- Gereksiz verileri temizle
DELETE FROM Bildirimler WHERE Okundu = 1;
DELETE FROM Randevu WHERE Durumu = 'Tamamlandı' AND BaslangicTarihi < DATEADD(month, -6, GETDATE());

-- Log dosyasını küçült
DBCC SHRINKFILE (AracTakip_Log, 1);
GO

-- Küçük backup oluştur
BACKUP DATABASE AracTakip TO DISK = 'C:\Users\emrul\OneDrive\Masaüstü\PROJELERİM\AracTakipSistemi\AracTakip_Small.bak'
WITH 
    COMPRESSION,
    INIT,
    FORMAT,
    MEDIANAME = 'AracTakip_Small_Backup',
    NAME = 'AracTakip_Small_Backup',
    SKIP,
    STATS = 10;
GO

PRINT 'Küçük backup başarıyla oluşturuldu!'; 
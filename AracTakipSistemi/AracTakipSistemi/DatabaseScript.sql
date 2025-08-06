-- AracTakipSistemi Veritabanı Oluşturma Scripti
-- Bu script hosting sağlayıcınızda çalıştırılabilir

-- Veritabanını oluştur (eğer yoksa)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'AracTakip')
BEGIN
    CREATE DATABASE AracTakip;
END
GO

USE AracTakip;
GO

-- Kullanıcılar tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Kullanicilar')
BEGIN
    CREATE TABLE Kullanicilar (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        KullaniciAdi NVARCHAR(50) NOT NULL UNIQUE,
        Sifre NVARCHAR(100) NOT NULL,
        Ad NVARCHAR(50) NOT NULL,
        Soyad NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        Telefon NVARCHAR(20),
        Departman NVARCHAR(50) NOT NULL,
        Rol BIT NOT NULL DEFAULT 0, -- 0: Kullanıcı, 1: Admin
        Durumu NVARCHAR(20) DEFAULT 'Aktif',
        KayitTarihi DATETIME2 DEFAULT GETDATE()
    );
END
GO

-- Araçlar tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Araclar')
BEGIN
    CREATE TABLE Araclar (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        Marka NVARCHAR(50) NOT NULL,
        Model NVARCHAR(50) NOT NULL,
        Plaka NVARCHAR(20) NOT NULL UNIQUE,
        AracTipi NVARCHAR(30) NOT NULL,
        Durumu NVARCHAR(20) NOT NULL DEFAULT 'Müsait',
        VitesTipi NVARCHAR(20) NOT NULL,
        YakitTuru NVARCHAR(20) NOT NULL,
        KayitTarihi DATETIME2 DEFAULT GETDATE()
    );
END
GO

-- Randevular tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Randevu')
BEGIN
    CREATE TABLE Randevu (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        KullaniciID INT NOT NULL,
        AracID INT,
        BaslangicTarihi DATETIME2 NOT NULL,
        BitisTarihi DATETIME2 NOT NULL,
        Amac NVARCHAR(200) NOT NULL,
        Durumu NVARCHAR(20) DEFAULT 'Talep Edildi',
        OnayDurumu NVARCHAR(20) DEFAULT 'Beklemede',
        OlusturmaTarihi DATETIME2 DEFAULT GETDATE(),
        FOREIGN KEY (KullaniciID) REFERENCES Kullanicilar(ID),
        FOREIGN KEY (AracID) REFERENCES Araclar(ID)
    );
END
GO

-- Bildirimler tablosu
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Bildirimler')
BEGIN
    CREATE TABLE Bildirimler (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        KullaniciID INT NOT NULL,
        Baslik NVARCHAR(100) NOT NULL,
        Mesaj NVARCHAR(500) NOT NULL,
        Tip NVARCHAR(20) NOT NULL, -- 'Randevu', 'Sistem', 'Uyarı'
        Okundu BIT DEFAULT 0,
        OlusturmaTarihi DATETIME2 DEFAULT GETDATE(),
        FOREIGN KEY (KullaniciID) REFERENCES Kullanicilar(ID)
    );
END
GO

-- Örnek veriler (isteğe bağlı)
-- Admin kullanıcısı oluştur
IF NOT EXISTS (SELECT * FROM Kullanicilar WHERE KullaniciAdi = 'admin')
BEGIN
    INSERT INTO Kullanicilar (KullaniciAdi, Sifre, Ad, Soyad, Email, Telefon, Departman, Rol)
    VALUES ('admin', 'admin123', 'Admin', 'Kullanıcı', 'admin@example.com', '5551234567', 'Yönetim', 1);
END
GO

-- Örnek araçlar
IF NOT EXISTS (SELECT * FROM Araclar WHERE Plaka = '34 ABC 123')
BEGIN
    INSERT INTO Araclar (Marka, Model, Plaka, AracTipi, Durumu, VitesTipi, YakitTuru)
    VALUES 
    ('Toyota', 'Corolla', '34 ABC 123', 'Sedan', 'Müsait', 'Manuel', 'Benzin'),
    ('Honda', 'Civic', '34 DEF 456', 'Sedan', 'Müsait', 'Otomatik', 'Benzin'),
    ('Ford', 'Transit', '34 GHI 789', 'Minibüs', 'Müsait', 'Manuel', 'Dizel');
END
GO

PRINT 'Veritabanı başarıyla oluşturuldu!'; 
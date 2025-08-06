-- AracTakip Veri Yapısı Export Scripti
-- Bu script sadece tablo yapılarını ve temel verileri içerir

USE AracTakip;
GO

-- Tablo yapılarını oluştur
CREATE TABLE Kullanicilar (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    KullaniciAdi NVARCHAR(50) NOT NULL UNIQUE,
    Sifre NVARCHAR(100) NOT NULL,
    Ad NVARCHAR(50) NOT NULL,
    Soyad NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Telefon NVARCHAR(20),
    Departman NVARCHAR(50) NOT NULL,
    Rol BIT NOT NULL DEFAULT 0,
    Durumu NVARCHAR(20) DEFAULT 'Aktif',
    KayitTarihi DATETIME2 DEFAULT GETDATE()
);

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

CREATE TABLE Bildirimler (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    KullaniciID INT NOT NULL,
    Baslik NVARCHAR(100) NOT NULL,
    Mesaj NVARCHAR(500) NOT NULL,
    Tip NVARCHAR(20) NOT NULL,
    Okundu BIT DEFAULT 0,
    OlusturmaTarihi DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (KullaniciID) REFERENCES Kullanicilar(ID)
);

-- Temel verileri ekle
INSERT INTO Kullanicilar (KullaniciAdi, Sifre, Ad, Soyad, Email, Telefon, Departman, Rol)
VALUES ('admin', 'admin123', 'Admin', 'Kullanıcı', 'admin@example.com', '5551234567', 'Yönetim', 1);

INSERT INTO Araclar (Marka, Model, Plaka, AracTipi, Durumu, VitesTipi, YakitTuru)
VALUES 
('Toyota', 'Corolla', '34 ABC 123', 'Sedan', 'Müsait', 'Manuel', 'Benzin'),
('Honda', 'Civic', '34 DEF 456', 'Sedan', 'Müsait', 'Otomatik', 'Benzin'),
('Ford', 'Transit', '34 GHI 789', 'Minibüs', 'Müsait', 'Manuel', 'Dizel');

PRINT 'Veri yapısı başarıyla export edildi!'; 
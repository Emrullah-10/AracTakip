# 🚀 AracTakipSistemi Hosting Deployment Rehberi

## 📋 Adım Adım Deployment

### 1. **Veritabanı Oluşturma**

#### Seçenek A: SQL Script Kullanarak
1. Hosting kontrol panelinizde **phpMyAdmin** veya **SQL Server Management Studio** açın
2. `DatabaseScript.sql` dosyasını yükleyin ve çalıştırın
3. Veritabanı otomatik olarak oluşturulacak

#### Seçenek B: Entity Framework Migrations Kullanarak
```bash
# Projeyi hosting'e yükledikten sonra
dotnet ef database update
```

### 2. **Connection String Güncelleme**

`appsettings.json` dosyasını hosting bilgilerinizle güncelleyin:

```json
{
  "ConnectionStrings": {
    "BaglantiCumlesi": "Server=your-server-name;Database=AracTakip;User Id=your-username;Password=your-password;TrustServerCertificate=True;"
  }
}
```

### 3. **Hosting Sağlayıcıları İçin Özel Ayarlar**

#### 🟦 **Azure App Service**
```json
{
  "ConnectionStrings": {
    "BaglantiCumlesi": "Server=tcp:your-server.database.windows.net,1433;Database=AracTakip;User Id=your-username@your-server;Password=your-password;Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;"
  }
}
```

#### 🟩 **GoDaddy**
```json
{
  "ConnectionStrings": {
    "BaglantiCumlesi": "Server=your-server.secureserver.net;Database=AracTakip;User Id=your-username;Password=your-password;TrustServerCertificate=True;"
  }
}
```

#### 🟨 **Hostinger**
```json
{
  "ConnectionStrings": {
    "BaglantiCumlesi": "Server=your-server.hostinger.com;Database=AracTakip;User Id=your-username;Password=your-password;TrustServerCertificate=True;"
  }
}
```

### 4. **Publish İşlemi**

```bash
# Release build oluştur
dotnet publish -c Release -o ./publish

# Veya Visual Studio'dan
# Build > Publish > Folder > Publish
```

### 5. **Hosting'e Yükleme**

1. **FTP/File Manager** ile dosyaları yükleyin
2. **Web.config** dosyasını kontrol edin
3. **Application Pool** .NET Core 8.0 olarak ayarlayın

### 6. **Test Etme**

1. Ana sayfaya erişin
2. Admin girişi yapın: `admin` / `admin123`
3. Veritabanı bağlantısını test edin

## 🔧 Sorun Giderme

### ❌ "Log file size" Hatası
**Çözüm**: Veritabanını sıfırdan oluşturun, backup yüklemeyin

### ❌ "Connection String" Hatası
**Çözüm**: Hosting bilgilerinizi doğru girin

### ❌ "Entity Framework" Hatası
**Çözüm**: 
```bash
dotnet ef database update --connection "your-connection-string"
```

## 📞 Destek

Sorun yaşarsanız:
1. Hosting sağlayıcınızın destek ekibiyle iletişime geçin
2. Veritabanı boyutu sınırlamalarını öğrenin
3. Gerekirse daha yüksek plana geçin

## ✅ Kontrol Listesi

- [ ] Veritabanı oluşturuldu
- [ ] Connection string güncellendi
- [ ] Proje yayınlandı
- [ ] Test edildi
- [ ] Admin girişi çalışıyor 
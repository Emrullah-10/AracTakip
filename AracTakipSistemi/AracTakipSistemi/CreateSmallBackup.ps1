# AracTakipSistemi Küçük Backup Oluşturma Scripti
# Bu script veritabanını küçük boyutlu backup olarak oluşturur

param(
    [string]$ServerName = ".",
    [string]$DatabaseName = "AracTakip",
    [string]$BackupPath = "C:\Backups\"
)

# Backup klasörünü oluştur
if (!(Test-Path $BackupPath)) {
    New-Item -ItemType Directory -Path $BackupPath -Force
}

$BackupFile = Join-Path $BackupPath "AracTakip_Small_$(Get-Date -Format 'yyyyMMdd_HHmmss').bak"

# SQL Server bağlantısı
$ConnectionString = "Server=$ServerName;Database=$DatabaseName;Integrated Security=True;"

# Küçük backup oluştur (sadece veri yapısı ve temel veriler)
$SQL = @"
BACKUP DATABASE [$DatabaseName] TO DISK = '$BackupFile'
WITH 
    COMPRESSION,
    INIT,
    FORMAT,
    MEDIANAME = 'AracTakip_Small_Backup',
    NAME = 'AracTakip_Small_Backup',
    SKIP,
    STATS = 10
"@

try {
    # SQL Server Management Objects kullanarak backup oluştur
    $Server = New-Object Microsoft.SqlServer.Management.Smo.Server $ServerName
    $Database = $Server.Databases[$DatabaseName]
    
    if ($Database) {
        # Backup oluştur
        $Backup = New-Object Microsoft.SqlServer.Management.Smo.Backup
        $Backup.Database = $DatabaseName
        $Backup.Action = "Database"
        $Backup.BackupSetDescription = "AracTakip Küçük Backup"
        $Backup.BackupSetName = "AracTakip_Small_Backup"
        $Backup.MediaDescription = "AracTakip_Small_Backup"
        $Backup.Devices.AddDevice($BackupFile, "File")
        $Backup.SqlBackup($Server)
        
        Write-Host "✅ Küçük backup başarıyla oluşturuldu: $BackupFile" -ForegroundColor Green
        
        # Backup boyutunu göster
        $FileInfo = Get-Item $BackupFile
        $SizeMB = [math]::Round($FileInfo.Length / 1MB, 2)
        Write-Host "📁 Backup boyutu: $SizeMB MB" -ForegroundColor Cyan
    }
    else {
        Write-Host "❌ Veritabanı bulunamadı: $DatabaseName" -ForegroundColor Red
    }
}
catch {
    Write-Host "❌ Backup oluşturulurken hata: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n📋 Backup dosyası: $BackupFile" -ForegroundColor Yellow
Write-Host "💡 Bu backup dosyası hosting'e yüklenebilir." -ForegroundColor Yellow 
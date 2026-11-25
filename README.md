# YMH219 - TCP İstemci-Sunucu Chat Uygulaması

## Proje Açıklaması
Nesne Tabanlı Programlama (YMH 219) dersi kapsamında `TcpListener` ve `TcpClient` sınıfları ile geliştirilmiş, `async/await` tabanlı eş zamanlı istemci-sunucu sohbet uygulamasıdır.

## Kullanılan Teknoloji ve Paketler
- C# / .NET 8.0 (Windows Forms)
- `System.Net.Sockets` (`TcpListener`, `TcpClient`)
- `StreamReader` / `StreamWriter` (UTF-8)

## Kurulum ve Çalıştırma Adımları
1. Depoyu klonlayın:
   ```bash
   git clone https://github.com/255541002AsimGokalp/YMH219-ChatApp.git
   ```
2. `YMH219-ChatApp.sln` dosyasını Visual Studio 2022 ile açıp çalıştırın.
3. Başlatıcı pencereden önce **Sunucu Penceresi Aç** diyerek sunucuyu `5000` portunda başlatın.
4. Ardından bir veya birden fazla **İstemci Penceresi Aç** diyerek kullanıcı adlarıyla bağlanın ve mesajlaşın.

## Uygulamanın Temel Özellikleri
- Birden fazla istemcinin aynı anda sohbet odasına katılabilmesi (Broadcast)
- Sunucu tarafında bağlı istemcilerin kullanıcı adı, IP/port ve bağlantı zamanı ile listelenmesi
- Zaman damgalı mesaj akışı
- Bağlan, Mesaj Gönder ve Bağlantıyı Kes işlevleri
- Bağlantı kopması veya ulaşılamayan sunucu durumlarında güvenli hata yönetimi

## Ekran Görüntüleri
![Sunucu ve İstemci Mesajlaşma Ekranı](screenshots/screenshot1.png)
![Çoklu İstemci Sohbet Ekranı](screenshots/screenshot2.png)

## Bilinen Sınırlamalar / Geliştirme Notları
- Yerel bilgisayar (`127.0.0.1`) ve aynı yerel ağ (LAN) üzerinde çalışacak şekilde tasarlanmıştır.

## Öğrenci Bilgileri
- **Ad-Soyad:** Asım Gökalp
- **Öğrenci Numarası:** 255541002

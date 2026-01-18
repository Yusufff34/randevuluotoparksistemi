using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace OtoparkProjesi
{
    public static class VeriDeposu
    {
        private static List<Rezervasyon> _rezervasyonlar;
        private static string dosyaYolu = "otopark_verileri.xml";

        // Rezervasyon Listesi (Otomatik Yüklenir)
        public static List<Rezervasyon> Rezervasyonlar
        {
            get
            {
                if (_rezervasyonlar == null)
                {
                    _rezervasyonlar = Yukle();
                }
                return _rezervasyonlar;
            }
        }

        // Yeni Kayıt Ekleme Metodu (Hata Veren Kısım Burasıydı)
        public static void Ekle(Rezervasyon yeniKayit)
        {
            Rezervasyonlar.Add(yeniKayit);
            Kaydet();
        }

        // Kayıt Silme Metodu
        public static void Sil(Rezervasyon kayit)
        {
            Rezervasyonlar.Remove(kayit);
            Kaydet();
        }

        // --- Dosya İşlemleri (XML) ---
        private static void Kaydet()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<Rezervasyon>));
                using (var writer = new StreamWriter(dosyaYolu))
                {
                    serializer.Serialize(writer, _rezervasyonlar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kayıt hatası: " + ex.Message);
            }
        }

        private static List<Rezervasyon> Yukle()
        {
            if (!File.Exists(dosyaYolu)) return new List<Rezervasyon>();
            try
            {
                var serializer = new XmlSerializer(typeof(List<Rezervasyon>));
                using (var stream = new FileStream(dosyaYolu, FileMode.Open))
                {
                    if (stream.Length == 0) return new List<Rezervasyon>();
                    return (List<Rezervasyon>)serializer.Deserialize(stream);
                }
            }
            catch
            {
                return new List<Rezervasyon>();
            }
        }
    }
}
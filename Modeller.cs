using System;

namespace OtoparkProjesi
{
    public abstract class TemelVarlik
    {
        public string Id { get; set; }
        public DateTime OlusturmaTarihi { get; set; }

        protected TemelVarlik()
        {
            Id = Guid.NewGuid().ToString();
            OlusturmaTarihi = DateTime.Now;
        }
    }

    public interface IUcretHesaplayici
    {
        decimal Hesapla(DateTime giris, DateTime cikis);
    }

    public class StandartUcretHesaplayici : IUcretHesaplayici
    {
        private const decimal GUNLUK_UCRET = 250m;

        public decimal Hesapla(DateTime giris, DateTime cikis)
        {
            TimeSpan sure = cikis.Date - giris.Date;
            int gunSayisi = sure.Days;
            if (gunSayisi == 0) gunSayisi = 1;
            return gunSayisi * GUNLUK_UCRET;
        }
    }

    public class Rezervasyon : TemelVarlik
    {
        public string AdSoyad { get; set; }
        public string Telefon { get; set; }
        public string Plaka { get; set; }
        public DateTime GirisTarihi { get; set; }
        public DateTime CikisTarihi { get; set; }
        public string ParkYeriKodu { get; set; }
        public decimal Ucret { get; set; }
    }
}
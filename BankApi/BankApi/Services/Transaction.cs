using System;

namespace BankApi.Services
{
    public partial class Program
    {
        public class Transaction
        {
            public int Id { get; set; }
            public decimal Kwota { get; set; }
            public DateTime DataTransakcji { get; set;}
            public string NumerKontaNadawcy { get; set; }
            public string NumerKontaOdbiorcy { get; set; }
            public string Typ { get; set; }
            public Transaction() { }
            public Transaction(decimal kwota, DateTime dataTransakcji, string numerKontaNadawcy , string numerKontaOdbiorcy)
            {
                Kwota = kwota;
                DataTransakcji = dataTransakcji;
                NumerKontaNadawcy = numerKontaNadawcy;
                NumerKontaOdbiorcy = numerKontaOdbiorcy;
            }
            public void GetDetails()
            {
                Console.WriteLine($"Data transakcji: {DataTransakcji} \r\n" +
                                  $"Kwota transakcji: {Kwota} \r\n" +
                                  $"Numer konta nadawcy: {NumerKontaNadawcy} \r\n" +
                                  $"Numer konta odbiorcy: {NumerKontaOdbiorcy}");
            }

        }
    }
}

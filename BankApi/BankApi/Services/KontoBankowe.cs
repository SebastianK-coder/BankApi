using System;
using System.Collections.Generic;

namespace BankApi.Services
{
    public partial class Program
    {
        public class KontoBankowe
        {
            public string NumerKonta { get; set; }
            public string Wlasciciel { get; set; }
            public decimal Saldo { get; set; }
            public string Haslo { get; set; }
            public List<Transaction> Transakcje { get; set; } = new List<Transaction>();
            private KontoBankowe() { }

            public KontoBankowe(string numerkonta, string wlasciciel ,decimal saldo, string haslo)
            {
                NumerKonta = numerkonta;
                Wlasciciel = wlasciciel;
                Saldo = saldo;
                Haslo = haslo;
            }
            public void ZmienSaldo(decimal kwota)
            {
                Saldo += kwota;
            }
            public void PokazInfo()
            {
                Console.WriteLine($"Saldo wynosi {Saldo} zl");
                Console.WriteLine();
                foreach(Transaction t in Transakcje)
                {
                    t.GetDetails();
                    Console.WriteLine();
                }
            }

        }
    }
}

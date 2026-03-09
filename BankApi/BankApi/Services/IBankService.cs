using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApi.Services
{ 
    public partial class Program
    {
        public interface IBankService
        {
            Task<bool> DodajKonto(string numerKonta, string wlasciciel, string haslo);
            Task<bool> Wplac(string numerKonta, decimal kwota);
            Task<bool> Wyplac(string numerKonta, decimal kwota);
            Task<bool> Przelew(string numerNadawcy, string numerOdbiorcy, decimal kwota);
            Task<bool> Zaloguj(string numerKonta, string haslo);
            Task<KontoBankowe> PobierzKonto(string numerKonta);
            Task<int> ZwrocIloscKont();
            Task PokazHistorieKonta(string numerKonta);
            Task<bool> CzyIstniejeKonto(string numer);

        }
    }
}

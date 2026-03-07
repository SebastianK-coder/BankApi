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
            bool DodajKonto(string numerKonta, string wlasciciel, string haslo);
            bool Wplac(string numerKonta, decimal kwota);
            bool Wyplac(string numerKonta, decimal kwota);
            bool Przelew(string numerNadawcy, string numerOdbiorcy, decimal kwota);
            bool Zaloguj(string numerKonta, string haslo);
            KontoBankowe PobierzKonto(string numerKonta);
            int ZwrocIloscKont();
            void PokazHistorieKonta(string numerKonta);
            bool CzyIstniejeKonto(string numer);

        }
    }
}

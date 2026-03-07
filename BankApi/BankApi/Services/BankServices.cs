using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BankApi.Services
{
    public partial class Program
    {
        const int PoczatkoweSaldo = 0;
        public class BankServices : IBankService
        {
            private List<KontoBankowe> KontaBankowe = new List<KontoBankowe>();
            public BankServices()
            {
                WczytajWszystkieKonta();
            }
            public KontoBankowe ZnajdzKonto(string numerKonta)
            {
                return KontaBankowe.FirstOrDefault(k => k.NumerKonta == numerKonta);
            }
            public int ZwrocIloscKont()
            {
                return KontaBankowe.Count();
            }
            bool IBankService.CzyIstniejeKonto(string numer)
            {
                foreach (KontoBankowe k in KontaBankowe)
                {
                    if (k.NumerKonta == numer)
                    {
                        Console.WriteLine("Istnieje juz konto o takim numerze");
                        return true;
                    }
                }
                return false;
            }
            public void ZapiszKontoDoPliku(KontoBankowe konto)
            {
                try
                {
                    string folder = "Files";

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string sciezka = Path.Combine(folder, $"{konto.NumerKonta}.json");

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };

                    string json = JsonSerializer.Serialize(konto, options);
                    File.WriteAllText(sciezka, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Błąd zapisu konta:");
                    Console.WriteLine(ex.Message);
                }
            }
            public void WczytajWszystkieKonta()
            {
                try
                {
                    string sciezka = "Files";

                    if (!Directory.Exists(sciezka))
                        return;

                    string[] pliki = Directory.GetFiles(sciezka, "*.json");

                    foreach (var plik in pliki)
                    {
                        string json = File.ReadAllText(plik);

                        var konto = JsonSerializer.Deserialize<KontoBankowe>(json);

                        if (konto != null) KontaBankowe.Add(konto);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Błąd podczas wczytywania kont:");
                    Console.WriteLine(ex.Message);
                }
            }

            bool IBankService.Wplac(string numerKonta, decimal kwota)
            {
                KontoBankowe szukane = ZnajdzKonto(numerKonta);
                if (szukane != null)
                {
                    szukane.ZmienSaldo(kwota);
                    szukane.transakcje.Add(new Transaction(kwota, DateTime.Now, "Brak nadawcy. Wplata srodkow", numerKonta));
                    ZapiszKontoDoPliku(szukane);
                    return true;
                }
                return false;
            }

            bool IBankService.Wyplac(string numerKonta, decimal kwota)
            {
                KontoBankowe szukane = ZnajdzKonto(numerKonta);
                if (szukane != null)
                {
                    if (szukane.Saldo >= kwota)
                    {
                        szukane.ZmienSaldo(-kwota);
                        szukane.transakcje.Add(new Transaction(kwota, DateTime.Now, numerKonta, "Brak odbiorcy. Wyplata srodkow"));
                        ZapiszKontoDoPliku(szukane);
                        return true;

                    }
                }
                return false;
            }

            bool IBankService.Przelew(string numerNadawcy, string numerOdbiorcy, decimal kwota)
            {
                KontoBankowe szukane1 = ZnajdzKonto(numerNadawcy);
                KontoBankowe szukane2 = ZnajdzKonto(numerOdbiorcy);
                if (szukane1 != null && szukane2 != null)
                {
                    if (szukane1.Saldo >= kwota)
                    {
                        szukane1.ZmienSaldo(-kwota);
                        szukane1.transakcje.Add(new Transaction(kwota, DateTime.Now, numerNadawcy, numerOdbiorcy));
                        ZapiszKontoDoPliku(szukane1);
                        szukane2.ZmienSaldo(kwota);
                        szukane2.transakcje.Add(new Transaction(kwota, DateTime.Now, numerNadawcy, numerOdbiorcy));
                        ZapiszKontoDoPliku(szukane2);
                        return true;
                    }
                }
                return false;
            }

            KontoBankowe IBankService.PobierzKonto(string numerKonta)
            {
                KontoBankowe szukane = KontaBankowe.FirstOrDefault(k => k.NumerKonta == numerKonta);
                if(szukane != null)
                {
                    return szukane;
                }
                return null;
            }

            bool IBankService.DodajKonto(string numerKonta, string wlasciciel, string haslo)
            {
                if(CzyPoprawnyNumerKonta(numerKonta))
                {
                    KontaBankowe.Add(new KontoBankowe(numerKonta, wlasciciel, PoczatkoweSaldo, haslo));
                    ZapiszKontoDoPliku(KontaBankowe.Last());
                    return true;
                }
                return false;
                
            }
            void IBankService.PokazHistorieKonta(string numerKonta)
            {
                KontoBankowe szukane = ZnajdzKonto(numerKonta);
                szukane.PokazSaldo();
            }
            bool IBankService.Zaloguj(string numerKonta, string haslo)
            {
                KontoBankowe szukane = KontaBankowe.FirstOrDefault(k => k.NumerKonta == numerKonta);
                if (szukane != null)
                { 
                    if (szukane.Haslo == haslo) return true;
                }
                return false;
            }
            bool CzyPoprawnyNumerKonta(string numerKonta)
            {
                if (numerKonta.Length == 26 && Regex.IsMatch(numerKonta.Replace(" ", ""), @"^\d{26}$")) return true;
                else return false;
            }
        }
    }
}

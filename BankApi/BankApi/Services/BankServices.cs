using BankApi.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services
{
    public partial class Program
    {

        const int PoczatkoweSaldo = 0;
        public class BankServices : IBankService
        {
            private readonly BankingDbContext _context;
            public BankServices(BankingDbContext context)
            {
                _context = context;
            }
            public async Task<KontoBankowe> ZnajdzKonto(string numerKonta)
            {
                return await _context.Accounts.FirstOrDefaultAsync(k => k.NumerKonta == numerKonta);
            }
            public async Task<int> ZwrocIloscKont()
            {
                return _context.Accounts.Count();
            }
            async Task<bool> IBankService.CzyIstniejeKonto(string numer)
            {
                if(_context.Accounts.FirstOrDefault(k => k.NumerKonta == numer) == null)
                    return false;
                return true;
            }

            async Task<bool> IBankService.Wplac(string numerKonta, decimal kwota)
            {
                var szukane = await ZnajdzKonto(numerKonta);

                if (szukane == null)
                    return false;

                szukane.ZmienSaldo(kwota);

                var transakcja = new Transaction
                {
                    NumerKontaNadawcy = "BRAK",
                    NumerKontaOdbiorcy = numerKonta,
                    Kwota = kwota,
                    DataTransakcji = DateTime.Now,
                    Typ = "Wplata"
                };

                _context.Transactions.Add(transakcja);

                await _context.SaveChangesAsync();

                return true;
            }

            async Task<bool> IBankService.Wyplac(string numerKonta, decimal kwota)
            {
                var konto = await ZnajdzKonto(numerKonta);

                if (konto == null)
                    return false;

                konto.ZmienSaldo(-kwota);

                var transakcja = new Transaction
                {
                    NumerKontaNadawcy = numerKonta,
                    NumerKontaOdbiorcy = "BRAK",
                    Kwota = kwota,
                    DataTransakcji = DateTime.Now,
                    Typ = "Wyplata"
                };

                _context.Transactions.Add(transakcja);

                await _context.SaveChangesAsync();

                return true;
            }


                public async Task<bool> Przelew(string numerNadawcy, string numerOdbiorcy, decimal kwota)
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        var kontoNadawcy = await ZnajdzKonto(numerNadawcy);
                        var kontoOdbiorcy = await ZnajdzKonto(numerOdbiorcy);

                        if (kontoNadawcy == null || kontoOdbiorcy == null)
                            return false;

                        kontoNadawcy.ZmienSaldo(-kwota);
                        kontoOdbiorcy.ZmienSaldo(kwota);

                        var transakcja = new Transaction
                        {
                            NumerKontaNadawcy = numerNadawcy,
                            NumerKontaOdbiorcy = numerOdbiorcy,
                            Kwota = kwota,
                            DataTransakcji = DateTime.Now,
                            Typ = "Przelew"
                        };

                        _context.Transactions.Add(transakcja);

                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return true;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

            async Task<KontoBankowe> IBankService.PobierzKonto(string numerKonta)
            {
                KontoBankowe szukane = _context.Accounts.FirstOrDefault(k => k.NumerKonta == numerKonta);
                if(szukane != null)
                {
                    return szukane;
                }
                return null;
            }

            async Task<bool> IBankService.DodajKonto(string numerKonta, string wlasciciel, string haslo)
            {
                if (await CzyPoprawnyNumerKonta(numerKonta))
                {
                    var konto = new KontoBankowe(numerKonta, wlasciciel, PoczatkoweSaldo, haslo);

                    _context.Accounts.Add(konto);

                    await _context.SaveChangesAsync();

                    return true;
                }
                return false;

                
            }
            async Task IBankService.PokazHistorieKonta(string numerKonta)
            {
                KontoBankowe szukane = await ZnajdzKonto(numerKonta);
                szukane.PokazInfo();
            }
            async Task<bool> IBankService.Zaloguj(string numerKonta, string haslo)
            {
                var szukane = await _context.Accounts
                .FirstOrDefaultAsync(k => k.NumerKonta == numerKonta);
                if (szukane != null && szukane.Haslo == haslo)
                    return true;
                return false;
            }
            async Task<bool> CzyPoprawnyNumerKonta(string numerKonta)
            {
                if (numerKonta.Length == 26 && Regex.IsMatch(numerKonta.Replace(" ", ""), @"^\d{26}$")) return true;
                else return false;
            }
        }
    }
}

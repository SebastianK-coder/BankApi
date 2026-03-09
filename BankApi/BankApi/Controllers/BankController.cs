using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static BankApi.Services.Program;
using static BankApi.Data.BankingDbContext;
using BankApi.Data;
using SQLitePCL;
[Authorize]
[ApiController]
[Route("api/[controller]")]

public class BankController : ControllerBase
{
    private IBankService _bank;
    public BankController(IBankService bank)
    {
        _bank = bank;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(string numerKonta, string haslo)
    {
        if (await _bank.Zaloguj(numerKonta, haslo))
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("1234567890abcdefghijklmopqrstuwvxyz");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, numerKonta)
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new { token = tokenString });
        }
        return Unauthorized("Błędne dane logowania");
    }
    [AllowAnonymous]
    [HttpGet("display-account-number")]
    public async Task<IActionResult> ZwrocIloscKont()
    {
        return Ok(_bank.ZwrocIloscKont());
    }
    [HttpGet("account/display-info")]
    public async Task<IActionResult> PobierzKonto()
    {
        var konto = _bank.PobierzKonto(User.Identity.Name);
        if (konto == null) return NotFound("Nie znaleziono konta");
        return Ok(konto);
    }
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> DodajKonto([FromBody] KontoBankowe konto)
    {
        if (await _bank.CzyIstniejeKonto(konto.NumerKonta)) 
            return BadRequest("Konto już istnieje");
        if (await _bank.DodajKonto(konto.NumerKonta, konto.Wlasciciel, konto.Haslo))
            return Ok("Pomdyslnie dodano konto");
        return BadRequest("Zly numer konta");
    }
    [HttpPut("deposit")]
    public async Task<IActionResult> Wplata(decimal kwota)
    {
        bool wynik = await _bank.Wplac(User.Identity.Name, kwota);
        if (wynik) 
            return Ok("Pomyslnie wplacono pieniadze");
        else 
            return BadRequest("Nie znaleziono konta");
    }
    [HttpPut("withdraw")]
    public async Task<IActionResult> Wyplata(decimal kwota)
    {
        bool wynik = await _bank.Wyplac(User.Identity.Name, kwota);
        if (wynik) 
            return Ok("Pomyslnie wyplacono pieniadze");
        else 
            return BadRequest("Blad przy wprowadzaniu danych");
    }
    [HttpPut("transfer")]
    public async Task<IActionResult> Przelew(string numerOdbiorcy, decimal kwota)
    {
        if (User.Identity.Name == numerOdbiorcy)
            return BadRequest("Nie można zrobić przelewu na własne konto");
        bool wynik = await _bank.Przelew(User.Identity.Name, numerOdbiorcy, kwota);
        if (wynik)
            return Ok("Pomyslnie wykonano przelew");
        else 
            return BadRequest("Blad przy wprowadzaniu danych");
    }
}

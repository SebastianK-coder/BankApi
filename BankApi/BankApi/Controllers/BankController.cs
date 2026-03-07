using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static BankApi.Services.Program;
[Authorize]
[ApiController]
[Route("api/[controller]")]

public class BankController : ControllerBase
{
    private readonly IBankService _bank;
    public BankController(IBankService bank)
    {
        _bank = bank;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(string numerKonta, string haslo)
    {
        if (!_bank.Zaloguj(numerKonta, haslo))
            return Unauthorized("Błędne dane logowania");

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
    [AllowAnonymous]
    [HttpGet("display-account-number")]
    public IActionResult ZwrocIloscKont()
    {
        return Ok(_bank.ZwrocIloscKont());
    }
    [HttpGet("account/display-info")]
    public IActionResult PobierzKonto()
    {
        var konto = _bank.PobierzKonto(User.Identity.Name);
        if (konto == null) return NotFound("Nie znaleziono konta");
        return Ok(konto);
    }
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult DodajKonto([FromBody] KontoBankowe konto)
    {
        if (_bank.CzyIstniejeKonto(konto.NumerKonta)) 
            return BadRequest("Konto już istnieje");
        if (!_bank.DodajKonto(konto.NumerKonta, konto.Wlasciciel, konto.Haslo)) 
            return BadRequest("Zly numer konta");
        return Ok(konto);
    }
    [HttpPut("deposit")]
    public IActionResult Wplata(decimal kwota)
    {
        if (_bank.Wplac(User.Identity.Name, kwota)) 
            return Ok("Pomyslnie wplacono pieniadze");
        else 
            return BadRequest("Nie znaleziono konta");
    }
    [HttpPut("withdraw")]
    public IActionResult Wyplata(decimal kwota)
    {
        if (_bank.Wyplac(User.Identity.Name, kwota)) 
            return Ok("Pomyslnie wyplacono pieniadze");
        else 
            return BadRequest("Blad przy wprowadzaniu danych");
    }
    [HttpPut("transfer")]
    public IActionResult Przelew(string numerOdbiorcy, decimal kwota)
    {
        if (User.Identity.Name == numerOdbiorcy)
            return BadRequest("Nie można zrobić przelewu na własne konto");
        if (_bank.Przelew(User.Identity.Name, numerOdbiorcy, kwota))
            return Ok("Pomyslnie wykonano przelew");
        else 
            return BadRequest("Blad przy wprowadzaniu danych");
    }
}

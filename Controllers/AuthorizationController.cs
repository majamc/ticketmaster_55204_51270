using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager; //zarzadzanie uzytkownikami
    private readonly IConfiguration _configuration; //odczyt danych z pliku appsettings.json

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")] //POST /api/auth/register
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email }; //tworzeni uzytkownika z emailem jako loginem
        var result = await _userManager.CreateAsync(user, request.Password); //tworzenie uzytkownika i haslo

        if (!result.Succeeded) //jesli sie nie uda to zwraca blad
            return BadRequest(result.Errors);

        return Ok("Rejestracja udana");
    }

    [HttpPost("login")] //POST /api/auth/login
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email); //szukanie usera po emailu
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password)) //sprawdzenie czy haslo sie zgadza
            return Unauthorized("Nieprawid?owe dane logowania");

        var token = GenerateJwtToken(user); //jesli dane sa poprawne to generuje sie token JWT
        return Ok(new { token });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            //tworzenie danych do tokena JWT
            new Claim(JwtRegisteredClaimNames.Sub, user.Email), //Sub - uzytkownik
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //jti - unikalne ID tokena
            new Claim(ClaimTypes.NameIdentifier, user.Id) //ID usera
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); //tworzy klucz
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //podpis

        var token = new JwtSecurityToken( //tworzenie tokenu
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), //token bedzie wazny 1h
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

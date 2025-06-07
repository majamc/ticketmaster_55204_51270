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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            throw new BadRequestException("Invalid data."); //nieprawidlowe dane wejsciowe
        }

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new BadRequestException("Please enter your email.");

        if (!request.Email.Contains("@"))
            throw new BadRequestException("Please enter a valid email.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("Please enter your password.");

        if (!request.Password.Any(char.IsUpper))
            throw new BadRequestException("Password must contain at least one uppercase letter.");

        if (!request.Password.Any(char.IsDigit))
            throw new BadRequestException("Password must contain at least one digit.");

        if (request.Password.Length < 8)
            throw new BadRequestException("Password has to be at least 8 characters long.");

        if (!request.Password.Any(c => "!@#$%^&*()_+-=[]{};':\",.<>/?\\|".Contains(c)))
            throw new BadRequestException("Password must contain at least one special character.");

        var existingUser = await _userManager.FindByEmailAsync(request.Email); 
        if (existingUser != null)
            throw new ConflictException("Email taken.");

        var user = new ApplicationUser { UserName = request.Email, Email = request.Email }; //tworzenie uzytkownika z emailem jako loginem
        var result = await _userManager.CreateAsync(user, request.Password); //tworzenie uzytkownika i haslo

        return Ok("Registration succesful");
    }

    [HttpPost("login")] //POST /api/auth/login
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new BadRequestException("Please enter your email.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("Please enter your password.");

        if (!request.Email.Contains("@"))
            throw new BadRequestException("Please enter a valid email.");

        var user = await _userManager.FindByEmailAsync(request.Email); //szukanie usera po emailu
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password)) //sprawdzenie czy haslo sie zgadza
            throw new UnauthorizedException("Incorrect email or password");

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

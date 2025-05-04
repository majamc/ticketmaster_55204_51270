using ConcertTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ConcertTracker.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient(); //umozliwia wykonywanie zapytan HTTP przez klienta
builder.Services.AddHttpClient<TicketmasterService>(); //rejestracja klienta HTTP dla TicketmasterService
builder.Services.AddControllers(); //dodanie kontrolerow

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>() //rejestracja uslug do logowania i uzykownikow
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"], //nadawca tokenu (czyli nasza aplikacja)
        ValidAudience = builder.Configuration["Jwt:Audience"], //odbiorca tokenu, klient
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) //klucz do podpisywania tokenow
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); //swagger

builder.Services.AddHttpClient<SetlistFmService>();

var app = builder.Build(); //budowanie aplikacji

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); //autentykacja
app.UseAuthorization(); //autoryzacja
app.MapControllers();
app.Run(); //start aplikacji

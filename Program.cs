using ConcertTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja klienta HTTP do korzystania z us�ug, takich jak TicketmasterService
builder.Services.AddHttpClient();
// Rejestracja klienta HTTP dla TicketmasterService
builder.Services.AddHttpClient<TicketmasterService>();
// Rejestracja kontroler�w
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Rejestracja Swaggera
builder.Services.AddSwaggerGen();

// Rejestracja klienta HTTP dla SetList
builder.Services.AddHttpClient<SetlistFmService>();

//builder.Services.AddAuthorization();
// Rejestracja serwisu TicketmasterService z zakresem 'Scoped' (jeden egzemplarz na ka�de ��danie HTTP)
//builder.Services.AddScoped<TicketmasterService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

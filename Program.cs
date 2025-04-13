using ConcertTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja klienta HTTP do korzystania z us³ug, takich jak TicketmasterService
builder.Services.AddHttpClient();
// Rejestracja klienta HTTP dla TicketmasterService
builder.Services.AddHttpClient<TicketmasterService>();
// Rejestracja kontrolerów
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Rejestracja Swaggera
builder.Services.AddSwaggerGen();

// Rejestracja klienta HTTP dla SetList
builder.Services.AddHttpClient<SetlistFmService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

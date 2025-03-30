using ConcertTracker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient(); //umozliwia wykonywanie zapytan HTTP przez klienta
builder.Services.AddHttpClient<TicketmasterService>(); //rejestracja klienta HTTP dla TicketmasterService
builder.Services.AddControllers(); //dodanie kontrolerow
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); //swagger

var app = builder.Build(); //budowanie aplikacji

if (app.Environment.IsDevelopment()) //uzywanie swaggera
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers(); //endpointy
app.Run(); //start aplikacji

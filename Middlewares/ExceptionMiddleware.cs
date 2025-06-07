using System.Net;
using System.Text.Json;

namespace ConcertTracker.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next; //delegat do kolejnego middleware w kolejce.
        //po zakonczeniu naszego kodu, wolamy _next(context) zeby przekazac sterowanie dalej.
        private readonly ILogger<ExceptionMiddleware> _logger; //logger do logowania bledow

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        { //konstruktor
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) //glowna metoda wywolywana automatycznie przy kazdym zadaniu
        {
            try
            {
                await _next(context); //przekazanie dalej zadania do nastepnych warstw (np. kontrolera)
            }
            catch (Exception ex) //jesli wystapi wyjatek
            {
                _logger.LogError(ex, "An error occurred"); //logowanie wyjatku

                context.Response.ContentType = "application/json";
                //500 to bedzie domyslny blad
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError; //jesli nie przwidzimy jakiegos bledu ty bedzie blad 500
                string message = "Internal server error";

                switch (ex)
                {
                    case BadRequestException:
                        statusCode = HttpStatusCode.BadRequest;
                        message = ex.Message;
                        break;
                    case UnauthorizedException:
                        statusCode = HttpStatusCode.Unauthorized;
                        message = ex.Message;
                        break;
                    case NotFoundException:
                        statusCode = HttpStatusCode.NotFound;
                        message = ex.Message;
                        break;
                    case ConflictException:
                        statusCode = HttpStatusCode.Conflict;
                        message = ex.Message;
                        break;

                }

                context.Response.StatusCode = (int)statusCode;

                var response = new
                { //tworzenie obiektu json z info o bledzie
                    statusCode = (int)statusCode,
                    error = message,
                    details = ex.Message //szczegoly wyjatku
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response)); //Wysylanie JSON jako odpowied? dla u?ytkownika
            }
        }
    }
}
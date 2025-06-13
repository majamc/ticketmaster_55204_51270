using System.Net;
using System.Text.Json;

namespace ConcertTracker.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next; //delegat do kolejnego middleware w kolejce.
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); //przekazanie dalej zadania do nastepnych warstw (np. kontrolera)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred"); //logowanie wyjatku

                context.Response.ContentType = "application/json";
                HttpStatusCode statusCode = HttpStatusCode.InternalServerError; //jesli nie przwidziany blad to bedzie blad 500
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

                await context.Response.WriteAsync(JsonSerializer.Serialize(response)); //wysylanie JSON jako odpowiedz dla uzytkownika
            }
        }
    }
}
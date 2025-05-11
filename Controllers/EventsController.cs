using Microsoft.AspNetCore.Mvc;
using ConcertTracker.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcertTracker.Models;
using ConcertTracker.Models.TicketmasterAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ConcertTracker.Controllers
{
    /// <summary>
    /// Kontroler obsługujący żądania związane z wydarzeniami z TicketmasterApi.
    /// </summary>

    //[AllowAnonymous] //zmienic potem na [Authorize] 
    [Authorize]
    [ApiController]
    [Route("api/events")] //adres bazowy
    public class EventsController : ControllerBase
    {
        private readonly TicketmasterService _ticketmasterService;

        // Serwis do pobierania topowych piosenek artysty z Setlist.fm
        private readonly SetlistFmService _setlistFmService;

        /// <summary>
        /// Konstruktor kontrolera, wstrzykujący zależności serwisów Ticketmaster i Setlist.fm.
        /// </summary>
        public EventsController(TicketmasterService ticketmasterService, SetlistFmService setlistFmService)
        {
            _ticketmasterService = ticketmasterService;
            _setlistFmService = setlistFmService;
        }

        /// <summary>
        /// Endpoint zwracający listę wydarzeń na podstawie słowa kluczowego (np. nazwa zespołu, miasto),
        /// wraz z 3 najpopularniejszymi piosenkami artysty (jeśli dostępne).
        /// </summary>
        /// <param name="keyword">Słowo kluczowe do wyszukania wydarzeń.</param>
        /// <returns>Lista wydarzeń z przypisanymi piosenkami.</returns>
        [HttpGet("{keyword}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EventWithSongsResponse>>> GetEventsWithSongs(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new BadRequestException("Pole nie może być puste.");

            // Pobierz wydarzenia pasujące do słowa kluczowego z Ticketmaster
            var events = await _ticketmasterService.GetEventsAsync(keyword);

            // Sprawdzenie, czy API zwróciło wydarzenia
            if (events == null || !events.Any())
            {
                throw new NotFoundException("Brak wydarzeń dla podanego słowa kluczowego.");
            }

            var result = new List<EventWithSongsResponse>();

            // Iteruj przez każde wydarzenie
            foreach (var ev in events)
            {
                try
                {
                    // Pobierz listę topowych piosenek dla artysty (na podstawie nazwy wydarzenia)
                    var songs = await _setlistFmService.GetTopSongsAsync(ev.Name);

                    // Jeśli brak wyników – ustaw pustą listę
                    if (songs == null || !songs.Any())
                    {
                        songs = new List<string> { "Brak danych o piosenkach" };
                    }

                    // Dodaj wydarzenie z przypisanymi piosenkami do listy wynikowej
                    result.Add(new EventWithSongsResponse
                    {
                        Name = ev.Name,
                        Date = ev.Date,
                        Venue = ev.Venue,
                        TopSongs = songs
                    });
                }
                catch (Exception ex)
                {
                    // Obsługa błędu – jeżeli nie udało się pobrać piosenek, dodaj komunikat zastępczy
                    result.Add(new EventWithSongsResponse
                    {
                        Name = ev.Name,
                        Date = ev.Date,
                        Venue = ev.Venue,
                        TopSongs = new List<string> { "Błąd podczas pobierania piosenek: " + ex.Message }
                    });
                }
            }

            // Zwróć gotową listę wydarzeń z przypisanymi piosenkami
            return Ok(result);
        }
    }
}

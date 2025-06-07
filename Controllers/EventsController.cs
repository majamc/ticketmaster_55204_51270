using Microsoft.AspNetCore.Mvc;
using ConcertTracker.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcertTracker.Models;
using ConcertTracker.Models.TicketmasterAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ConcertTracker.Controllers
{
    // Kontroler obsługujący żądania związane z wydarzeniami z TicketmasterApi.
    [Authorize]
    [ApiController]
    [Route("api/events")] //adres bazowy
    public class EventsController : ControllerBase
    {
        private readonly TicketmasterService _ticketmasterService;

        // Serwis do pobierania topowych piosenek artysty z Setlist.fm
        private readonly SetlistFmService _setlistFmService;

        // Konstruktor kontrolera, wstrzykujący zależności serwisów Ticketmaster i Setlist.fm.
        public EventsController(TicketmasterService ticketmasterService, SetlistFmService setlistFmService)
        {
            _ticketmasterService = ticketmasterService;
            _setlistFmService = setlistFmService;
        }

        // Endpoint zwracający listę wydarzeń na podstawie słowa kluczowego (np. nazwa zespołu, miasto),
        // wraz z 3 najpopularniejszymi piosenkami artysty (jeśli dostępne).
        [HttpGet("{keyword}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EventWithSongsResponse>>> GetEventsWithSongs(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new BadRequestException("Enter a artist/band name");

            // Pobierz wydarzenia pasujące do słowa kluczowego z Ticketmaster
            var events = await _ticketmasterService.GetEventsAsync(keyword);

            // Sprawdzenie, czy API zwróciło wydarzenia
            if (events == null || !events.Any())
            {
                throw new NotFoundException("No concerts for that artist/band.");
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
                        songs = new List<string> { "No data about top songs" };
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
                        TopSongs = new List<string> { "Error while loading songs: " + ex.Message }
                    });
                }
            }

            // Zwróć gotową listę wydarzeń z przypisanymi piosenkami
            return Ok(result);
        }
    }
}

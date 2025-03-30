using Microsoft.AspNetCore.Mvc;
using ConcertTracker.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcertTracker.Models;
using ConcertTracker.Models.TicketmasterAPI.Models;

namespace ConcertTracker.Controllers
{
    /// <summary>
    /// Kontroler obsługujący żądania związane z wydarzeniami z TicketmasterApi.
    /// </summary>

    [ApiController]
    [Route("api/events")] //adres bazowy
    public class EventsController : ControllerBase
    {
        private readonly TicketmasterService _ticketmasterService;

        /// <summary>
        /// Konstruktor kontrolera, który przyjmuje serwis Ticketmaster.
        /// </summary>
        /// <param name="ticketmasterService">Serwis odpowiedzialny za pobieranie wydarzeń z Ticketmaster.</param>
        public EventsController(TicketmasterService ticketmasterService)
        {
            _ticketmasterService = ticketmasterService;
        }

        /// <summary>
        /// Pobiera listę wydarzeń na podstawie podanego słowa kluczowego.
        /// </summary>
        /// <param name="keyword">Słowo kluczowe do wyszukiwania wydarzeń (np. nazwa zespołu, miasto).</param>
        /// <returns>Lista wydarzeń spełniających kryteria wyszukiwania.</returns>
        [HttpGet("{keyword}")]
        public async Task<ActionResult<List<EventModel>>> GetEvents(string keyword)
        {
            var events = await _ticketmasterService.GetEventsAsync(keyword);
            return Ok(events); //zwraca liste wydarzen w formacie JSON z kodem http 200 OK
        }
    }
}

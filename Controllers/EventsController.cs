using Microsoft.AspNetCore.Mvc;
using ConcertTracker.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConcertTracker.Models;
using ConcertTracker.Models.TicketmasterAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ConcertTracker.Controllers
{
    [Authorize]
    //[AllowAnonymous] //potem usunac
    [ApiController]
    [Route("api/events")] //adres bazowy
    public class EventsController : ControllerBase
    {
        private readonly TicketmasterService _ticketmasterService; //serwis tickemastera do koncertow
        private readonly SpotifyService _SpotifyService; //serwis spotify do zdj artysty i topowych piosenek

        //wstrzykiwanie zaleznosci serwisow
        public EventsController(TicketmasterService ticketmasterService, SpotifyService setlistFmService)
        {
            _ticketmasterService = ticketmasterService;
            _SpotifyService = setlistFmService;
        }

        //zwracajanie listy wydarzen na podstawie slowa kluczowego z piosenkami i zdjeciem artysty
        [HttpGet("{keyword}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<EventWithSongsResponse>>> GetEventsWithSongs(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new BadRequestException("Enter a artist/band name");

            //pobieranie wydarzenia pasujacego do slowa kluczowego z ticketmastera
            var events = await _ticketmasterService.GetEventsAsync(keyword);

            if (events == null || !events.Any()) //czy API zwrocilo wydarzenia
            {
                throw new NotFoundException("No concerts for that artist/band.");
            }

            var result = new List<EventWithSongsResponse>();

            //iteruj przez kazde wydarzenie
            foreach (var ev in events)
            {
                try
                {
                   //var songs = await _setlistFmService.GetTopTracksAsync(ev.Name);
                    var songs = await _SpotifyService.GetTopTracksAsync(keyword); //pobieranie top 5 piosenek artysty
                    var imageUrl = await _SpotifyService.GetArtistImageUrlAsync(keyword); //pobieranie zdj

                    Console.WriteLine("NAME: " +keyword);

                    if (songs == null || !songs.Any()) //jesli brak wynikow
                    {
                        songs = new List<string> { "No data about top songs" };
                    }

                    result.Add(new EventWithSongsResponse //dodanie wydarzenia z przypisanymi piosenkami do listy wynikowej
                    {
                        Name = ev.Name,
                        Date = ev.Date,
                        Venue = ev.Venue,
                        TopSongs = songs,
                        ArtistImageUrl = imageUrl ?? ""
                    });
                }
                catch (Exception ex)
                {
                    //jesli sie nie udalo pobrac piosenek
                    result.Add(new EventWithSongsResponse
                    {
                        Name = ev.Name,
                        Date = ev.Date,
                        Venue = ev.Venue,
                        TopSongs = new List<string> { "Error while loading songs: " + ex.Message },
                        ArtistImageUrl = ""
                    });
                }
            }

            //zwrocenie listy wydarzen z przypisanymi piosenkami i zdjeciem artysty
            return Ok(result);
        }
    }
}

using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ConcertTracker.Models.TicketmasterAPI.Models;

namespace ConcertTracker.Services
{
    //komunikacja z API Ticketmaster
    public class TicketmasterService
    {
        private readonly HttpClient _httpClient; //klient HTTP do komunikacji z API
        private const string ApiKey = "AGNZBZex35RIvQP6xTYFAPHD8KqrIZqG"; //klucz API
        private const string BaseUrl = "https://app.ticketmaster.com/discovery/v2/events.json"; //adres API

        public TicketmasterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EventModel>> GetEventsAsync(string keyword, int size = 20) //pobiera liste wydarzen na podstawie slowa kluczowego
        {
            string startDate = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"); //ustawienie daty początkowej jako obecna data

            //tworzenie URL do API z wymaganymi parametrami
            var url = $"{BaseUrl}?apikey={ApiKey}&keyword={keyword}&classificationName=music&size={size}&startDateTime={startDate}";

            var response = await _httpClient.GetStringAsync(url); //wyslanie zapytania do API
            var data = JsonConvert.DeserializeObject<dynamic>(response); //wyslanie zapytania do API

            if (data?._embedded?.events == null) return new List<EventModel>(); //sprawdzenie czy API zwrocilo jakies wyniki

            //pobieranie wydarzenia z odpowiedzi
            var events = data._embedded.events as IEnumerable<dynamic>;
            if (events == null) return new List<EventModel>(); //czy udalo się odczytac i przekonwertowac dane

            var filteredEvents = events.Select(e => new EventModel //z kazdego wyniku tworzy sie nowy obiekt EventModel
            {
                Name = e.name.ToString(),
                Date = e.dates.start.localDate.ToString(),
                Venue = (e._embedded?.venues != null && e._embedded.venues.Count > 0 && e._embedded.venues[0] != null)
                    ? e._embedded.venues[0].name?.ToString() + ", " + e._embedded.venues[0].city?.name?.ToString()
                    : "Nieznana lokalizacja",
            }).ToList();

            return filteredEvents; //zwrocenie listy koncertow
        }
    }
}

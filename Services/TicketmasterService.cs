using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ConcertTracker.Models.TicketmasterAPI.Models;

namespace ConcertTracker.Services
{
    // Serwis do komunikacji z API Ticketmaster
    public class TicketmasterService
    {
        private readonly HttpClient _httpClient; // Klient HTTP do komunikacji z API
        private const string ApiKey = "AGNZBZex35RIvQP6xTYFAPHD8KqrIZqG"; //klucz API
        private const string BaseUrl = "https://app.ticketmaster.com/discovery/v2/events.json"; // Adres API

        public TicketmasterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Pobiera listę wydarzeń (koncertów) na podstawie słowa kluczowego.
        /// </summary>
        /// <param name="keyword">Słowo kluczowe do wyszukiwania (np. nazwa zespołu)</param>
        /// <param name="size">Liczba wyników, które mają zostać pobrane (domyślnie 20)</param>
        /// <returns>Lista wydarzeń (koncertów) z API Ticketmaster</returns>
        public async Task<List<EventModel>> GetEventsAsync(string keyword, int size = 20)
        {
            // Ustawienie daty początkowej jako obecna data UTC
            string startDate = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
            // Tworzymy URL do API z wymaganymi parametrami
            var url = $"{BaseUrl}?apikey={ApiKey}&keyword={keyword}&classificationName=music&size={size}&startDateTime={startDate}";

            // Wysyłamy zapytanie do API
            var response = await _httpClient.GetStringAsync(url);
            // Deserializacja odpowiedzi JSON z API
            var data = JsonConvert.DeserializeObject<dynamic>(response);

            // Sprawdzamy, czy istnieją jakiekolwiek wydarzenia w odpowiedzi
            if (data?._embedded?.events == null) return new List<EventModel>();

            // Pobieramy wydarzenia z odpowiedzi
            var events = data._embedded.events as IEnumerable<dynamic>;
            if (events == null) return new List<EventModel>();

            var filteredEvents = events.Select(e => new EventModel
            {
                Name = e.name.ToString(),
                Date = e.dates.start.localDate.ToString(),
                // Lokalizacja wydarzenia (sprawdzamy, czy są dostępne dane o miejscach)
                Venue = e._embedded?.venues != null && e._embedded.venues.HasValues
                    ? e._embedded.venues[0].name.ToString()
                    : "Nieznana lokalizacja"
            }).ToList();

            return filteredEvents;
        }
    }
}

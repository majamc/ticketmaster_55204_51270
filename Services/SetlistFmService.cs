using ConcertTracker.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ConcertTracker.Services
{
    /// <summary>
    /// Serwis do komunikacji z API Setlist.fm w celu pobierania najczęściej granych piosenek artysty.
    /// </summary>
    public class SetlistFmService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.setlist.fm/rest/1.0/search/setlists";

        // Klucz API do autoryzacji zapytań do Setlist.fm
        private const string ApiKey = "Kcerh64K2jVAkwfv9TGfSGPWf94BiJrqt86O";

        /// <summary>
        /// Konstruktor serwisu – ustawia nagłówki HTTP wymagane przez API Setlist.fm.
        /// </summary>
        public SetlistFmService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // Wyczyść istniejące nagłówki i ustaw wymagane do komunikacji z API
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("x-api-key", ApiKey); // Klucz API
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json"); // Oczekiwany format odpowiedzi
        }

        /// <summary>
        /// Pobiera listę 3 najczęściej granych piosenek danego artysty według danych z Setlist.fm.
        /// </summary>
        /// <param name="artistName">Nazwa artysty do wyszukania.</param>
        /// <returns>Lista nazw 3 najczęściej występujących piosenek.</returns>
        public async Task<List<string>> GetTopSongsAsync(string artistName)
        {
            // Przygotowanie URL z zakodowaną nazwą artysty (dla bezpieczeństwa URL)
            var url = $"{BaseUrl}?artistName={Uri.EscapeDataString(artistName)}&p=1";

            // Wysłanie GET do API Setlist.fm
            var response = await _httpClient.GetAsync(url);

            // Jeżeli odpowiedź nie powiodła się – zwróć pustą listę
            if (!response.IsSuccessStatusCode)
                return new List<string>();

            // Odczytaj treść odpowiedzi jako string
            var content = await response.Content.ReadAsStringAsync();

            // Parsuj odpowiedź JSON do obiektu JObject
            var data = JObject.Parse(content);

            // Słownik do zliczania wystąpień każdej piosenki
            var songCounts = new Dictionary<string, int>();

            // Pobierz tablicę setlist z odpowiedzi
            var setlists = data["setlist"];
            if (setlists == null) return new List<string>();

            // Przejdź przez każdą setlistę
            foreach (var setlist in setlists)
            {
                var sets = setlist["sets"]?["set"];
                if (sets == null) continue;

                foreach (var set in sets)
                {
                    var songs = set["song"];
                    if (songs == null) continue;

                    // Iteruj przez piosenki i zliczaj ich wystąpienia
                    foreach (var song in songs)
                    {
                        string name = song["name"]?.ToString() ?? "";
                        if (string.IsNullOrWhiteSpace(name)) continue;

                        if (songCounts.ContainsKey(name))
                            songCounts[name]++;
                        else
                            songCounts[name] = 1;
                    }
                }
            }

            // Zwróć 3 najczęściej grane piosenki w kolejności malejącej liczby wystąpień
            return songCounts
                .OrderByDescending(s => s.Value)
                .Take(3)
                .Select(s => s.Key)
                .ToList();
        }
    }
}

using ConcertTracker.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ConcertTracker.Services
{
    /// <summary>
    /// Serwis do komunikacji z API Setlist.fm w celu pobierania najcz??ciej granych piosenek artysty.
    /// </summary>
    public class SetlistFmService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.setlist.fm/rest/1.0/search/setlists";

        // Klucz API do autoryzacji zapyta? do Setlist.fm
        private const string ApiKey = "Kcerh64K2jVAkwfv9TGfSGPWf94BiJrqt86O";

        /// <summary>
        /// Konstruktor serwisu – ustawia nag?ówki HTTP wymagane przez API Setlist.fm.
        /// </summary>
        public SetlistFmService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // Wyczy?? istniej?ce nag?ówki i ustaw wymagane do komunikacji z API
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("x-api-key", ApiKey); // Klucz API
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json"); // Oczekiwany format odpowiedzi
        }

        /// <summary>
        /// Pobiera list? 3 najcz??ciej granych piosenek danego artysty wed?ug danych z Setlist.fm.
        /// </summary>
        /// <param name="artistName">Nazwa artysty do wyszukania.</param>
        /// <returns>Lista nazw 3 najcz??ciej wyst?puj?cych piosenek.</returns>
        public async Task<List<string>> GetTopSongsAsync(string artistName)
        {
            // Przygotowanie URL z zakodowan? nazw? artysty (dla bezpiecze?stwa URL)
            var url = $"{BaseUrl}?artistName={Uri.EscapeDataString(artistName)}&p=1";

            // Wys?anie GET do API Setlist.fm
            var response = await _httpClient.GetAsync(url);

            // Je?eli odpowied? nie powiod?a si? – zwró? pust? list?
            if (!response.IsSuccessStatusCode)
                return new List<string>();

            // Odczytaj tre?? odpowiedzi jako string
            var content = await response.Content.ReadAsStringAsync();

            // Parsuj odpowied? JSON do obiektu JObject
            var data = JObject.Parse(content);

            // S?ownik do zliczania wyst?pie? ka?dej piosenki
            var songCounts = new Dictionary<string, int>();

            // Pobierz tablic? setlist z odpowiedzi
            var setlists = data["setlist"];
            if (setlists == null) return new List<string>();

            // Przejd? przez ka?d? setlist?
            foreach (var setlist in setlists)
            {
                var sets = setlist["sets"]?["set"];
                if (sets == null) continue;

                foreach (var set in sets)
                {
                    var songs = set["song"];
                    if (songs == null) continue;

                    // Iteruj przez piosenki i zliczaj ich wyst?pienia
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
            // Zwró? 3 najcz??ciej grane piosenki w kolejno?ci malej?cej liczby wyst?pie?
            return songCounts
                .OrderByDescending(s => s.Value)
                .Take(3)
                .Select(s => s.Key)
                .ToList();
        }
    }
}

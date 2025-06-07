using ConcertTracker.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ConcertTracker.Services
{
    //komunikacja z API Setlist.fm w celu pobierania najczesciej granych piosenek artysty
    public class SetlistFmService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.setlist.fm/rest/1.0/search/setlists";

        //klucz API do autoryzacji zapytan
        private const string ApiKey = "Kcerh64K2jVAkwfv9TGfSGPWf94BiJrqt86O";

        //konstruktor serwisu – ustawia naglowki HTTP wymagane przez API Setlist.fm
        public SetlistFmService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            //czysci istniejace naglowki i ustawia wymagane do komunikacji z API
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("x-api-key", ApiKey); //klucz API
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json"); //oczekiwany format odpowiedzi
        }

        ///pobiera liste 3 najczesciej granych piosenek danego artysty wdg danych z Setlist.fm
        public async Task<List<string>> GetTopSongsAsync(string artistName)
        {
            var url = $"{BaseUrl}?artistName={Uri.EscapeDataString(artistName)}&p=1"; //URL z zakodowana nazwa artysty
            var response = await _httpClient.GetAsync(url); //GET do API Setlist.fm

            //pusta lista jesli nie powiodlo sie
            if (!response.IsSuccessStatusCode)
                return new List<string>();

            var content = await response.Content.ReadAsStringAsync(); //odczytaj tresc odpowiedzi jako string
            var data = JObject.Parse(content); //parsowanie odp
            var songCounts = new Dictionary<string, int>(); //zliczanie wystapien kazdej piosenki

            //pobranie tablicy setlist z odpowiedzi
            var setlists = data["setlist"];
            if (setlists == null) return new List<string>();

            //przechdzenie przez setliste
            foreach (var setlist in setlists)
            {
                var sets = setlist["sets"]?["set"];
                if (sets == null) continue;

                foreach (var set in sets)
                {
                    var songs = set["song"];
                    if (songs == null) continue;

                    //iteruj przez piosenki i zliczaj ich wystapienia
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
            //zwracane 3 najczesciej grane piosenki
            return songCounts
                .OrderByDescending(s => s.Value)
                .Take(3)
                .Select(s => s.Key)
                .ToList();
        }
    }
}

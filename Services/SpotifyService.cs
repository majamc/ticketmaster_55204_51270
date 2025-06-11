using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ConcertTracker.Services
{
    public class SpotifyService
    {
        private readonly HttpClient _httpClient;
        private const string ClientId = "837dadea6cad438290000cc2567b7950";
        private const string ClientSecret = "ac3e757fcc8345fab539369478ad0cf0";
        private string _accessToken = "";

        public SpotifyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var auth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            });

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Token error: {response.StatusCode} - {content}");

            var token = JObject.Parse(content)["access_token"]?.ToString();
            return token;
        }

        private async Task EnsureAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                _accessToken = await GetAccessTokenAsync();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }
        }

        public async Task<List<string>> GetTopTracksAsync(string artistName)
        {
            await EnsureAccessTokenAsync();
            if (string.IsNullOrEmpty(_accessToken)) return new List<string>();

            // Wyszukujemy artystê po nazwie
            var searchUrl = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(artistName)}&type=artist&limit=1";
            var searchResponse = await _httpClient.GetAsync(searchUrl);
            var searchContent = await searchResponse.Content.ReadAsStringAsync();

            if (!searchResponse.IsSuccessStatusCode)
                throw new Exception($"Artist search error: {searchResponse.StatusCode} - {searchContent}");

            var searchData = JObject.Parse(searchContent);
            var artist = searchData["artists"]?["items"]?.FirstOrDefault();
            if (artist == null) return new List<string>();

            var artistId = artist["id"]?.ToString();
            if (string.IsNullOrEmpty(artistId)) return new List<string>();

            // Pobieramy top tracki artysty
            var topTracksUrl = $"https://api.spotify.com/v1/artists/{artistId}/top-tracks";
            var topTracksResponse = await _httpClient.GetAsync(topTracksUrl);
            var topTracksContent = await topTracksResponse.Content.ReadAsStringAsync();

            if (!topTracksResponse.IsSuccessStatusCode)
                throw new Exception($"Top tracks error: {topTracksResponse.StatusCode} - {topTracksContent}");

            var topTracksData = JObject.Parse(topTracksContent);
            var tracks = topTracksData["tracks"];

            if (tracks == null) return new List<string>();

            return tracks
                .Take(5)
                .Select(t => t["name"]?.ToString() ?? "")
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList();
        }
    }
}

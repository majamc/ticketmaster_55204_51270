using ConcertTracker.Models.TicketmasterAPI.Models;

namespace ConcertTracker.Models
{
    public class EventWithSongsResponse
    {
        public string Name { get; set; } = string.Empty; // Nazwa wydarzenia / artysty
        public string Date { get; set; } = string.Empty;
        public string Venue { get; set; } = "Nieznana lokalizacja";
        public List<string> TopSongs { get; set; } = new(); // Top 3 piosenki
    }
}

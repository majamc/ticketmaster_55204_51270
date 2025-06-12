using ConcertTracker.Models.TicketmasterAPI.Models;

namespace ConcertTracker.Models
{
    public class EventWithSongsResponse
    {
        public string Name { get; set; } = string.Empty; // Nazwa wydarzenia / artysty
        public string Date { get; set; } = string.Empty;
        public string Venue { get; set; } = "Venue unknown";
        public List<string> TopSongs { get; set; } = new(); // Top 5 piosenki ze spotify
        public string ArtistImageUrl { get; set; } = string.Empty; //zdjecie artysty
    }
}
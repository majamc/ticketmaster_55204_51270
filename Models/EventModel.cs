namespace ConcertTracker.Models
{
    using Newtonsoft.Json;

    namespace TicketmasterAPI.Models
    {
        /// <summary>
        /// Reprezentuje wydarzenie (np. koncert) w systemie Ticketmaster.
        /// test
        /// </summary>
        public class EventModel
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty; // Nazwa zespołu / wydarzenia

            public string Date { get; set; } = string.Empty; // Data koncertu

            public string Venue { get; set; } = "Nieznana lokalizacja"; // Lokalizacja koncertu
        }

        /// <summary>
        /// Reprezentuje datę wydarzenia.
        /// </summary>
        public class EventDate
        {
            [JsonProperty("start")]
            public EventStart Start { get; set; } = new EventStart();
        }

        /// <summary>
        /// Reprezentuje szczegóły rozpoczęcia wydarzenia.
        /// </summary>
        public class EventStart
        {
            [JsonProperty("localDate")]
            public string LocalDate { get; set; } = string.Empty;
        }

        /// <summary>
        /// Reprezentuje osadzone informacje w odpowiedzi z API, takie jak informacje o miejscach.
        /// </summary>
        public class EventEmbedded
        {
            [JsonProperty("venues")]
            public List<Venue> Venues { get; set; } = new List<Venue>();
        }

        /// <summary>
        /// Reprezentuje miejsce (venue), w którym odbywa się wydarzenie.
        /// </summary>
        public class Venue
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;
        }

    }

}

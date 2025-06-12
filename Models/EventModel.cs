namespace ConcertTracker.Models
{
    using Newtonsoft.Json;

    namespace TicketmasterAPI.Models
    {
        public class EventModel //glowne dane koncertu
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty; //nazwa zespolu/wydarzenia
            public string Date { get; set; } = string.Empty; //data
            public string Venue { get; set; } = "Venue unknown"; //lokalizacja
        }

        public class EventDate //start wydarzenia
        {
            [JsonProperty("start")]
            public EventStart Start { get; set; } = new EventStart();
        }

        public class EventStart //data rozpoczecia
        {
            [JsonProperty("localDate")]
            public string LocalDate { get; set; } = string.Empty;
        }

        public class EventEmbedded //lista miejsc
        {
            [JsonProperty("venues")]
            public List<Venue> Venues { get; set; } = new List<Venue>();
        }

        public class Venue //lokalizacja wydarzenia
        {
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;
        }

    }

}

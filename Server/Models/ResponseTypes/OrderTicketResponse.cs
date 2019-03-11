using System;
using Newtonsoft.Json;

namespace TheVoid.Models.ResponseTypes{   
    public class OrderTicketModel{
        [JsonProperty("OrderId")]
        public int OrderId { get; set; }
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }
        [JsonProperty("LastName")]
        public string LastName { get; set; }
        [JsonProperty("TicketId")]
        public int TicketId { get; set; }
        [JsonProperty("TicketNumber")]
        public string TicketNumber { get; set; }
        [JsonProperty("EventDate")]
        public DateTime EventDate { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CosmosDB_CustomerData.Data.Entities
{
    public class Customer
    {
        [JsonProperty("id")]
        public string id { get; set; } // Still required for Cosmos DB, will be set in code

        [Required] public string Name { get; set; }
        public string Title { get; set; }

        [Required] public string Phone { get; set; }

        [Required, EmailAddress] public string Email { get; set; }

        [Required] public string Address { get; set; }

        [Required] public Salesperson Salesperson { get; set; }
    }
}

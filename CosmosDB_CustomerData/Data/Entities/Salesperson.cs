using System.ComponentModel.DataAnnotations;

namespace CosmosDB_CustomerData.Data.Entities
{
    public class Salesperson
    {
        [Required] public string Name { get; set; }
        [Required] public string Phone { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
    }
}

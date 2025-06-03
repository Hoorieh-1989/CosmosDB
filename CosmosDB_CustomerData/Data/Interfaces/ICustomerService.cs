using CosmosDB_CustomerData.Data.Entities;

namespace CosmosDB_CustomerData.Data.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task<IEnumerable<Customer>> SearchAsync(string? name, string? salespersonName);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(string id, Customer customer);
        Task DeleteCustomerAsync(string id);
       
    }
}

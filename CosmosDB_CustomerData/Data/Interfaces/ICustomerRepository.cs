using CosmosDB_CustomerData.Data.Entities;
using Microsoft.Azure.Cosmos;

namespace CosmosDB_CustomerData.Data.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task<IEnumerable<Customer>> SearchAsync(string? name, string? salespersonName);
        Task AddAsync(Customer customer);
        Task UpdateAsync(string id, Customer customer);
        Task DeleteAsync(string id);
        
    }
}

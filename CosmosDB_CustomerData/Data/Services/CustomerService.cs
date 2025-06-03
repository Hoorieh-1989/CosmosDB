using CosmosDB_CustomerData.Data.Entities;
using CosmosDB_CustomerData.Data.Interfaces;

namespace CosmosDB_CustomerData.Data.Services
{
    public class CustomerService: ICustomerService
    {
        private readonly ICustomerRepository _repo;

        public CustomerService(ICustomerRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Customer>> GetCustomersAsync() => _repo.GetAllAsync();

        public Task<IEnumerable<Customer>> SearchAsync(string? name, string? salespersonName) =>
            _repo.SearchAsync(name, salespersonName);
       
        public Task AddCustomerAsync(Customer customer) => _repo.AddAsync(customer);

        public Task UpdateCustomerAsync(string id, Customer customer) => _repo.UpdateAsync(id, customer);

        public Task DeleteCustomerAsync(string id) => _repo.DeleteAsync(id);
    }
}

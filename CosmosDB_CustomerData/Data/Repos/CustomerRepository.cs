using CosmosDB_CustomerData.Data.Entities;
using CosmosDB_CustomerData.Data.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CosmosDB_CustomerData.Data.Repos
{
    public class CustomerRepository:ICustomerRepository
    {
        private readonly Container _container;

        public CustomerRepository(CosmosClient client, IConfiguration config)
        {
            _container = client.GetContainer(config["CosmosDB:DatabaseId"], config["CosmosDb:ContainerId"]);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var query = _container.GetItemLinqQueryable<Customer>(true)
                .ToFeedIterator();

            var results = new List<Customer>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<Customer?> GetByIdAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Customer>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string? name, string? salespersonName)
        {
            var queryText = "SELECT * FROM c WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(name))
               
            queryText += $" AND CONTAINS(c.Name, '{name}')";

            if (!string.IsNullOrWhiteSpace(salespersonName))
              
                queryText += $" AND CONTAINS(c.Salesperson.Name, '{salespersonName}')";

            var query = _container.GetItemQueryIterator<Customer>(new QueryDefinition(queryText));
            var results = new List<Customer>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
        
        public async Task AddAsync(Customer customer)
        {
            await _container.CreateItemAsync(customer, new PartitionKey(customer.id));
        }

        public async Task UpdateAsync(string id, Customer customer)
        {
            customer.id = id; // tvinga objektet att använda id från URL
            await _container.UpsertItemAsync(customer, new PartitionKey(customer.id));
        }


      

        public async Task DeleteAsync(string id)
        {
            await _container.DeleteItemAsync<Customer>(id, new PartitionKey(id));
        }
    }
}


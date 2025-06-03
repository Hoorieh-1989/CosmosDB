using CosmosDB_CustomerData.Data.Entities;
using CosmosDB_CustomerData.Data.Interfaces;

namespace CosmosDB_CustomerData.Endpoints
{
    public static class CustomerEndpoints
    {
        public static void MapCustomerEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/customers", async (ICustomerService service) =>
                Results.Ok(await service.GetCustomersAsync()));

            app.MapGet("/customers/search", async (string? name, string? salesperson, ICustomerService service) =>
                Results.Ok(await service.SearchAsync(name, salesperson)));

           
            app.MapPost("/customers", async (Customer customer, ICustomerService service) =>
            {
                customer.id = Guid.NewGuid().ToString(); 
                await service.AddCustomerAsync(customer);
                return Results.Created($"/customers/{customer.id}", customer);
            });

            app.MapPut("/customers/{id}", async (string id, Customer customer, ICustomerService service) =>
            {
                await service.UpdateCustomerAsync(id, customer);
                return Results.Ok(customer);
            });

            app.MapDelete("/customers/{id}", async (string id, ICustomerService service) =>
            {
                await service.DeleteCustomerAsync(id);
                return Results.NoContent();
            });
        }
    }
}

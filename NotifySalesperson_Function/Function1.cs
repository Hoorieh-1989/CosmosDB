using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CosmosDB_CustomerData.Data.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NotifySalespersonFunction
{
    public class Function1
    {
        private readonly ILogger _log;
        private readonly string _sendGridApiKey;

        public Function1(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<Function1>();

            _sendGridApiKey = Environment.GetEnvironmentVariable("EmailApi");
            if (string.IsNullOrWhiteSpace(_sendGridApiKey))
            {
                _log.LogError("SendGridApiKey miljövariabeln saknas eller är tom!");
                throw new InvalidOperationException("SendGridApiKey måste finnas som miljövariabel.");
            }
        }

        [Function("NotifySalesperson")]
        public async Task Run(
            [CosmosDBTrigger(
                databaseName: "CosmosDB",
                containerName: "Customers",
                Connection = "CosmosDBConnection",
                LeaseContainerName = "leases",
                CreateLeaseContainerIfNotExists = true)]
            IReadOnlyList<Customer> customers)
        {
            _log.LogInformation("NotifySalesperson function triggered.");

            if (customers == null || customers.Count == 0)
            {
                _log.LogInformation("Inga ändringar hittades i change feed.");
                return;
            }

            var client = new SendGridClient(_sendGridApiKey);

            foreach (var customer in customers)
            {
                try
                {
                    if (customer == null)
                    {
                        _log.LogWarning("Hittade null-kund i change feed, hoppar över.");
                        continue;
                    }

                    if (customer.Salesperson == null || string.IsNullOrWhiteSpace(customer.Salesperson.Email))
                    {
                        _log.LogWarning($"Saknar salespersons e-post för kund {customer.Name}, hoppar över.");
                        continue;
                    }

                    _log.LogInformation($"Förbereder e-post för kund: {customer.Name}");

                    var message = new SendGridMessage()
                    {
                        From = new EmailAddress("hoorieh.23@gmail.com", "hoorieh"),
                        Subject = $"New or Updated Customer: {customer.Name}",
                        HtmlContent = $@"
                            <strong>Customer Details:</strong><br/>
                            Name: {customer.Name}<br/>
                            Title: {customer.Title}<br/>
                            Phone: {customer.Phone}<br/>
                            Email: {customer.Email}<br/>
                            Address: {customer.Address}<br/>"
                    };

                    message.AddTo(new EmailAddress(customer.Salesperson.Email, customer.Salesperson.Name));

                    var response = await client.SendEmailAsync(message);

                    if (response.IsSuccessStatusCode)
                    {
                        _log.LogInformation($"E-post skickades framgångsrikt till {customer.Salesperson.Email}.");
                    }
                    else
                    {
                        _log.LogWarning($"Misslyckades skicka e-post till {customer.Salesperson.Email}. Statuskod: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"Fel vid försök att skicka e-post för kund {customer?.Name}");
                }
            }
        }
    }
}


using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using CosmosDB_CustomerData.Data.Entities;

namespace NotifySalespersonFunction
{
    public class Function1
    {
        private readonly string _sendGridApiKey;

        public Function1(IConfiguration config)
        {
            _sendGridApiKey = config["SendGridApiKey"];
        }

        [FunctionName("NotifySalesperson")]
        public async Task Run(
            [CosmosDBTrigger(
                databaseName: "CustomerSalesDB",
                containerName: "Customers",
                Connection = "CustomerData",
                LeaseContainerName = "leases",
                CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Customer> input,
            ILogger log)
        {
            if (input == null || input.Count == 0)
            {
                log.LogInformation("No changes detected in Cosmos DB.");
                return;
            }

            var client = new SendGridClient(_sendGridApiKey);

            foreach (var customer in input)
            {
                if (customer.Salesperson == null || string.IsNullOrWhiteSpace(customer.Salesperson.Email))
                {
                    log.LogWarning($"Customer {customer.Id} skipped due to missing salesperson email.");
                    continue;
                }

                var toEmail = customer.Salesperson.Email;
                var subject = $"New/Updated Customer: {customer.Name}";
                var body = $@"
                    <h3>Customer Details</h3>
                    <ul>
                        <li><strong>Name:</strong> {WebUtility.HtmlEncode(customer.Name)}</li>
                        <li><strong>Title:</strong> {WebUtility.HtmlEncode(customer.Title)}</li>
                        <li><strong>Phone:</strong> {WebUtility.HtmlEncode(customer.Phone)}</li>
                        <li><strong>Email:</strong> {WebUtility.HtmlEncode(customer.Email)}</li>
                        <li><strong>Address:</strong> {WebUtility.HtmlEncode(customer.Address)}</li>
                    </ul>
                    <p>Regards,<br/>Your CRM System</p>
                ";

                var message = MailHelper.CreateSingleEmail(
                    from: new EmailAddress("haritha.1015@gmail.com"),
                    to: new EmailAddress(customer.Salesperson.Email),
                    subject,
                    plainTextContent: subject,
                    htmlContent: body
                );

                var response = await client.SendEmailAsync(message);

                if ((int)response.StatusCode >= 400)
                {
                    log.LogError($"Failed to send email to {toEmail}. Status: {response.StatusCode}");
                }
                else
                {
                    log.LogInformation($"Email sent to {toEmail} with status {response.StatusCode}");
                }
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stripe;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace Task6.Controllers
{
    public class StripeWebHook : Controller
    {
        [Route("api/[controller]")]
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    // Then define and call a method to handle the successful payment intent.
                    // handlePaymentIntentSucceeded(paymentIntent)
                }
                else if (stripeEvent.Type == Events.PaymentMethodAttached)
                {
                    var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                    // Then define and call a method to handle the successful attachment of a PaymentMethod.
                    // handlePaymentMethodAttached(paymentMethod);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
                {
                    var customerSubscriptionCreated = stripeEvent.Data.Object as Subscription;
                    saveSubscriptionTransaction(customerSubscriptionCreated);

                }
                // ... handle other event types
                else
                {
                    // Unexpected event type
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

        public static void saveSubscriptionTransaction(Subscription subscription)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = Credentials.databaseDataSource;
                builder.UserID = Credentials.databaseUserId;
                builder.Password = Credentials.databasePassword;
                builder.InitialCatalog = Credentials.databaseInitialCatalogue;

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(null, connection);

                    // Create and prepare an SQL statement.
                    command.CommandText =
                        "INSERT INTO SUBSCRIPTIONS (customerId, subscriptionId, transactionType) " +
                        "VALUES (@customerId, @subscriptionId, @transactionType)";
                    SqlParameter customerIdParam = new SqlParameter("@customerId", SqlDbType.Text, 50);
                    SqlParameter subscriptionIdParam =
                        new SqlParameter("@subscriptionId", SqlDbType.Text, 50);
                    SqlParameter transactionTypeParam = new SqlParameter("@transactionType", SqlDbType.Text, 50);
                    customerIdParam.Value = subscription.CustomerId;
                    subscriptionIdParam.Value = subscription.Id;
                    transactionTypeParam.Value = "Subscription Created";
                    command.Parameters.Add(customerIdParam);
                    command.Parameters.Add(subscriptionIdParam);
                    command.Parameters.Add(transactionTypeParam);

                    // Call Prepare after setting the Commandtext and Parameters.
                    command.Prepare();
                    var affected = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

}

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
                if (stripeEvent.Type == Events.CustomerCreated)
                {
                    var customerCreated = stripeEvent.Data.Object as Customer;
                    saveCustomer(customerCreated);
                }
                else if (stripeEvent.Type == Events.PaymentMethodAttached)
                {
                    var paymentMethodAttatched = stripeEvent.Data.Object as PaymentMethod;
                    savePaymentMethod(paymentMethodAttatched);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
                {
                    var customerSubscriptionCreated = stripeEvent.Data.Object as Subscription;
                    saveSubscription(customerSubscriptionCreated);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                    var customerSubscriptionCreated = stripeEvent.Data.Object as Subscription;
                    updateSubscription(customerSubscriptionCreated);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                {
                    var customerSubscriptionCreated = stripeEvent.Data.Object as Subscription;
                    updateSubscription(customerSubscriptionCreated);
                }
                else if (stripeEvent.Type == Events.PaymentIntentCreated)
                {
                    var paymentIntentCreated = stripeEvent.Data.Object as PaymentIntent;
                    saveTransaction(paymentIntentCreated);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntentCreated = stripeEvent.Data.Object as PaymentIntent;
                    saveTransaction(paymentIntentCreated);
                }
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

        public static void saveCustomer(Customer customer)
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
                        "INSERT INTO CUSTOMER (stripe_id, name, email, addressLine1, addressLine2, city, postcode, country) " +
                        "VALUES (@stripe_id, @name, @email, @addressLine1, @addressLine2, @city, @postcode, @country)";

                    SqlParameter stripeIdParam = new SqlParameter("@stripe_id", SqlDbType.Text, 64);
                    SqlParameter nameParam = new SqlParameter("@name", SqlDbType.Text, 64);
                    SqlParameter emailParam = new SqlParameter("@email", SqlDbType.Text, 128);
                    SqlParameter addressLine1Param = new SqlParameter("@addressLine1", SqlDbType.Text, 128);
                    SqlParameter addressLine2Param = new SqlParameter("@addressLine2", SqlDbType.Text, 128);
                    SqlParameter cityParam = new SqlParameter("@city", SqlDbType.Text, 64);
                    SqlParameter postcodeParam = new SqlParameter("@postcode", SqlDbType.Text, 32);
                    SqlParameter countryParam = new SqlParameter("@country", SqlDbType.Text, 64);

                    stripeIdParam.Value = customer.Id;
                    nameParam.Value = customer.Name;
                    emailParam.Value = customer.Email;
                    addressLine1Param.Value = customer.Address.Line1;
                    addressLine2Param.Value = customer.Address.Line2;
                    cityParam.Value = customer.Address.City;
                    postcodeParam.Value = customer.Address.PostalCode;
                    countryParam.Value = customer.Address.Country;

                    command.Parameters.Add(stripeIdParam);
                    command.Parameters.Add(nameParam);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(addressLine1Param);
                    command.Parameters.Add(addressLine2Param);
                    command.Parameters.Add(cityParam);
                    command.Parameters.Add(postcodeParam);
                    command.Parameters.Add(countryParam);

                    // Call Prepare after setting the Commandtext and Parameters.
                    command.Prepare();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void savePaymentMethod(PaymentMethod paymentMethod)
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
                        "INSERT INTO PAYMENTMETHODS (paymentmethod_id, customer_stripe_id) " +
                        "VALUES (@paymentmethod_id, @customer_stripe_id)";

                    SqlParameter paymentmethodIdParam = new SqlParameter("@paymentmethod_id", SqlDbType.Text, 64);
                    SqlParameter customerStripeIdParam = new SqlParameter("@customer_stripe_id", SqlDbType.Text, 64);

                    paymentmethodIdParam.Value = paymentMethod.Id;
                    customerStripeIdParam.Value = paymentMethod.CustomerId;

                    command.Parameters.Add(paymentmethodIdParam);
                    command.Parameters.Add(customerStripeIdParam);

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

        public static void saveSubscription(Subscription subscription)
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
                        "INSERT INTO SUBSCRIPTIONS (subscription_id, customer_stripe_id, status) " +
                        "VALUES (@subscription_id, @customer_stripe_id, @status)";
                    SqlParameter subscriptionIdParam = new SqlParameter("@subscription_id", SqlDbType.Text, 64);
                    SqlParameter customerStripeIdParam = new SqlParameter("@customer_stripe_id", SqlDbType.Text, 64);
                    SqlParameter statusParam = new SqlParameter("@status", SqlDbType.Text, 64);

                    subscriptionIdParam.Value = subscription.Id;
                    customerStripeIdParam.Value = subscription.CustomerId;
                    statusParam.Value = subscription.Status;

                    command.Parameters.Add(subscriptionIdParam);
                    command.Parameters.Add(customerStripeIdParam);
                    command.Parameters.Add(statusParam);

                    // Call Prepare after setting the Commandtext and Parameters.
                    command.Prepare();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void updateSubscription(Subscription subscription)
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
                        "UPDATE SUBSCRIPTIONS SET status = @status where subscription_id = @subscription_id";

                    SqlParameter subscriptionIdParam = new SqlParameter("@subscription_id", SqlDbType.VarChar, 64);
                    SqlParameter statusParam = new SqlParameter("@status", SqlDbType.VarChar, 64);

                    subscriptionIdParam.Value = subscription.Id;
                    statusParam.Value = subscription.Status;

                    command.Parameters.Add(subscriptionIdParam);
                    command.Parameters.Add(statusParam);

                    // Call Prepare after setting the Commandtext and Parameters.
                    command.Prepare();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void saveTransaction(PaymentIntent paymentIntent)
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
                        "INSERT INTO TRANSACTION_LOG (customer_stripe_id, paymentintent_id, event_date, event_result) " +
                        "VALUES (@customer_stripe_id, @paymentintent_id, @event_date, @event_result)";

                    SqlParameter customerStripeIdParam = new SqlParameter("@customer_stripe_id", SqlDbType.Text, 64);
                    SqlParameter paymentIntentIdParam = new SqlParameter("@paymentintent_id", SqlDbType.Text, 64);
                    SqlParameter eventDateParam = new SqlParameter("@event_date", SqlDbType.DateTime);
                    SqlParameter eventResultParam = new SqlParameter("@event_result", SqlDbType.Text, 64);

                    customerStripeIdParam.Value = paymentIntent.CustomerId;
                    paymentIntentIdParam.Value = paymentIntent.Id;
                    eventDateParam.Value = paymentIntent.Created;
                    eventResultParam.Value = paymentIntent.Status;

                    command.Parameters.Add(customerStripeIdParam);
                    command.Parameters.Add(paymentIntentIdParam);
                    command.Parameters.Add(eventDateParam);
                    command.Parameters.Add(eventResultParam);

                    // Call Prepare after setting the Commandtext and Parameters.
                    command.Prepare();
                    command.ExecuteNonQuery();
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

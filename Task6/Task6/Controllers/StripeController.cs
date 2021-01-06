using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task6.Models;
using Stripe;
using System.Web.Http;
using Stripe.BillingPortal;

namespace Task6.Controllers
{
    public class StripeController : Controller
    {
        private static string ApiKey = Credentials.stripePrivateKey;
        public IActionResult Stripe()
        {
            return View();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Customer(StripeCharge model)
        {
            if (!ModelState.IsValid)
            {
                return View("Stripe");
            }

            var customerId = await CreateCustomer(model);
            var cardId = await CreatePaymentCard(model, customerId);
            var subscriptionId = await ProcessSubscription(customerId);
            var sessionUrl = await CustomerPortal(customerId);


            //create customer -> add card -> subscribe -> show portal

            return Redirect(sessionUrl);
            //return View("Index");
        }

        private static async Task<string> CreateCustomer(StripeCharge model)
        {
            return await Task.Run(() =>
            {
                AddressOptions address = new AddressOptions();
                address.State = model.AddressCountry;
                address.Line1 = model.AddressLine1;
                address.Line2 = model.AddressLine2;
                address.PostalCode = model.AddressPostalcode;
                address.City = model.AddressCity;
                address.Country = model.AddressCountry;

                StripeConfiguration.ApiKey = ApiKey;

                var options = new CustomerCreateOptions
                {
                    Name = model.CardHolderName,
                    Email = model.Email,
                    Address = address,
                };
                var service = new CustomerService();
                var customer = service.Create(options);
                return customer.Id;
            });
        }

        private static async Task<string> CreatePaymentCard(StripeCharge model, string customerId)
        {
            return await Task.Run(() =>
            {
                StripeConfiguration.ApiKey = ApiKey;

                var options = new CardCreateOptions
                {
                    Source = model.Token,
                };
                var service = new CardService();
                var card = service.Create(customerId, options);
                return card.Id;
            });
        }

        private static async Task<string> ProcessSubscription(string customerId)
        {
            return await Task.Run(() =>
            {
                StripeConfiguration.ApiKey = ApiKey;

                var options = new SubscriptionCreateOptions
                {
                    Customer = customerId,
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Price = "price_1I5WIVKZfnMuvFdbiyfwpkB0",
                        },
                    },
                    //TrialEnd = DateTimeOffset.FromUnixTimeSeconds(1609919880).UtcDateTime,
                };
                var service = new SubscriptionService();
                var subscription = service.Create(options);
                return subscription.Id;
            });
        }

        public static async Task<string> CustomerPortal(string customerId)
        {
            return await Task.Run(() =>
            {
                // Authenticate your user.
                StripeConfiguration.ApiKey = ApiKey;

                var options = new SessionCreateOptions
                {
                    Customer = customerId,
                    ReturnUrl = "https://localhost:44302/",
                };
                var service = new SessionService();
                var session = service.Create(options);
                return session.Url;
            });
        }
    }
}

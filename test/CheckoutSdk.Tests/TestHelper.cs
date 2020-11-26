using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.Common;
using Checkout.Payments;
using Checkout.Tokens;
using Checkout.Webhooks;
using Shouldly;

namespace Checkout.Tests
{
    public static class TestHelper
    {
        public static string CkoRequestId = "bcfe03bd-2a8a-4340-8ce5-214761fae065";
        public static string CkoVersion = "3.46.1";

        public static void DueDiligenceTests<TContent>(CheckoutHttpResponseMessage<TContent> checkoutHttpResponseMessage)
        {
            checkoutHttpResponseMessage.ShouldNotBeNull();
            checkoutHttpResponseMessage.CkoRequestId.ShouldNotBeNull();
            checkoutHttpResponseMessage.CkoVersion.ShouldNotBeNull();
        }

        public static PaymentRequest<CardSource> CreateCardPaymentRequest(long? amount = 100)
        {
            return new PaymentRequest<CardSource>(
                new CardSource(TestCardSource.Visa.Number, TestCardSource.Visa.ExpiryMonth, TestCardSource.Visa.ExpiryYear)
                {
                    Cvv = TestCardSource.Visa.Cvv
                },
                Currency.GBP,
                amount
            )
            {
                Capture = false,
                Customer = new Checkout.Payments.CustomerRequest() { Email = TestHelper.GenerateRandomEmail()},
                Reference = Guid.NewGuid().ToString()
            };
        }
        
        public static PaymentRequest<DlocalCardSource> CreateDlocalCardPaymentRequest(long? amount = 100)
        {
            var dlocalCardSource = new DlocalCardSource(TestCardSource.HiperCard.Number, TestCardSource.HiperCard.ExpiryMonth, TestCardSource.HiperCard.ExpiryYear, TestCardSource.HiperCard.Name)
            {
                Cvv = TestCardSource.HiperCard.Cvv
            };

            
            
            var dlocalCardPaymentRequest = new PaymentRequest<DlocalCardSource>(
                dlocalCardSource,
                Currency.BRL,
                amount
            )
            {
                Capture = false,
                Customer = new Checkout.Payments.CustomerRequest() { Email = TestHelper.GenerateRandomEmail() },
                Reference = Guid.NewGuid().ToString(),
                BillingDescriptor = new BillingDescriptor("billdescriptor", "gotham")
            };

            var dLocalProcessing = new
            {
                country = "BR", 
                payer = new
                {
                    document = "53033315550",
                    name = "Bill Gates",
                    email = "test@checkout.com"
                },
                installments = new { count = 4 }
            };


            dlocalCardPaymentRequest.Processing.Add("dlocal", dLocalProcessing);

            return dlocalCardPaymentRequest;
        }

        public static PaymentRequest<IRequestSource> CreateAlternativePaymentMethodRequest(IRequestSource alternativePaymentMethodRequestSource, string currency, long? amount = 100)
        {
            return new PaymentRequest<IRequestSource>(
                alternativePaymentMethodRequestSource,
                currency,
                amount
            )
            {
                Capture = false,
                Customer = new Checkout.Payments.CustomerRequest() { Email = TestHelper.GenerateRandomEmail() },
                Reference = Guid.NewGuid().ToString()
            };
        }

        public static CardTokenRequest CreateCardTokenRequest()
        {
            return new CardTokenRequest(TestCardSource.Visa.Number, TestCardSource.Visa.ExpiryMonth,
                TestCardSource.Visa.ExpiryYear)
            {
                Cvv = TestCardSource.Visa.Cvv
            };
        }

        public static PaymentRequest<CardSource> CreateChargebackCardPaymentRequest()
        {
            return new PaymentRequest<CardSource>(
                new CardSource(TestCardSource.Visa.Number, TestCardSource.Visa.ExpiryMonth, TestCardSource.Visa.ExpiryYear)
                {
                    Cvv = TestCardSource.Visa.Cvv
                },
                Currency.GBP,
                1040
            )
            {
                Capture = true,
                Customer = new Checkout.Payments.CustomerRequest() { Email = TestHelper.GenerateRandomEmail() },
                Reference = Guid.NewGuid().ToString()
            };
        }

        public static string GenerateRandomEmail()
        {
            return Guid.NewGuid().ToString("n") + "@checkout-sdk-net.com";
        }

        public static int PaymentDisputedVerificationInterval()
        {
            return 10000;
        }

        public static PaymentRequest<TokenSource> CreateTokenPaymentRequest(string token)
        {
            return new PaymentRequest<TokenSource>(new TokenSource(token),
                    Currency.GBP,
                    100)
                    {
                        Capture = false
                    };
        }

        public static IWebhook CreateWebhook()
        {
            return new Webhook()
            {
                Url = "https://www.checkout.com/net/sdk/tests",
                EventTypes = new List<string>
                {
                    "payment_pending",
                    "payment_captured"
                },
                Headers = new Dictionary<string, string>
                {
                    { "User-Agent", ".NET SDK Tests" }
                }
            };           
        }
    }
}

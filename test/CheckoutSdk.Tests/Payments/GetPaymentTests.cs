using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Checkout.Common;
using Checkout.Payments;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Checkout.Tests.Payments
{
    public class GetPaymentTests : IClassFixture<ApiTestFixture>
    {
        private readonly ICheckoutApi _api;

        public GetPaymentTests(ApiTestFixture fixture, ITestOutputHelper outputHelper)
        {
            fixture.CaptureLogsInTestOutput(outputHelper);
            _api = fixture.Api;
        }
        
        [Fact]
        public async Task CanGetNonThreeDsPayment()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            paymentRequest.PaymentType = PaymentType.Recurring;
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);
            var paymentDetails = await _api.Payments.GetPaymentDetails(paymentResponse.Content.Payment.Id);

            paymentDetails.ShouldNotBeNull();
            paymentDetails.Content.Id.ShouldBe(paymentResponse.Content.Payment.Id);
            paymentDetails.Content.Customer.ShouldNotBeNull();
            paymentDetails.Content.Customer.Id.ShouldBe(paymentResponse.Content.Payment.Customer.Id);
            paymentDetails.Content.Customer.Email.ShouldBe(paymentRequest.Customer.Email);
            paymentDetails.Content.Amount.ShouldBe(paymentResponse.Content.Payment.Amount);
            paymentDetails.Content.Currency.ShouldBe(paymentResponse.Content.Payment.Currency);
            paymentDetails.Content.PaymentType.ShouldBe(paymentRequest.PaymentType);
            paymentDetails.Content.BillingDescriptor.ShouldNotBeNull();
            paymentDetails.Content.Reference.ShouldNotBeNullOrWhiteSpace();
            paymentDetails.Content.Risk.ShouldNotBeNull();
            paymentDetails.Content.RequestedOn.ShouldBeGreaterThan(paymentResponse.Content.Payment.ProcessedOn.AddMinutes(-1));
            paymentDetails.Content.ThreeDS.ShouldBeNull();
            paymentDetails.Content.Links.ShouldNotBeNull();
            paymentDetails.Content.Links.ShouldNotBeEmpty();
            paymentDetails.Content.Status.ShouldBe(PaymentStatus.Authorized);
            paymentDetails.Content.Source.AsCard().ShouldNotBeNull();
            paymentDetails.Content.Approved.ShouldBeTrue();
        }

        [Fact]
        public async Task CanGetThreeDsPaymentBeforeAuth()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            paymentRequest.ThreeDS = true;
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);
            paymentResponse.Content.IsPending.ShouldBe(true);

            var paymentDetails = await _api.Payments.GetPaymentDetails(paymentResponse.Content.Pending.Id);

            paymentDetails.ShouldNotBeNull();
            paymentDetails.Content.Id.ShouldBe(paymentResponse.Content.Pending.Id);
            paymentDetails.Content.Customer.ShouldNotBeNull();
            paymentDetails.Content.Customer.Id.ShouldBe(paymentResponse.Content.Pending.Customer.Id);
            paymentDetails.Content.Customer.Email.ShouldBe(paymentRequest.Customer.Email);
            paymentDetails.Content.Amount.ShouldBe(paymentRequest.Amount);
            paymentDetails.Content.Currency.ShouldBe(paymentRequest.Currency);
            paymentDetails.Content.Reference.ShouldNotBeNullOrWhiteSpace();
            paymentDetails.Content.PaymentType.ShouldBe(PaymentType.Regular);
            paymentDetails.Content.Risk.ShouldNotBeNull();
            paymentDetails.Content.RequestedOn.ShouldBeGreaterThan(DateTime.MinValue);
            paymentDetails.Content.ThreeDS.ShouldNotBeNull();
            paymentDetails.Content.ThreeDS.Downgraded.ShouldBe(false);
            paymentDetails.Content.ThreeDS.Enrolled.ShouldNotBeNullOrEmpty();
            paymentDetails.Content.RequiresRedirect().ShouldBe(true);
            paymentDetails.Content.GetRedirectLink().ShouldNotBeNull();
            paymentDetails.Content.Links.ShouldNotBeNull();
            paymentDetails.Content.Links.ShouldNotBeEmpty();
            paymentDetails.Content.Status.ShouldBe(PaymentStatus.Pending);
            paymentDetails.Content.Source.AsCard().ShouldNotBeNull();
            paymentDetails.Content.Approved.ShouldBeFalse();
        }

        [Fact]
        public async Task CanGetPaymentMetadata()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            var metadata = new KeyValuePair<string, object>("test", "1234");
            paymentRequest.Metadata.Add(metadata.Key, metadata.Value);
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);

            var paymentDetails = await _api.Payments.GetPaymentDetails(paymentResponse.Content.Payment.Id);

            paymentDetails.Content.Metadata.ShouldNotBeNull();
            paymentDetails.Content.Metadata.ShouldNotBeEmpty();
            paymentDetails.Content.Metadata.ShouldHaveSingleItem();
            paymentDetails.Content.Metadata.ShouldContain(d => d.Key == metadata.Key && d.Value.Equals(metadata.Value));
        }

        [Fact]
        public async Task CanGetPaymentIp()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            paymentRequest.PaymentIp = "10.1.2.3";
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);

            var paymentDetails = await _api.Payments.GetPaymentDetails(paymentResponse.Content.Payment.Id);

            paymentDetails.Content.PaymentIp.ShouldBe(paymentRequest.PaymentIp);
        }

        [Fact]
        public async Task CanGetPaymentRecipient()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            paymentRequest.Recipient =
                new PaymentRecipient(new DateTime(1985, 05, 15), "4242424242", "W1T", lastName: "Wensle");
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);

            var paymentDetails = await _api.Payments.GetPaymentDetails(paymentResponse.Content.Payment.Id);

            paymentDetails.Content.Recipient.ShouldNotBeNull();
            paymentDetails.Content.Recipient.AccountNumber.ShouldBe(paymentRequest.Recipient.AccountNumber);
            paymentDetails.Content.Recipient.DateOfBirth.ShouldBe(paymentRequest.Recipient.DateOfBirth);
            paymentDetails.Content.Recipient.LastName.ShouldBe(paymentRequest.Recipient.LastName);
            paymentDetails.Content.Recipient.Zip.ShouldBe(paymentRequest.Recipient.Zip);
        }

        [Fact]
        public async Task CanGetPaymentShipping()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            paymentRequest.Shipping = new ShippingDetails()
            {
                Address = new Address() { AddressLine1 = "221B Baker Street", AddressLine2 = null, City = "London", Country = "UK", State = "n/a", Zip = "NW1 6XE" },
                Phone = new Phone() { CountryCode = "44", Number = "124312431243" }
            };
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);

            var paymentDetails = await _api.Payments.GetPaymentDetails(paymentResponse.Content.Payment.Id);

            paymentDetails.Content.Shipping.ShouldNotBeNull();
            paymentDetails.Content.Shipping.Address.ShouldNotBeNull();
            paymentDetails.Content.Shipping.Address.AddressLine1.ShouldBe(paymentRequest.Shipping.Address.AddressLine1);
            paymentDetails.Content.Shipping.Address.AddressLine2.ShouldBe(paymentRequest.Shipping.Address.AddressLine2);
            paymentDetails.Content.Shipping.Address.City.ShouldBe(paymentRequest.Shipping.Address.City);
            paymentDetails.Content.Shipping.Address.Country.ShouldBe(paymentRequest.Shipping.Address.Country);
            paymentDetails.Content.Shipping.Address.State.ShouldBe(paymentRequest.Shipping.Address.State);
            paymentDetails.Content.Shipping.Address.Zip.ShouldBe(paymentRequest.Shipping.Address.Zip);
            paymentDetails.Content.Shipping.Phone.CountryCode.ShouldBe(paymentRequest.Shipping.Phone.CountryCode);
            paymentDetails.Content.Shipping.Phone.Number.ShouldBe(paymentRequest.Shipping.Phone.Number);
        }

        [Fact]
        public async Task CanGetPaymentDescription()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            paymentRequest.Description = "Too descriptive";
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);

            var paymentDetails = await _api.Payments.GetPaymentDetails(paymentResponse.Content.Payment.Id);

            paymentDetails.Content.Description.ShouldBe(paymentRequest.Description);
        }

        [Fact]
        public async Task CanGetPaymentAction()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);

            var actionsResponse = await _api.Payments.GetPaymentActions(paymentResponse.Content.Payment.Id);

            actionsResponse.ShouldNotBeNull();
            actionsResponse.Content.ShouldHaveSingleItem();

            PaymentProcessed payment = paymentResponse.Content.Payment;
            PaymentAction paymentAction = actionsResponse.Content.SingleOrDefault();
            paymentAction.ShouldNotBeNull();
            paymentAction.Id.ShouldBe(payment.ActionId);
            paymentAction.ProcessedOn.ShouldBeGreaterThanOrEqualTo(payment.ProcessedOn);
            paymentAction.Approved.ShouldBeTrue();
            paymentAction.Approved.ShouldBe(payment.Approved);
            paymentAction.ResponseCode.ShouldBe(payment.ResponseCode);
            paymentAction.ResponseSummary.ShouldBe(payment.ResponseSummary);
            paymentAction.Reference.ShouldBe(payment.Reference);
            paymentAction.AuthCode.ShouldBe(payment.AuthCode);
            paymentAction.Type.ShouldBe(ActionType.Authorization);
            paymentAction.Links.ShouldNotBeNull();
            paymentAction.Processing.ShouldNotBeNull();
            paymentAction.Processing.AcquirerTransactionId.ShouldNotBeNullOrWhiteSpace();
            paymentAction.Processing.RetrievalReferenceNumber.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task CanGetMultiplePaymentActions()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);
            var captureRequest = new CaptureRequest
            {
                Reference = Guid.NewGuid().ToString()
            };
            var captureResponse = await _api.Payments.CaptureAPayment(paymentResponse.Content.Payment.Id, captureRequest);

            var actionsResponse = await _api.Payments.GetPaymentActions(paymentResponse.Content.Payment.Id);

            actionsResponse.ShouldNotBeNull();

            PaymentAction authorizationPaymentAction = actionsResponse.Content.SingleOrDefault(a => a.Type == ActionType.Authorization);
            authorizationPaymentAction.ShouldNotBeNull();
            authorizationPaymentAction.Id.ShouldBe(paymentResponse.Content.Payment.ActionId);

            PaymentAction capturePaymentAction = actionsResponse.Content.SingleOrDefault(a => a.Type == ActionType.Capture);
            capturePaymentAction.ShouldNotBeNull();
            capturePaymentAction.Id.ShouldBe(captureResponse.Content.ActionId);
            capturePaymentAction.Reference.ShouldBe(captureResponse.Content.Reference);
            capturePaymentAction.Links.ShouldNotBeNull();
            capturePaymentAction.Processing.AcquirerReferenceNumber.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task CanGetPaymentActionMetadata()
        {
            PaymentRequest<CardSource> paymentRequest = TestHelper.CreateCardPaymentRequest();
            var metadata = new KeyValuePair<string, object>("test", "1234");
            paymentRequest.Metadata.Add(metadata.Key, metadata.Value);
            var paymentResponse = await _api.Payments.RequestAPayment(paymentRequest);

            var actionsResponse = await _api.Payments.GetPaymentActions(paymentResponse.Content.Payment.Id);

            actionsResponse.ShouldNotBeNull();

            PaymentAction paymentAction = actionsResponse.Content.FirstOrDefault();
            paymentAction.ShouldNotBeNull();
            paymentAction.Metadata.ShouldNotBeNull();
            paymentAction.Metadata.ShouldNotBeEmpty();
            paymentAction.Metadata.ShouldHaveSingleItem();
            paymentAction.Metadata.ShouldContain(d => d.Key == metadata.Key && d.Value.Equals(metadata.Value));
        }
    }
}
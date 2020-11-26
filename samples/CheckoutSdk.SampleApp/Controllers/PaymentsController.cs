using Checkout.Common;
using Checkout.Payments;
using Checkout.SampleApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Checkout.SampleApp.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ICheckoutApi _checkoutApi;
        private readonly ISerializer _serializer;

        public PaymentsController(ICheckoutApi checkoutApi, ISerializer serializer)
        {
            _checkoutApi = checkoutApi ?? throw new ArgumentNullException(nameof(checkoutApi));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public IActionResult Index()
        {
            PaymentModel model = PrepareModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(PaymentModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    PrepareModel(model);
                    return View(nameof(Index), model);
                }

                if (string.IsNullOrWhiteSpace(model.CardToken))
                    throw new ArgumentException($"{nameof(model.CardToken)} is missing.", nameof(model));

                var source = new TokenSource(model.CardToken);
                var paymentRequest = new PaymentRequest<TokenSource>(source, model.Currency, model.Amount)
                {
                    Capture = model.Capture,
                    Reference = model.Reference,
                    ThreeDS = model.DoThreeDS,
                    SuccessUrl = BuildUrl(nameof(ThreeDSSuccess)),
                    FailureUrl = BuildUrl(nameof(ThreeDSFailure))
                };

                var response = await _checkoutApi.Payments.RequestAPayment(paymentRequest);

                if (response.Content.IsPending && response.Content.Pending.RequiresRedirect())
                {
                    return Redirect(response.Content.Pending.GetRedirectLink().Href);
                }

                StorePaymentInTempData(response.Content.Payment);

                if (response.Content.Payment.Approved)
                {
                    return RedirectToAction(nameof(NonThreeDSSuccess));
                }

                return RedirectToAction(nameof(NonThreeDSFailure));
            }
            catch (Exception ex)
            {
                return View(nameof(Error), new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier, Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult NonThreeDSSuccess() => GetPaymentFromTempData();

        [HttpGet]
        public IActionResult NonThreeDSFailure() => GetPaymentFromTempData();

        [HttpGet]
        public Task<IActionResult> ThreeDSSuccess([FromQuery(Name = "cko-session-id")] string ckoSessionId) 
            => GetThreeDsPaymentAsync(ckoSessionId);
        
        [HttpGet]
        public Task<IActionResult> ThreeDSFailure([FromQuery(Name = "cko-session-id")] string ckoSessionId) 
            => GetThreeDsPaymentAsync(ckoSessionId);

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void StorePaymentInTempData(PaymentProcessed payment)
        {
            TempData[nameof(PaymentProcessed)] = _serializer.Serialize(payment);
        }

        private async Task<IActionResult> GetThreeDsPaymentAsync(string sessionId)
        {
            var response = await _checkoutApi.Payments.GetPaymentDetails(sessionId);
            
            if (response.Content == null)
                return RedirectToAction(nameof(Index));

            return View(response.Content);
        }

        private ActionResult GetPaymentFromTempData()
        {
            if (TempData.TryGetValue(nameof(PaymentProcessed), out var serializedPayment))
            {
                var payment = _serializer.Deserialize(serializedPayment.ToString(), typeof(PaymentProcessed)) as PaymentProcessed;
                return View(payment);
            }

            return RedirectToAction(nameof(Index));
        }

        private PaymentModel PrepareModel(PaymentModel existingModel = null)
        {
            var model = existingModel ?? new PaymentModel();
            model.Currencies = new[]
            {
                new SelectListItem() {Value = Currency.USD, Text = Currency.USD},
                new SelectListItem() {Value = Currency.EUR, Text = Currency.EUR},
                new SelectListItem() {Value = Currency.GBP, Text = Currency.GBP}
            };
            return model;
        }

        private string BuildUrl(string actionName) 
            => Url.Action(actionName, "payments", null, Request.Scheme);
    }
}

using Checkout;
using Checkout.Common;
using Checkout.Payments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Checkout.SampleApp.Controllers;
using Checkout.SampleApp.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Xunit;

namespace Checkout.SampleApp.Tests
{
    public class PaymentsControllerTests
    {
        private readonly Mock<ICheckoutApi> _checkoutApi;
        private readonly Mock<IPaymentsClient> _paymentsClient;
        private readonly PaymentsController _controller;
        private readonly PaymentResponse _paymentsResponse;
        private readonly JsonSerializer _jsonSerializer = new JsonSerializer();

        public PaymentsControllerTests()
        {
            _checkoutApi = new Mock<ICheckoutApi>();

            _paymentsResponse = new PaymentResponse()
            {
                Payment = new PaymentProcessed()
                {
                    Id = Guid.NewGuid().ToString(),
                    Source = new CardSourceResponse() { Type = CardSource.TypeName }
                }
            };
            _paymentsClient = new Mock<IPaymentsClient>();
            _paymentsClient.Setup(p =>
                    p.RequestAsync(It.IsAny<PaymentRequest<TokenSource>>(), default(CancellationToken), null))
                .ReturnsAsync(() => _paymentsResponse);
            _paymentsClient.Setup(p =>
                p.GetAsync(It.IsAny<string>(), default(CancellationToken)))
                    .ReturnsAsync(() => new GetPaymentResponse());

            _controller = new PaymentsController(_checkoutApi.Object, new JsonSerializer());

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(m => m.Action(It.IsAny<UrlActionContext>()))
                .Returns("test_route")
                .Verifiable();
            _controller.Url = mockUrlHelper.Object;
            var context = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            request.SetupGet(r => r.Path).Returns(() => new PathString("/Payments/Post"));
            request.SetupGet(r => r.Host).Returns(() => new HostString("localhost"));
            request.SetupGet(r => r.Scheme).Returns(() => "http");
            context.Setup(x => x.Request).Returns(request.Object);
            _controller.ControllerContext.HttpContext = context.Object;
            var routeData = new RouteData();
            routeData.Values.Add("controller", "Payments");
            _controller.ControllerContext.RouteData = routeData;
            _controller.TempData = new TempDataDictionary(context.Object, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public void CanRenderIndexView()
        {
            var result = _controller.Index();

            var viewResult = result.ShouldBeAssignableTo<ViewResult>();
            viewResult.ViewData.ModelState.IsValid.ShouldBeTrue();
            viewResult.ViewName.ShouldBeNull();
        }

        [Fact]
        public async Task GivenModelInvalidRenderIndexViewWithModelErrors()
        {
            _controller.ModelState.AddModelError("", "test");
            var model = new PaymentModel();

            var result = await _controller.Post(model);

            var viewResult = result.ShouldBeAssignableTo<ViewResult>();
            viewResult.ViewData.ModelState.IsValid.ShouldBeFalse();
            viewResult.ViewName.ShouldBe(nameof(PaymentsController.Index));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        public async Task GivenCardTokenInvalidRenderErrorView(string cardToken)
        {
            var model = CreateValidModel();
            model.CardToken = cardToken;

            var result = await _controller.Post(model);

            var viewResult = result.ShouldBeAssignableTo<ViewResult>();
            viewResult.ViewName.ShouldBe(nameof(PaymentsController.Error));
            viewResult.Model.ShouldNotBeNull();
            var errorModel = viewResult.Model.ShouldBeAssignableTo<ErrorViewModel>();
            errorModel.Message.ShouldNotBeNullOrWhiteSpace();
        }

        private PaymentModel CreateValidModel()
        {
            return new PaymentModel()
            {
                Amount = 3,
                Currency = "USD",
                DoThreeDS = false,
                CardToken = "test"
            };
        }

        [Fact]
        public async Task GivenExceptionThrownRenderErrorView()
        {
            _checkoutApi.SetupGet(a => a.Payments).Returns(() => throw new Exception());

            var result = await _controller.Post(new PaymentModel());

            var viewResult = result.ShouldBeAssignableTo<ViewResult>();
            viewResult.ViewName.ShouldBe(nameof(PaymentsController.Error));
            viewResult.Model.ShouldNotBeNull();
            var errorModel = viewResult.Model.ShouldBeAssignableTo<ErrorViewModel>();
            errorModel.Message.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task CanRenderNonThreeDsSuccessView()
        {
            _checkoutApi.Setup(a => a.Payments).Returns(_paymentsClient.Object);
            _paymentsResponse.Payment.Approved = true;
            var model = CreateValidModel();
            model.DoThreeDS = false;

            var result = await _controller.Post(model);

            var viewResult = result.ShouldBeAssignableTo<RedirectToActionResult>();
            viewResult.ActionName.ShouldBe(nameof(PaymentsController.NonThreeDSSuccess));
            _controller.TempData.ShouldContainKey(nameof(PaymentProcessed));
            var serialized = _controller.TempData[nameof(PaymentProcessed)].ShouldBeAssignableTo<string>();
            var payment = (PaymentProcessed)_jsonSerializer.Deserialize(serialized, typeof(PaymentProcessed));
            payment.Approved.ShouldBeTrue();
        }

        [Fact]
        public async Task CanRenderNonThreeDsFailureView()
        {
            _checkoutApi.Setup(a => a.Payments).Returns(_paymentsClient.Object);
            _paymentsResponse.Payment.Approved = false;
            var model = CreateValidModel();
            model.DoThreeDS = false;

            var result = await _controller.Post(model);

            var viewResult = result.ShouldBeAssignableTo<RedirectToActionResult>();
            viewResult.ActionName.ShouldBe(nameof(PaymentsController.NonThreeDSFailure));
            _controller.TempData.ShouldContainKey(nameof(PaymentProcessed));
            var serialized = _controller.TempData[nameof(PaymentProcessed)].ShouldBeAssignableTo<string>();
            var payment = (PaymentProcessed)_jsonSerializer.Deserialize(serialized, typeof(PaymentProcessed));
            payment.Approved.ShouldBeFalse();
        }

        [Fact]
        public async Task CanRedirectThreeDs()
        {
            _checkoutApi.Setup(a => a.Payments).Returns(_paymentsClient.Object);
            _paymentsResponse.Pending = new PaymentPending();
            var redirectLink = new Link() { Href = "test" };
            _paymentsResponse.Pending.Links.Add("redirect", redirectLink);
            var model = CreateValidModel();
            model.DoThreeDS = true;

            var result = await _controller.Post(model);

            var viewResult = result.ShouldBeAssignableTo<RedirectResult>();
            viewResult.Url.ShouldBe(redirectLink.Href);
        }

        [Fact]
        public async Task CanRenderThreeDsSuccessView()
        {
            _checkoutApi.Setup(a => a.Payments).Returns(_paymentsClient.Object);
            var result = await _controller.ThreeDSSuccess("test");

            var viewResult = result.ShouldBeAssignableTo<ViewResult>();
            viewResult.ViewName.ShouldBeNull();
            viewResult.Model.ShouldBeAssignableTo<GetPaymentResponse>();
        }

        [Fact]
        public async Task CanRenderThreeDsFailureView()
        {
            _checkoutApi.Setup(a => a.Payments).Returns(_paymentsClient.Object);
            var result = await _controller.ThreeDSFailure("test");

            var viewResult = result.ShouldBeAssignableTo<ViewResult>();
            viewResult.ViewName.ShouldBeNull();
            viewResult.Model.ShouldBeAssignableTo<GetPaymentResponse>();
        }
    }
}

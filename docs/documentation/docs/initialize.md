---
id: initialize
title: Initialize
---


To start making API requests you need to create an instance of `CheckoutApi` providing your Checkout.com secret API key:

```jsx
var api = CheckoutApi.Create("sk_07fa5e52-3971-4bab-ae6b-a8e26007fccc", true);
```

```jsx
var api = CheckoutApi.Create("sk_07fa5e52-3971-4bab-ae6b-a8e26007fccc", true, "pk_7d9921be-b71f-47fa-b996-29515831d911");
```

The second parameter determines whether the SDK will use our Sandbox API (default) or Production. For testing purposes you should use Sandbox. The third parameter is your public key (used only for tokenisation).

See [[configuration|.NET-Core]] for more details on how to configure the SDK.

`CheckoutApi` provides access to each of our API resources, for example:

```jsx
var tokenResponse = await api.Tokens.RequestAsync(...);
var paymentResponse = await api.Payments.RequestAsync(...);
``` 

## General Usage

To make the SDK as intuitive as possible, each API operation will typically have a corresponding request and response type. For example:

```jsx
var refundRequest = new RefundRequest { Amount = 100 };
var refundResponse = await api.Payments.RefundAsync("pay_xxx", refundRequest);
var actionId = refundResponse.ActionId;
```

All parameters required by our API must be passed in the request object constructor. All optional parameters are set using property setters.

For detailed examples, please see [[API Resources]].
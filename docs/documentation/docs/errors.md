---
id: errors
title: Error Handling
---

## CheckoutApiException

With the exception of the cases below, the SDK will throw a `CheckoutApiException` if a non-successful HTTP status code is returned. 

### Validation Errors (HTTP 422)

If you send invalid data to our APIs we return a `HTTP 422` response with a JSON error similar to the following:

```json
{
  "request_id": "b20f81ac-e6e7-40a2-b048-92e319da030c",
  "error_type": "request_invalid",
  "error_codes": [
    "payment_source_required"
  ]
}
```

The SDK will throw a `CheckoutValidationException` which can be caught to obtain the underlying validation error details:

```jsx
try
{
    var apiResponse = await api.Payments.RequestAsync(paymentRequest);
}
catch (CheckoutValidationException validEx)
{
    return ShowError(validEx.Error);
}

```

### Resource Not Found (HTTP 404)

In the event of a resource not being present the SDK will throw a `CheckoutResourceNotFoundException`.


## How errors are determined

The errors above are triggered by status codes that do not fall in the 20X Status codes, or by validation issues. This means that statuses like a 202, 204 will not throw an exception

:::tip

It's important to understand that Declines or 3D Secure responses do not throw an exception since the status code associated with them is in the 20X range. In the Payments section you will see some examples of best practices when it comes to handling responses.

:::

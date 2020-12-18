---
id: sources
title: Sources
---

export const Highlight = ({children, color}) => (
<span
style={{
      color: color,
      padding: '0.2rem',
    }}>
{children}
</span>
);

You can find a list of request body parameters and possible outcomes [here](https://api-reference.checkout.com/#tag/Sources).

The SDK will infer the type of the source, if not provided.

## Add a <Highlight color="#25c2a0">SEPA source</Highlight>

:::note

SEPA is not enabled by default, so please let your account manager know if you want to use it.

:::

```jsx
var sourceRequest = new SourceRequest
(
  "sepa",
  new Address()
  {
    AddressLine1 = "Checkout.com",
    AddressLine2 = "Shepherdess Walk",
    City = "London",
    State = "London",
    Zip = "N1 7LH",
    Country = "GB"
  }
)
{
  Reference = "X-080957-N34",
  Phone = new Phone()
  {
    CountryCode = "+1",
    Number = "415 555 2671"
  },
  SourceData = new SourceData()
  {
    { "first_name", "Marcus" },
    { "last_name", "Barrilius Maximus" },
    { "account_iban", "DE68100100101234567895" },
    { "bic", "PBNKDEFFXXX" },
    { "billing_descriptor", "Test" },
    { "mandate_type", "single" }
  }
};

try
{
  var sourceResponse = await api.Sources.RequestAsync(sourceRequest);
  var source = sourceResponse.Source;
}
catch (CheckoutValidationException validationEx)
{
  return ValidationError(validationEx.Error);
}
catch (CheckoutApiException apiEx)
{
  Log.Error("Source request failed with status code {HttpStatusCode}", apiEx.HttpStatusCode);
  throw;
}
```

## Add a <Highlight color="#25c2a0">ACH source</Highlight>

:::note

ACH is not enabled by default, so please let your account manager know if you want to use it.

:::

```jsx
var sourceRequest = new SourceRequest
(
  "ach",
  new Address()
  {
    AddressLine1 = "Checkout.com",
    AddressLine2 = "Shepherdess Walk",
    City = "London",
    State = "London",
    Zip = "N1 7LH",
    Country = "GB"
  }
)
{
  SourceData = new SourceData()
  {
    { "account_holder_name", "Bruce Wayne" },
    { "account_type": "Checking"},
    {"account_number": "0123456789"},
    {"routing_number": "211370545"},
    {"billing_descriptor": "ACH Demo},
    {"company_name": null}
  }
};

try
{
  var sourceResponse = await api.Sources.RequestAsync(sourceRequest);
  var source = sourceResponse.Source;
}
catch (CheckoutValidationException validationEx)
{
  return ValidationError(validationEx.Error);
}
catch (CheckoutApiException apiEx)
{
  Log.Error("Source request failed with status code {HttpStatusCode}", apiEx.HttpStatusCode);
  throw;
}


```

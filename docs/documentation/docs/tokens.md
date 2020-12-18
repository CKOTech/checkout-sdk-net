---
id: tokens
title: Tokens
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

You can find a list of request body parameters and possible outcomes [here](https://api-reference.checkout.com/#tag/Tokens).

The SDK will infer the type of the payload, if not provided.

## Request a token for <Highlight color="#25c2a0">Apple Pay</Highlight>

```jsx
var tokenData = new Dictionary<string, object>
{
  { "version", "EC_v1" },
  { "data", "t7GeajLB9skXB6QSWfEpPA4WPhD..." },
  { "signature", "MIAGCSqGSIb3DQEHAqCAMI..." },
  { "header", new Dictionary<string, string>
    {
      { "ephemeralPublicKey", "MFkwEwYHK..." },
      { "publicKeyHash", "tqYV+tmG9aMh+l..." },
      { "transactionId", "3cee89679130a4..." }
    }
  }
};
var request = new WalletTokenRequest(WalletType.ApplePay, tokenData);

try
{
  var response = await api.Tokens.RequestAsync(request);
  var token = response.Token;
}
catch (CheckoutValidationException validationEx)
{
  return ValidationError(validationEx.Error);
}
catch (CheckoutApiException apiEx)
{
  Log.Error("Token request failed with status code {HttpStatusCode}", apiEx.HttpStatusCode);
  throw;
}
```

## Request a token for <Highlight color="#25c2a0">Google Pay</Highlight>

```jsx
const token = await cko.tokens.request({
    // type:"googlepay" is inferred
    token_data: {
        protocolVersion: 'EC_v1',
        signature: 'XXX',
        signedMessage: 'XXX'
    }
});

var tokenData = new Dictionary<string, object>
{
  { "version", "EC_v1" },
  { "signature", "MIAGCSqGSIb3DQEHAqCAMI..." },
  { "signedMessage", "......." },
};
var request = new WalletTokenRequest(WalletType.GooglePay, tokenData);

try
{
  var response = await api.Tokens.RequestAsync(request);
  var token = response.Token;
}
catch (CheckoutValidationException validationEx)
{
  return ValidationError(validationEx.Error);
}
catch (CheckoutApiException apiEx)
{
  Log.Error("Token request failed with status code {HttpStatusCode}", apiEx.HttpStatusCode);
  throw;
}
```

## Request a token for <Highlight color="#25c2a0">raw card details</Highlight>

:::warning

If you do not have SEQ-D level of PCI Compliance, this interaction can only be done in the test environment in case you want to unit test your code and you need a token to subsequently do a payment. In the production environment you need to use a solution like **Checkout.Frames** to generate the token for you.

:::

```jsx
CardTokenRequest cardTokenRequest = new CardTokenRequest("4242424242424242", 4, 24) 
{ 
    Cvv = "100" 
};

CardTokenResponse cardTokenResponse = await api.Tokens.RequestAsync(cardTokenRequest);
return cardTokenResponse.Token;
```

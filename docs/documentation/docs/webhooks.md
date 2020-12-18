---
id: webhooks
title: Webhooks
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

You can find a list of request body parameters and possible outcomes [here](https://api-reference.checkout.com/#tag/Webhooks).

## Retrieve webhooks

```jsx
var webhooksRetrievalResponse = await api.Webhooks.RetrieveWebhooksAsync();
```

## Register webhook

```jsx
var webhook = new Webhook()
{
    Url = "https://example.com/webhook",
    EventTypes = new List<string>
    {
        "payment_pending",
        "payment_captured"
    },
    Headers = new Dictionary<string, string>
    {
        { "Authorization", "1234" }
    }
};
var webhookRegistrationResponse = await api.Webhooks.RegisterWebhookAsync(new RegisterWebhookRequest(webhook));
```

## Retrieve webhook

```js
var webhookRetrievalResponse = await api.Webhooks.RetrieveWebhookAsync("wh_tdt72zlbe7cudogxdgit6nwk6i");
```

## Update webhook

```jsx
var updatedWebhook = new Webhook()
{
    Url = "https://example.com/webhooks/updated",
    EventTypes = new List<string>
    {
        "payment_pending",
        "payment_captured"
    },
    Headers = new Dictionary<string, string>
    {
        { "Authorization", "1234" }
    }
};

var webhookUpdateResponse = await api.Webhooks.UpdateWebhookAsync(
    "wh_ahun3lg7bf4e3lohbhni65335u",
    new UpdateWebhookRequest(updatedWebhook)
);
```

## Partially update webhook

```jsx
var webhook = new Webhook()
{
    Url = "https://example.com/webhooks",
    EventTypes = new List<string>
    {
        "payment_pending",
        "payment_captured"
    },
    Headers = new Dictionary<string, string>
    {
        { "Authorization", "1234" }
    }
};
var webhookRegistrationResponse = await api.Webhooks.RegisterWebhookAsync(new RegisterWebhookRequest(webhook));

// Partially update
webhook.Url += "/partially/updated";
webhook.Headers = null;
var webhookPartialUpdateResponse = await api.Webhooks.PartiallyUpdateWebhookAsync(
    webhookRegistrationResponse.Id,
    new PartialUpdateWebhookRequest(webhook)
    );
```

## Remove webhook

```js
var webhooksRetrievalResponse = await api.Webhooks.RemoveWebhookAsync("wh_ahun3lg7bf4e3lohbhni65335u");
```

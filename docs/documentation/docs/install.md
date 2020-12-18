---
id: install
title: Install
---

## Installing the SDK

The SDK can be installed from [NuGet](https://www.nuget.org/packages/CheckoutSDK). We also publish unstable builds to [MyGet](https://www.myget.org/feed/checkout/package/nuget/CheckoutSDK).

### Install with NuGet

```ssh
Install-Package CheckoutSDK -Version 2.3.0
```

### Install with .NET CLI
```ssh
dotnet add package CheckoutSDK --version 2.3.0dd package CheckoutSDK --version 2.3.1-unstable0028 --source https://www.myget.org/F/checkout/api/v3/index.json
```

## Import

```jsx
using Checkout;
using Checkout.Payments; //etc
```


---
id: net-core
title: .Net Core
---


If you are using the SDK in a .NET Core application and make use of either the Microsoft Dependency Injection or Configuration libraries you can use the [CheckoutSDK.Extensions.Microsoft](https://www.nuget.org/packages/CheckoutSDK.Extensions.Microsoft) package.

## Dependency Injection

With the above package installed, register SDK with the built-in DI container in `Startup.cs`:

```jsx
public void ConfigureServices(IServiceCollection services)
{
    // ...
    var configuration = new CheckoutConfiguration("sk_70d144d5-92bd-4040-83cf-faeb978b3d75", useSandbox: true);
    services.AddCheckoutSdk(configuration);    
}
```

Then take a dependency on `ICheckoutApi` in your class constructor:

```jsx
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutApi _checkoutApi;

    public CheckoutController(ICheckoutApi checkoutApi)
    {
        _checkoutApi = checkoutApi ?? throw new ArgumentNullException(nameof(checkoutApi));
    }

    // etc.
}
```

If you wish to override some of the [[Architecture|SDK's dependencies]] you can do this by registering the overrides after calling `AddCheckoutSdk` for example:

```jsx
var configuration = new CheckoutConfiguration("sk_70d144d5-92bd-4040-83cf-faeb978b3d75", useSandbox: true);
services.AddCheckoutSdk(configuration);    
services.AddTransient<IHttpClientFactory, MyCustomHttpClientFactory>();
```

## Configuration

If you would prefer to configure the SDK using a JSON settings file or Environment Variables, we recommend using [.NET Core Configuration](https://github.com/aspnet/Configuration) and providing your `IConfiguration` when calling `AddCheckoutSdk`:

```jsx
public class Startup
{
    public Startup(IConfiguration configuration, IHostingEnvironment env)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // ...
        services.AddCheckoutSdk(Configuration);    
    }
}
```

### JSON

If you are using the JSON file Configuration Provider, you can then add your Checkout credentials to your `appsettings.json` file:

```json
{
  "Checkout": {
    "UseSandbox": true,
    "SecretKey" : "sk_07fa5e52-3971-4bab-ae6b-a8e26007fccc"
  }
}
```

### Environment Variables

Alternatively, if using the Environment Variable Configuration Provider, set the following environment variables:

- `Checkout__UseSandbox`
- `Checkout__SecretKey`



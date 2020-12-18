---
id: logging
title: Logging
---
The Checkout.com SDK for .NET uses [LibLog](https://github.com/damianh/LibLog) for logging. 

LibLog is a logging abstraction designed specifically for library developers with _transparent_ built-in support for NLog, Log4Net, Serilog, Loupe and custom providers if necessary.

For examples of the above logging library please see the [LibLog repository](https://github.com/damianh/LibLog/tree/master/src). Once you've wired up the logging library of your choice, SDK will start logging to it.
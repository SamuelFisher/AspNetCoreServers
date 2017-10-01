# AspNetCoreServers

This project provides examples for using different servers to host an ASP.NET Core application.

## Caddy via FastCGI

Reference `AspNetCoreServers.Caddy.FastCgi`. You must also download [`caddy.exe`](https://caddyserver.com/) and place it in the working directory of your application.

A `WebHostBuilder` can be configured as follows:

```csharp
webHostBuilder
    .UseCaddyFastCgi()
    .UseStartup<Startup>()
    .UseUrls("http://localhost:5000/");
```

Alternatively, specify the path to the Caddy executable using:

```csharp
webHostBuilder
    .UseCaddyFastCgi(options => options.CaddyExecutablePath = "path/to/caddy")
```

## FastCGI

This uses the [Microsoft.AspNetCore.Owin](https://www.nuget.org/packages/Microsoft.AspNetCore.Owin/) adapter to handle requests from the [FastCGI OWIN Server](https://github.com/mzabani/Fos).

```chsarp
webHostBuilder
    .UseFastCgi(options =>
    {
        options.BindAddress = IPAddress.Loopback;
        options.BindPort = 9000;
    })
    .UseStartup<Startup>();
```

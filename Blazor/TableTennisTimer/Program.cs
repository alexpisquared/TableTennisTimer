var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.
  AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }).
  AddScoped<HttpClient>().
  AddSingleton<IScreenWakeLockService, ScreenWakeLockService>().
  AddScoped<IWebEventLoggerService, WebEventLoggerService>();

await builder.Build().RunAsync();

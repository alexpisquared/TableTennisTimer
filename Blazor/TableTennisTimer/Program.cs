var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }).
  AddSingleton<HttpClient>().
  AddSingleton<IScreenWakeLockService, ScreenWakeLockService>().
  AddSingleton<IWebEventLoggerService, WebEventLoggerService>();

await builder.Build().RunAsync();

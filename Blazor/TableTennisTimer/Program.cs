using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ScreenWakeLockDemo.Services;
using TableTennisTimer.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }).
  AddSingleton<IScreenWakeLockService, ScreenWakeLockService>().
  AddScoped<WebEventLoggerService>();

await builder.Build().RunAsync();

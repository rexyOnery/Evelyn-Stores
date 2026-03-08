using Blazored.LocalStorage;
using Blazored.SessionStorage;
using EvelynStores.Core.Services;
using EvelynStores.Shop;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

builder.Services.AddHttpClient<IEvelynPhilApiHttpClient, EvelynPhilApiHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7001");
});


// Configure HttpClient to call the API. If ApiBaseUrl is provided in configuration use it,
// otherwise fall back to the host environment base address.
var apiBase = builder.Configuration["ApiBaseUrl"];
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBase ?? builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

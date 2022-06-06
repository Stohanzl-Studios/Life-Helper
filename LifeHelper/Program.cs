using LifeHelper;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using LifeHelper.Logic;
using Blazored.SessionStorage;
using Microsoft.Extensions.Logging;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();

await builder.Build().RunAsync();
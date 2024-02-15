using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Reenbit_TestTask.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://reenbittesttaskappservice.azurewebsites.net") });
//builder.Services.AddHttpClient("MyApi", client => client.BaseAddress = new Uri("https://reenbittesttaskappservice.azurewebsites.net/"));

builder.Services.AddCors(options => options.AddPolicy("AllowAll",
    builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    )
);

await builder.Build().RunAsync();

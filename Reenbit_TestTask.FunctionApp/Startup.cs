using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

[assembly: FunctionsStartup(typeof(Reenbit_TestTask.FunctionApp.Startup))]

namespace Reenbit_TestTask.FunctionApp;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var blobConnectionString = Environment.GetEnvironmentVariable("BlobConnectionString");
        builder.Services.AddScoped(serviceProvider => 
                                 new BlobServiceClient(blobConnectionString));
    }
}
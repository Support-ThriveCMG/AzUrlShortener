using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Data.Tables;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddSingleton<TableServiceClient>(sp =>
        {
            var connectionString =
                Environment.GetEnvironmentVariable("strTables");

            return new TableServiceClient(connectionString);
        });
    })
    .Build();

host.Run();

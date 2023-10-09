using EFCore.DTO.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<VehicleRegistryContext>(options => {
            options.UseSqlServer(config.GetConnectionString("Sql"));
        }, ServiceLifetime.Transient);
    })
    .Build();

host.Run();
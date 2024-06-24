using Infrastructure.Common.Models;
using Infrastructure.Consumers;
using Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure;

/// <summary>
/// Extension Class For <see cref="IServiceCollection"/> Interface
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    /// Injects Infrastructure Dependencies Into Dependency Injection Container
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/> Interface</param>
    /// <param name="configuration"><see cref="IConfiguration"/> Interface</param>
    public static void AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {

        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter(); //user-created-event
            busConfig.AddConsumer<UserRegisterConsumer>();
#if DEBUG
            var settings = new MessageBrokerSettings();
            configuration.GetSection("MessageBroker").Bind(settings);
#else
            var stringSettings = Environment.GetEnvironmentVariable("MessageBroker");
            var settings = JsonConvert.DeserializeObject<MessageBrokerSettings>(stringSettings);
#endif
            busConfig.UsingRabbitMq((context, configuration) =>
            {

                configuration.Host(new Uri(settings.Host!), h =>
                {
                    h.Username(settings.Username!);
                    h.Password(settings.Password!);
                });

                configuration.ConfigureEndpoints(context);
            });
        });


        if (Convert.ToBoolean(configuration.GetValue<bool>("UseInMemoryDatabase")))
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("TestDb"));
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                string connString = string.Empty;
#if DEBUG
                connString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;     
#else
                connString = Environment.GetEnvironmentVariable("CONNSTRING");
#endif
                options.UseNpgsql(connString,
                    builder =>
                    {
                        builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        //EF allows you to specify that a given LINQ query should be split into multiple SQL queries.
                        //Instead of JOINs, split queries generate an additional SQL query for each included collection navigation
                        //More about that: https://docs.microsoft.com/en-us/ef/core/querying/single-split-queries
                        builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });
            });
        }

        services.AddScoped<IApplicationDbContext>(x => x.GetService<ApplicationDbContext>()!);
    }
}
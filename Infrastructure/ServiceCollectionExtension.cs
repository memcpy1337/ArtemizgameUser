using Application.Services;
using Contracts.Events.MatchMakingEvents;
using Contracts.Events.ServerEvents;
using Infrastructure.Common.Models;
using Infrastructure.Consumers;
using Infrastructure.Persistence;
using Infrastructure.Sagas;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Net;

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
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IUserNotifierService, UserNotifierService>();

        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter(); //user-created-event
            busConfig.AddConsumer<UserRegisterConsumer>();

            busConfig.AddSagaStateMachine<PlayerQueueSaga, PlayerQueueSagaData>()
            .EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                    r.ExistingDbContext<ApplicationDbContext>();
                    r.LockStatementProvider = new PostgresLockStatementProvider();
                    r.UsePostgres();
                });


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

                configuration.ReceiveEndpoint("user-register-queue-users", e =>
                {
                    e.ConfigureConsumer<UserRegisterConsumer>(context);
                });

                configuration.ReceiveEndpoint("saga-queue", e =>
                {
                    const int ConcurrencyLimit = 20;

                    e.PrefetchCount = ConcurrencyLimit;

                    e.UseMessageRetry(r => r.Interval(5, 1000));
                    e.UseInMemoryOutbox();

                    e.ConfigureSaga<PlayerQueueSagaData>(context, s =>
                    {
                        var partition = s.CreatePartitioner(ConcurrencyLimit);

                        s.Message<QueuePlayerAddEvent>(x => x.UsePartitioner(partition, m => m.Message.UserId));
                        s.Message<MatchPlayerAddEvent>(x => x.UsePartitioner(partition, m => m.Message.UserId));
                        s.Message<ServerConnectionDataUpdateEvent>(x => x.UsePartitioner(partition, m => m.Message.MatchId));
                        s.Message<MatchReadyEvent>(x => x.UsePartitioner(partition, m => m.Message.MatchId));
                        s.Message<ServerPlayerConnectedEvent>(x => x.UsePartitioner(partition, m => m.Message.UserId));
                        s.Message<QueuePlayerRemoveEvent>(x => x.UsePartitioner(partition, m => m.Message.UserId));
                        s.Message<MatchCancelEvent>(x => x.UsePartitioner(partition, m => m.Message.MatchId));
                        s.Message<MatchPlayerRemoveEvent>(x => x.UsePartitioner(partition, m => m.Message.UserId));
                    });
                });

                configuration.ConfigureEndpoints(context);
            });
        });


#if DEBUG
        var redisSettings = new RedisSettings();
        configuration.GetSection("RedisSettings").Bind(redisSettings);
#else
        var redisStringSettings = Environment.GetEnvironmentVariable("RedisSettings");
        var redisSettings = JsonConvert.DeserializeObject<RedisSettings>(redisStringSettings);
#endif
        var config = new ConfigurationOptions
        {
            EndPoints = { redisSettings.Host },
            User = redisSettings.User,
            Password = redisSettings.Password,
            AbortOnConnectFail = false
        };

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(config));

        services.AddSignalR()
        .AddStackExchangeRedis(o =>
        {
            o.ConnectionFactory = async writer =>
            {
                var config = new ConfigurationOptions
                {
                    EndPoints = { redisSettings.Host },
                    User = redisSettings.User,
                    Password = redisSettings.Password,
                    AbortOnConnectFail = false
                };
                config.SetDefaultPorts();
                var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
                connection.ConnectionFailed += (_, e) =>
                {
                    Console.WriteLine("Connection to Redis failed.");
                };

                if (!connection.IsConnected)
                {
                    Console.WriteLine("Did not connect to Redis.");
                }
                else
                {
                    Console.WriteLine("CONNECTED to Redis.");
                }


                return connection;
            };
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
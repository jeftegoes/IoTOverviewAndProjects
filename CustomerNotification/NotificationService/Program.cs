using System;
using NotificationService.Consumers;
using GreenPipes;
using MassTransit;
using Messaging.Interfaces.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using EmailService;
using System.Threading.Tasks;

namespace NotificationService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile($"appsettings.json", optional: false);
                    configHost.AddEnvironmentVariables();
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, config) => 
                {
                    config.AddJsonFile($"appsettings.{ hostContext.HostingEnvironment.EnvironmentName }.json", optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var emailConfig = hostContext.Configuration
                        .GetSection("EmailConfiguration")
                        .Get<EmailConfig>();

                    services.AddSingleton(emailConfig);
                    services.AddScoped<IEmailSender, EmailSender>();

                    services.AddMassTransit(c =>
                    {
                        c.AddConsumer<OrderProcessedEventConsumer>();

                        c.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                        {
                            config.Host("rabbitmq", "/", h =>
                            {
                                h.Username(RabbitMqMassTransitConstants.UserName);
                                h.Password(RabbitMqMassTransitConstants.Password);
                            });

                            config.ReceiveEndpoint(RabbitMqMassTransitConstants.NotificationServiceQueue, e =>
                            {
                                e.PrefetchCount = 16;
                                e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
                                e.Consumer<OrderProcessedEventConsumer>(provider);
                            });
                        }));
                    });

                    services.AddSingleton<IHostedService, BusService>();
                });

            return hostBuilder;
        }
    }
}

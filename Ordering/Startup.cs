using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Messaging.Interfaces.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Ordering.Hubs;
using Ordering.Messages.Consumers;
using Ordering.Persistence;
using Ordering.Services;

namespace Ordering
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OrderContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("OrderContextConnection")
            ));

            services.AddHttpClient();
            services.AddSignalR();

            services.AddTransient<IOrderRepository, OrderRepository>();

            services.AddMassTransit(c =>
            {
                c.AddConsumer<RegisterOrderCommandConsumer>();
                c.AddConsumer<OrderDispatchedEventConsumer>();

                c.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.Host("localhost", "/", h =>
                    {
                        h.Username(RabbitMqMassTransitConstants.UserName);
                        h.Password(RabbitMqMassTransitConstants.Password);
                    });

                    config.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, e =>
                    {
                        e.PrefetchCount = 16;
                        e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
                        e.Consumer<RegisterOrderCommandConsumer>(provider);
                    });

                    config.ReceiveEndpoint(RabbitMqMassTransitConstants.OrderDispatchedServiceQueue, e =>
                    {
                        e.PrefetchCount = 16;
                        e.UseMessageRetry(x => x.Interval(2, 100));
                        e.Consumer<OrderDispatchedEventConsumer>(provider);
                    });
                }));
            });

            services.AddSingleton<IHostedService, BusService>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials());
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering v1"));
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<OrderHub>("/ordersHub");
            });
        }
    }
}

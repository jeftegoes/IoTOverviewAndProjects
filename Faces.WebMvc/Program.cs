using Faces.SharedLib.Constants;
using Faces.WebMvc.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IHostedService, BusService>();

builder.Services.AddMassTransit(c =>
{
    // c.AddConsumer<RegisterOrderCommandConsumer>();
    // c.AddConsumer<OrderDispatchedEventConsumer>();
    c.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
    {
        config.Host("localhost", "/", h =>
        {
            h.Username(RabbitMqMassTransitConstants.UserName);
            h.Password(RabbitMqMassTransitConstants.Password);
        });
        // config.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, e =>
        // {
        //     e.PrefetchCount = 16;
        //     e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
        //     e.Consumer<RegisterOrderCommandConsumer>(provider);
        // });
        // config.ReceiveEndpoint(RabbitMqMassTransitConstants.OrderDispatchedServiceQueue, e =>
        // {
        //     e.PrefetchCount = 16;
        //     e.UseMessageRetry(x => x.Interval(2, 100));
        //     e.Consumer<OrderDispatchedEventConsumer>(provider);
        // });
    }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

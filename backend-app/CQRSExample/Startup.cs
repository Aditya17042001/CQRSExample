using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EventStore.Client;
using MongoDB.Driver;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add controllers for WebAPI
        services.AddControllers();

        // Enable CORS to allow requests from the React frontend
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Register EventStoreDB
        services.AddSingleton(new EventStoreClient(EventStoreClientSettings.Create("esdb://localhost:2113?tls=false")));

        // Register MongoDB connection
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var database = mongoClient.GetDatabase("cqrs-read-model");
        services.AddSingleton(database);

        // Add the background service for projections
        services.AddHostedService<UserProjectionService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Enable CORS
        app.UseCors("AllowAllOrigins");

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // Ensures only WebAPI controllers are used
        });
    }
}

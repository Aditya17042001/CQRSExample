using Microsoft.Extensions.Hosting;
using EventStore.Client;
using MongoDB.Driver;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class UserProjectionService : BackgroundService
{
    private readonly EventStoreClient _eventStoreClient;
    private readonly IMongoCollection<UserDocument> _usersCollection;

    public UserProjectionService(EventStoreClient eventStoreClient, IMongoDatabase database)
    {
        _eventStoreClient = eventStoreClient;
        _usersCollection = database.GetCollection<UserDocument>("users");
    }

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    await _eventStoreClient.SubscribeToStreamAsync(
        "user-stream",
        FromStream.Start,
        async (subscription, resolvedEvent, cancellationToken) =>
        {
            var jsonData = System.Text.Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);
            var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedEvent>(jsonData);

            var user = new UserDocument
            {
                Id = userCreatedEvent.Id,
                Name = userCreatedEvent.Name,
                Email = userCreatedEvent.Email
            };

            await _usersCollection.ReplaceOneAsync(
                u => u.Id == user.Id,
                user,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken
            );
        },
        cancellationToken: stoppingToken
    );
}

}

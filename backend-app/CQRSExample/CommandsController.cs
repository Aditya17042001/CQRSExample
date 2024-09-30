using Microsoft.AspNetCore.Mvc;
using EventStore.Client;
using System.Threading.Tasks;

[Route("api/commands")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly EventStoreClient _eventStoreClient;

    public CommandsController(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }

    [HttpPost("add-user")]
    public async Task<IActionResult> AddUser([FromBody] UserCommand command)
    {
        var userEvent = new UserCreatedEvent
        {
            Id = command.Id,
            Name = command.Name,
            Email = command.Email
        };

        await _eventStoreClient.AppendToStreamAsync(
            "user-stream",
            StreamState.Any,
            new[] { new EventData(
                Uuid.NewUuid(),
                nameof(UserCreatedEvent),
                System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(userEvent))
            }
        );

        return Ok();
    }
}

public class UserCommand
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UserCreatedEvent
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

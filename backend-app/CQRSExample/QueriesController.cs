using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/queries")]
[ApiController]
public class QueriesController : ControllerBase
{
    private readonly IMongoCollection<UserDocument> _usersCollection;

    public QueriesController(IMongoDatabase database)
    {
        _usersCollection = database.GetCollection<UserDocument>("users");
    }

    [HttpGet("users")]
    public async Task<IEnumerable<UserDocument>> GetUsers()
    {
        return await _usersCollection.Find(_ => true).ToListAsync();
    }
}

public class UserDocument
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

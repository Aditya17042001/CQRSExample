using EventStoreApiDemo.Models;
using EventStoreApiDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventStoreApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly EventStoreService _eventStoreService;

        public EventsController()
        {
            _eventStoreService = new EventStoreService();
        }

        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] SampleEvent sampleEvent)
        {
            await _eventStoreService.AddEventAsync(sampleEvent);
            return CreatedAtAction(nameof(GetEvents), new { }, sampleEvent);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SampleEvent>>> GetEvents()
        {
            var events = await _eventStoreService.ReadEventsAsync();
            return Ok(events);
        }
    }
}

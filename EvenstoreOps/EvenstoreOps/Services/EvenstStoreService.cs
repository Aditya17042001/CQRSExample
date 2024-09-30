using EventStore.Client;
using EventStoreApiDemo.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventStoreApiDemo.Services
{
    public class EventStoreService
    {
        private readonly EventStoreClient _client;
        private const string StreamName = "test-stream";

        public EventStoreService()
        {
            var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false"); // Adjust URL as necessary
            _client = new EventStoreClient(settings);
        }

        public async Task AddEventAsync(SampleEvent sampleEvent)
        {
            var eventData = new EventData(
                Uuid.NewUuid(),
                "test.event",
                data: JsonSerializer.SerializeToUtf8Bytes(sampleEvent),
                metadata: null);

            await _client.AppendToStreamAsync(StreamName, StreamState.Any, new[] { eventData });
        }

        public async Task<IEnumerable<SampleEvent>> ReadEventsAsync()
        {
            var events = new List<SampleEvent>();

            var readResult = _client.ReadStreamAsync(Direction.Forwards, StreamName, StreamPosition.Start);

            await foreach (var resolvedEvent in readResult)
            {
                var eventData = JsonSerializer.Deserialize<SampleEvent>(resolvedEvent.Event.Data.ToArray());
                events.Add(eventData);
            }

            return events;
        }
    }
}

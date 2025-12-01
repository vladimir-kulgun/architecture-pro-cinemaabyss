using System;

namespace Events.Api.CQRS.Events
{
    public class CreateEventResponse
    {
        public CreateEventResponse(int partition, long offset, bool success, string id, string type, string payload)
        {
            Partition = partition;
            Offset = offset;
            Status = success
                ? "success"
                : "error";
            Event = new EventData(id, type, payload);
        }

        public string Status { get; private set; }
        public int Partition { get; private set; }
        public long Offset { get; private set; }
        public EventData Event { get; private set; }
        public class EventData
        {
            public EventData(string id, string type, string payload)
            {
                Id = id;
                Type = type;
                Payload = payload;
                Timestamp = DateTime.UtcNow;
            }

            public string Id { get; private set; }
            public string Type { get; private set; }
            public DateTime Timestamp { get; private set; }
            public string Payload { get; private set; }
        }
    }
}
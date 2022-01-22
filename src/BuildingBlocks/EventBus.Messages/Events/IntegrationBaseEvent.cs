using System;

namespace EventBus.Messages.Events
{
    public class IntegrationBaseEvent
    {
        public Guid Id { get;  }

        public DateTime CreationDate { get;  }

        public IntegrationBaseEvent(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        public IntegrationBaseEvent(): this(Guid.NewGuid(), DateTime.UtcNow)
        {
        }
    }
}

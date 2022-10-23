using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IEventStore
    {
        IEnumerable<IEvent> LoadEvents(Guid aggregateId);
        void SaveEvents(Guid aggregateId, int eventsLoaded, List<IEvent> newEvents);

        IEnumerable<IEvent> GetAllEvents();
    }


    public class EventStore : IEventStore
    {
        private List<IEvent> events = new List<IEvent>();

        public IEnumerable<IEvent> LoadEvents(Guid aggregateId)
        {
            return events.Where(e => e.Id == aggregateId);
        }

        public void SaveEvents(Guid aggregateId, int eventsLoaded, List<IEvent> newEvents)
        {
            int numberOfExistingEventsForAggregate = events.Count(e => e.Id == aggregateId);

            if (eventsLoaded != numberOfExistingEventsForAggregate)
                throw new Exception("Concurrency conflict; cannot persist these events");

            events.AddRange(newEvents);
        }

        public IEnumerable<IEvent> GetAllEvents()
        {
            return events;
        }
    }
}

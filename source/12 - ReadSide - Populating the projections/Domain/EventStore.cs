using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IReadsideEventStore
    {
        void ForAllEvents(Action<IEvent> apply);
    }

    public interface IEventStore
    {
        IEnumerable<IEvent> LoadEvents(Guid aggregateId);
        void SaveEvents(Guid aggregateId, int eventsLoaded, List<IEvent> newEvents);

        IEnumerable<IEvent> GetAllEvents();
    }

    public class EventStore : IEventStore, IReadsideEventStore
    {
        private List<IEvent> events = new List<IEvent>();

        private Action<IEvent> eventCallback;
        private int eventRead = 0;

        public void ForAllEvents(Action<IEvent> apply)
        {
            //Remember a lambda to call when we get a new event
            eventCallback = apply;

            //Call method to publish events
            PublishNewEvents();
        }

        private void PublishNewEvents()
        {
            //Do we have any new events?
            if (eventCallback != null)
            {
                while ((events.Count()) > eventRead)
                {
                    var ev = events[eventRead];
                    eventCallback(ev);
                    eventRead++;
                }
            }
        }

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

            PublishNewEvents();
        }

        public IEnumerable<IEvent> GetAllEvents()
        {
            return events;
        }
    }
}

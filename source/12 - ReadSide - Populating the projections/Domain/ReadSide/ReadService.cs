using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ReadSide
{
    public interface IReadService
    {
        TResult Query<TProjection, TResult>(Func<TProjection, TResult> query) where TProjection : Projection, new();

    }

    public class ReadService : IReadService
    {
        private readonly IReadsideEventStore eventStore;
        private readonly Dictionary<Type, Projection> projectionInstances = new();
        private readonly Dictionary<Type, List<Action<IEvent>>> eventSubscribers = new();

        public ReadService(IReadsideEventStore eventStore)
        {
            this.eventStore = eventStore;

            ScanAssembly();

            eventStore.ForAllEvents(e =>
            {
                PublishEvent(e);
            });
        }

        private void PublishEvent(IEvent e)
        {
            foreach (var subscriber in eventSubscribers[e.GetType()])
            {
                subscriber(e);
            }
        }

        public TResult Query<TProjection, TResult>(Func<TProjection, TResult> query) where TProjection : Projection, new()
        {
            //Get an instance of the requested projection
            var projectionInstance = (TProjection)projectionInstances[typeof(TProjection)];

            //Pass the projection to the lambda and execute it
            return query(projectionInstance);
        }


        private void ScanAssembly()
        {
            //1. Find all projections / events
            var projections = FindAllProjections();

            //2. Create an instance of each projection
            CreateProjectionInstances(projections);

            //3. map corresponding event -> to projection instance
            MapEventsToProjections(projections);
        }

        private List<(Type EventType, Type ProjectionType)> FindAllProjections()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var projections =
                from t in assembly.GetTypes()
                from i in t.GetInterfaces()
                where i.IsGenericType
                where i.GetGenericTypeDefinition() == typeof(IBuildFrom<>)
                select (EventType: i.GetGenericArguments()[0],
                        ProjectionType: t);

            Console.WriteLine("\r\nReadside projection subscribers");
            foreach (var projection in projections)
                Console.WriteLine(projection.EventType + " " + projection.ProjectionType);

            Console.WriteLine();

            return projections.ToList();
        }

        private void CreateProjectionInstances(List<(Type EventType, Type ProjectionType)> projections)
        {
            foreach (var p in projections)
            {
                if (!projectionInstances.ContainsKey(p.ProjectionType))
                {
                    var instance = Activator.CreateInstance(p.ProjectionType);

                    if (instance is Projection == false)
                        throw new Exception($"Projection '{p.ProjectionType.Name}' does not implement Projection base class");

                    projectionInstances.Add(p.ProjectionType, (Projection)instance);
                }
            }
        }

        private void MapEventsToProjections(List<(Type EventType, Type ProjectionType)> projections)
        {
            foreach (var projection in projections)
            {
                //Lookup the subscriber instance we created earlier
                var projInstance = projectionInstances[projection.ProjectionType];

                GetType().GetMethod("AddSubscriberFor")
                    ?.MakeGenericMethod(projection.EventType)
                    ?.Invoke(this, new object[] { projInstance });
            }
        }

        public void AddSubscriberFor<TEvent>(IBuildFrom<TEvent> subscriber) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);

            if (!eventSubscribers.ContainsKey(eventType))
                eventSubscribers.Add(eventType, new List<Action<IEvent>>());

            eventSubscribers[eventType].Add(e =>
            {
                subscriber.Apply((TEvent)e);
            });
        }
    }
}

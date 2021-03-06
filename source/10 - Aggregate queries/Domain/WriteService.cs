using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IWriteService
    {
        void HandleCommand<TCommand>(TCommand command) where TCommand : ICommand;

        TResult QueryAggregate<TAggregate, TResult>(Guid aggregateId, Func<TAggregate, TResult> query) where TAggregate : Aggregate, new();
    }


    public class WriteService : IWriteService
    {
        private readonly IEventStore eventStore;

        public WriteService(IEventStore eventStore)
        {
            this.eventStore = eventStore;
            ScanAssembly();
        }

        private void ScanAssembly()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var handlers =
                from t in assembly.GetTypes()
                from i in t.GetInterfaces()
                where i.IsGenericType
                where i.GetGenericTypeDefinition() == typeof(IHandleCommand<>)
                select new
                {
                    CommandType = i.GetGenericArguments()[0],
                    AggregateType = t
                };

            foreach (var handler in handlers)
                Console.WriteLine(handler.CommandType + " " + handler.AggregateType);

            foreach (var h in handlers)
            {
                GetType().GetMethod("AddCommandHandlerFor")!
                    .MakeGenericMethod(h.CommandType, h.AggregateType)
                    .Invoke(this, Array.Empty<object>());

            }
        }

        private readonly Dictionary<Type, Action<ICommand>> commandHandlers = new();

        public void AddCommandHandlerFor<TCommand, TAggregate>() where TCommand : ICommand
                                                               where TAggregate : Aggregate, new()
        {
            //var handler = (TAggregate)Activator.CreateInstance(typeof(TAggregate), new object[] { orderRepository });

            commandHandlers.Add(typeof(TCommand), c =>
            {
                //Load the existing events
                var events = eventStore.LoadEvents(c.Id);
                int eventsLoaded = events.Count();

                //Create the instance of the aggregate
                var agg = new TAggregate();
                agg.ApplyEvents(events);

                //Handle the command
                var newEvents = (agg as IHandleCommand<TCommand>).Handle((TCommand)c).ToList();

                Console.WriteLine("\r\nNew events");
                foreach (var e in newEvents)
                {
                    Console.WriteLine(e.ToString());
                }

                //Save the new events
                if (newEvents.Count() > 0)
                {
                    eventStore.SaveEvents(c.Id, eventsLoaded, newEvents);
                }
            });
        }


        public void HandleCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            Console.WriteLine("\r\nHandling command " + command.ToString());

            if (commandHandlers.ContainsKey(typeof(TCommand)))
            {
                commandHandlers[typeof(TCommand)](command);
            }
            else
            {
                throw new Exception("No command handler registered for " + typeof(TCommand).Name);
            }
        }

        public TResult QueryAggregate<TAggregate, TResult>(Guid aggregateId, Func<TAggregate, TResult> query) where TAggregate : Aggregate, new()
        {
            var events = eventStore.LoadEvents(aggregateId);
            int eventsLoaded = events.Count();

            var agg = new TAggregate();
            agg.ApplyEvents(events);

            return query(agg);

        }
    }
}

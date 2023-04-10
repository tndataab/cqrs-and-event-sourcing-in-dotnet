using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WriteSide
{
    public interface IWriteService
    {
        void HandleCommand<TCommand>(TCommand command) where TCommand : ICommand;
    }

    public class WriteService : IWriteService
    {
        private readonly IOrderRepository repository;
        private readonly Dictionary<Type, Action<ICommand>> commandHandlers = new();

        public WriteService(IOrderRepository repository)
        {
            this.repository = repository;

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

            Console.WriteLine("\r\nFound command handlers");

            foreach (var handler in handlers)
                Console.WriteLine(handler.CommandType + " " + handler.AggregateType);

            foreach (var h in handlers)
            {
                this.GetType().GetMethod("AddCommandHandlerFor")
                    .MakeGenericMethod(h.CommandType, h.AggregateType)
                    .Invoke(this, Array.Empty<object>());

            }
        }

        public void AddCommandHandlerFor<TCommand, THandler>() where TCommand : ICommand
                                                                where THandler : IHandleCommand<TCommand>
        {
            var handler = (THandler)Activator.CreateInstance(typeof(THandler), new object[] { repository });

            commandHandlers.Add(typeof(TCommand), c =>
            {
                var newEvents = handler.Handle((TCommand)c).ToList();

                Console.WriteLine("\r\nNew events");
                foreach (var e in newEvents)
                {
                    Console.WriteLine(e.ToString());
                }
            });
        }

        public void HandleCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            Console.WriteLine("\r\nHandling command\r\n" + command.ToString());

            if (commandHandlers.ContainsKey(typeof(TCommand)))
            {
                var handler = commandHandlers[typeof(TCommand)];
                handler(command);
            }
            else
            {
                throw new Exception("No command handler registered for " + typeof(TCommand).Name);
            }
        }
    }
}

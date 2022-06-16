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
    }


    public class WriteService : IWriteService
    {
        private readonly IOrderRepository orderRepository;

        public WriteService(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;

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

        public void AddCommandHandlerFor<TCommand, THandler>() where TCommand : ICommand
                                                               where THandler : IHandleCommand<TCommand>
        {
            var handler = (THandler)Activator.CreateInstance(typeof(THandler), new object[] { orderRepository });

            commandHandlers.Add(typeof(TCommand), c =>
            {
                handler.Handle((TCommand)c);
            });
        }


        public void HandleCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (commandHandlers.ContainsKey(typeof(TCommand)))
            {
                commandHandlers[typeof(TCommand)](command);
            }
            else
            {
                throw new Exception("No command handler registered for " + typeof(TCommand).Name);
            }
        }
    }
}

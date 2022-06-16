using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IEvent
    {
        public Guid Id { get; set; }
    }

    public class OrderCreated : IEvent
    {
        public Guid Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
    }

    public class OrderCancelled : IEvent
    {
        public Guid Id { get; set; }
    }

    public class OrderLineDeleted : IEvent
    {
        public Guid Id { get; set; }
        public Guid OrderLineId { get; set; }
    }

    public class OrderLineAdded : IEvent
    {
        public Guid Id { get; set; }
        public OrderLine OrderLine { get; set; }
    }
}

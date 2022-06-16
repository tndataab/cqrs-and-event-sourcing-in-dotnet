using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface ICommand
    {
        public Guid Id { get; set; }
    }

    public class CreateOrder : ICommand
    {
        public Guid Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
    }

    public class CancelOrder : ICommand
    {
        public Guid Id { get; set; }
    }

    public class DeleteOrderLine : ICommand
    {
        public Guid Id { get; set; }
        public Guid OrderLineId { get; set; }
    }
    public class AddOrderLine : ICommand
    {
        public Guid Id { get; set; }
        public OrderLine OrderLine { get; set; }
    }

}

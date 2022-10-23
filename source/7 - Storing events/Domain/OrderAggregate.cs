namespace Domain;

public interface IHandleCommand<in T> where T : ICommand
{
    IEnumerable<IEvent> Handle(T command);
}

public abstract class Aggregate
{ 
}

public class OrderAggregate : Aggregate, 
                              IHandleCommand<CreateOrder>,
                              IHandleCommand<CancelOrder>,
                              IHandleCommand<DeleteOrderLine>,
                              IHandleCommand<AddOrderLine>

{
    private readonly IOrderRepository repository;

    public OrderAggregate(IOrderRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<IEvent> Handle(CreateOrder c)
    {
        var order = new Order()
        {
            Id = c.Id,
            CustomerId = c.CustomerId,
            CustomerName = c.CustomerName,
            orderState = OrderState.New,
            orderLines = new List<OrderLine>()
        };

        repository.Insert(c.Id, order);
        
        yield return new OrderCreated()
        {
            Id = c.Id,
            CustomerId = c.CustomerId,
            CustomerName = c.CustomerName
        };
    }

    public IEnumerable<IEvent> Handle(CancelOrder c)
    {
        var order = repository.Load(c.Id);

        order.orderState = OrderState.cancel;

        repository.Update(c.Id, order);

        yield return new OrderCancelled()
        {
            Id = c.Id,
        };

    }

    public IEnumerable<IEvent> Handle(DeleteOrderLine c)
    {
        var order = repository.Load(c.Id);

        var ol = order.orderLines.FirstOrDefault(ol => ol.Id == c.OrderLineId);
        if (ol != null)
            order.orderLines.Remove(ol);

        repository.Update(c.Id, order);

        yield return new OrderLineDeleted()
        {
            Id = c.Id,
            OrderLineId = c.OrderLineId,
        };
    }

    public IEnumerable<IEvent> Handle(AddOrderLine c)
    {
        var order = repository.Load(c.Id);

        order.orderLines.Add(c.OrderLine);

        repository.Update(c.Id, order);
   
        yield return new OrderLineAdded()
        {
            Id = c.Id,
            OrderLine = c.OrderLine,
        };
    }
}

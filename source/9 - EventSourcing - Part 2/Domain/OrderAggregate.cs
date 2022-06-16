namespace Domain;



public interface IHandleCommand<in T> where T : ICommand
{
    IEnumerable<IEvent> Handle(T command);
}

public interface IApplyEvent<TEvent>
{
    void Apply(TEvent e);
}


public abstract class Aggregate
{
    public void ApplyEvents(IEnumerable<IEvent> events)
    {

    }
}

public class OrderAggregate : Aggregate,
                              IHandleCommand<CreateOrder>,
                              IHandleCommand<CancelOrder>,
                              IHandleCommand<DeleteOrderLine>,
                              IHandleCommand<AddOrderLine>,
                              IApplyEvent<OrderCreated>,
                              IApplyEvent<OrderCancelled>,
                              IApplyEvent<OrderLineAdded>,
                              IApplyEvent<OrderLineDeleted>

{
    public OrderAggregate()
    {

    }

    private Guid orderId = Guid.NewGuid();
    private int customerId;
    private string customerName;
    private OrderState orderState;
    private List<OrderLine> orderlines = new();

    public void Apply(OrderCreated e)
    {
        orderId = e.Id;
        customerId = e.CustomerId;
        customerName = e.CustomerName;
        orderState = OrderState.New;
    }

    public void Apply(OrderCancelled e)
    {
        orderState = OrderState.cancel;
    }

    public void Apply(OrderLineAdded e)
    {
        orderlines.Add(e.OrderLine);
    }

    public void Apply(OrderLineDeleted e)
    {
        orderlines.RemoveAll(ol => ol.Id == e.OrderLineId);
    }

    public IEnumerable<IEvent> Handle(CreateOrder c)
    {
        yield return new OrderCreated()
        {
            Id = c.Id,
            CustomerId = c.CustomerId,
            CustomerName = c.CustomerName,
        };
    }

    public IEnumerable<IEvent> Handle(CancelOrder c)
    {
        yield return new OrderCancelled()
        {
            Id = c.Id,
        };
    }

    public IEnumerable<IEvent> Handle(DeleteOrderLine c)
    {
        yield return new OrderLineDeleted()
        {
            Id = c.Id,
            OrderLineId = c.OrderLineId,
        };
    }

    public IEnumerable<IEvent> Handle(AddOrderLine c)
    {
        yield return new OrderLineAdded()
        {
            Id = c.Id,
            OrderLine = c.OrderLine,
        };
    }

}

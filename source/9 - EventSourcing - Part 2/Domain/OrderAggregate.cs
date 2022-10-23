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
        foreach (var e in events)
        {
            var eventType = e.GetType();

            var method = this.GetType().GetMethod("Apply", new[] { eventType });
            if (method == null)
            {
                throw new InvalidOperationException(string.Format(
                  "Aggregate {0} does not know how to apply event {1}",
                  this.GetType().Name, eventType.Name));
            }

            method.Invoke(this, new object[] { e });
        }
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
    private Guid orderId;
    private int customerId;
    private string customerName;
    private OrderState orderState;
    private List<OrderLine> orderlines = new();

    public OrderAggregate()
    {
    }

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
        //TODO: Command validation

        yield return new OrderCreated()
        {
            Id = c.Id,
            CustomerId = c.CustomerId,
            CustomerName = c.CustomerName
        };
    }

    public IEnumerable<IEvent> Handle(CancelOrder c)
    {
        //TODO: Command validation

        yield return new OrderCancelled()
        {
            Id = c.Id,
        };
    }

    public IEnumerable<IEvent> Handle(DeleteOrderLine c)
    {
        //TODO: Command validation

        if (orderState == OrderState.cancel)
            throw new Exception("Can't modify a cancelled order");

        yield return new OrderLineDeleted()
        {
            Id = c.Id,
            OrderLineId = c.OrderLineId,
        };
    }

    public IEnumerable<IEvent> Handle(AddOrderLine c)
    {
        //TODO: Command validation

        if (orderState == OrderState.cancel)
            throw new Exception("Can't modify a cancelled order");

        yield return new OrderLineAdded()
        {
            Id = c.Id,
            OrderLine = c.OrderLine,
        };
    }
}

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
                              IHandleCommand<AddOrderLine>
{
    public OrderAggregate()
    {

    }

    public IEnumerable<IEvent> Handle(CreateOrder c)
    {
        //var order = new Order()
        //{
        //    Id = c.Id,
        //    CustomerId = c.CustomerId,
        //    CustomerName = c.CustomerName,
        //    orderState = OrderState.New,
        //    orderLines = new List<OrderLine>()
        //};

        //repository.Insert(c.Id, order);
        yield return new OrderCreated()
        {
            Id = c.Id,
            CustomerId = c.CustomerId,
            CustomerName = c.CustomerName,
        };
    }

    public IEnumerable<IEvent> Handle(CancelOrder c)
    {
        //var order = repository.Load(c.Id);

        //order.orderState = OrderState.cancel;

        //repository.Update(c.Id, order);
        yield return new OrderCancelled()
        {
            Id = c.Id,
        };
    }

    public IEnumerable<IEvent> Handle(DeleteOrderLine c)
    {
        //var order = repository.Load(c.Id);

        //var ol = order.orderLines.FirstOrDefault(ol => ol.Id == c.OrderLineId);
        //if (ol != null)
        //    order.orderLines.Remove(ol);

        //repository.Update(c.Id, order);
        yield return new OrderLineDeleted()
        {
            Id = c.Id,
            OrderLineId = c.OrderLineId,
        };
    }

    public IEnumerable<IEvent> Handle(AddOrderLine c)
    {
        //var order = repository.Load(c.Id);

        //order.orderLines.Add(c.OrderLine);

        //repository.Update(c.Id, order);
        yield return new OrderLineAdded()
        {
            Id = c.Id,
            OrderLine = c.OrderLine,
        };
    }

}


public class OrderAggregate : Aggregate,
                              IHandleCommand<CreateOrder>,
                              IHandleCommand<CancelOrder>,
                              IHandleCommand<DeleteOrderLine>,
                              IHandleCommand<AddOrderLine>
{
    public IEnumerable<IEvent> Handle(CreateOrder c) { }

    public IEnumerable<IEvent> Handle(CancelOrder c) { }

    public IEnumerable<IEvent> Handle(DeleteOrderLine c) { }

    public IEnumerable<IEvent> Handle(AddOrderLine c) { }
}
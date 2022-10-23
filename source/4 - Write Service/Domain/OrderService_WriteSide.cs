namespace Domain;

public interface IHandleCommand<in T> where T : ICommand
{
    void Handle(T command);
}

public class OrderService_WriteSide : IHandleCommand<CreateOrder>,
                                      IHandleCommand<CancelOrder>,
                                      IHandleCommand<DeleteOrderLine>,
                                      IHandleCommand<AddOrderLine>

{
    private readonly IOrderRepository repository;

    public OrderService_WriteSide(IOrderRepository repository)
    {
        this.repository = repository;
    }

    public void Handle(CreateOrder c)
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
    }

    public void Handle(CancelOrder c)
    {
        var order = repository.Load(c.Id);

        order.orderState = OrderState.cancel;

        repository.Update(c.Id, order);
    }

    public void Handle(DeleteOrderLine c)
    {
        var order = repository.Load(c.Id);

        var ol = order.orderLines.FirstOrDefault(ol => ol.Id == c.OrderLineId);
        if (ol != null)
            order.orderLines.Remove(ol);

        repository.Update(c.Id, order);
    }

    public void Handle(AddOrderLine c)
    {
        var order = repository.Load(c.Id);

        order.orderLines.Add(c.OrderLine);

        repository.Update(c.Id, order);
    }
}

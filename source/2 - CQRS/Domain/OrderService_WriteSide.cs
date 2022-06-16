namespace Domain;

public class OrderService_WriteSide : IOrderService_WriteSide
{
    private readonly IOrderRepository repository;

    public OrderService_WriteSide(IOrderRepository repository)
    {
        this.repository = repository;
    }

    public void CreateOrder(Guid orderId, int customerId, string customerName)
    {
        var order = new Order()
        {
            Id = orderId,
            CustomerId = customerId,
            CustomerName = customerName,
            orderState = OrderState.New,
            orderLines = new List<OrderLine>()
        };

        repository.Insert(orderId, order);
    }

    public void UpdateOrderState(Guid orderId, OrderState orderState)
    {
        var order = repository.Load(orderId);

        order.orderState = orderState;

        repository.Update(orderId, order);
    }

    public void DeleteOrderLine(Guid orderId, Guid orderLineId)
    {
        var order = repository.Load(orderId);

        var ol = order.orderLines.FirstOrDefault(ol => ol.Id == orderLineId);
        if (ol != null)
            order.orderLines.Remove(ol);

        repository.Update(orderId, order);
    }

    public void AddOrderLine(Guid orderId, OrderLine orderLine)
    {
        var order = repository.Load(orderId);

        orderLine.Id = Guid.NewGuid();
        order.orderLines.Add(orderLine);

        repository.Update(orderId, order);
    }
}

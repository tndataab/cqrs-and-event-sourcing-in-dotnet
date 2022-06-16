namespace Domain;

public class OrderService_ReadSide : IOrderService_ReadSide
{
    private readonly IOrderRepository repository;

    public OrderService_ReadSide(IOrderRepository repository)
    {
        this.repository = repository;
    }

    public Order LoadOrder(Guid orderId)
    {
        var order = repository.Load(orderId);

        order.OrderValue = CalculateOrderValue(order);

        return order;
    }

    public List<OrderSummary> LoadAllOrders()
    {
        var orders = repository.LoadAllOrders();

        var orderSummaryList = orders.Select(o => new OrderSummary()
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            CustomerName = o.CustomerName,
            orderState = o.orderState,
            OrderValue = CalculateOrderValue(o)
        }
        ).ToList();

        return orderSummaryList;

    }

    private decimal CalculateOrderValue(Order order)
    {
        decimal totalValue = 0;
        foreach (var line in order.orderLines)
        {
            totalValue = totalValue + line.Price * line.Quantity;
        }

        return totalValue;
    }
}

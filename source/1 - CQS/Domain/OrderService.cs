namespace Domain;

public interface IOrderService
{
    //Commands
    void CreateOrder(Guid orderId, int customerId, string customerName);
    void UpdateOrderState(Guid orderId, OrderState orderState);
    void DeleteOrderLine(Guid orderId, Guid orderLineId);
    void AddOrderLine(Guid orderId, OrderLine orderLine);
  
    //Queries
    Order LoadOrder(Guid orderId);
    List<OrderSummary> LoadAllOrders();
}

public class OrderService : IOrderService
{
    private readonly IOrderRepository repository;

    public OrderService(IOrderRepository repository)
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

        order.orderLines.Add(orderLine);

        repository.Update(orderId, order);
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

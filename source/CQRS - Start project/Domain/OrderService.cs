namespace Domain;

public interface IOrderService
{
    int CreateOrder(int customerId, string customerName);
    void UpdateOrderState(int orderId, OrderState orderState);
    void DeleteOrderLine(int orderId, int orderLineId);
    void AddOrderLine(int orderId, OrderLine orderLine);
    Order LoadOrder(int orderId);
    List<OrderSummary> LoadAllOrders();
}

public class OrderService : IOrderService
{
    private readonly IOrderRepository repository;

    public OrderService(IOrderRepository repository)
    {
        this.repository = repository;
    }

    public int CreateOrder(int customerId, string customerName)
    {
        var order = new Order()
        {
            CustomerId = customerId,
            CustomerName = customerName,
            orderState = OrderState.New,
            orderLines = new List<OrderLine>()
        };

        return repository.Insert(order);
    }

    public void UpdateOrderState(int orderId, OrderState orderState)
    {
        var order = repository.Load(orderId);

        order.orderState = orderState;

        repository.Update(orderId, order);
    }

    public void DeleteOrderLine(int orderId, int orderLineId)
    {
        var order = repository.Load(orderId);

        var ol = order.orderLines.FirstOrDefault(ol => ol.Id == orderLineId);
        if (ol != null)
            order.orderLines.Remove(ol);

        repository.Update(orderId, order);
    }

    public void AddOrderLine(int orderId, OrderLine orderLine)
    {
        var order = repository.Load(orderId);

        int newOrderLineId = order.orderLines.Count() + 1;
        orderLine.Id = newOrderLineId;

        order.orderLines.Add(orderLine);


        repository.Update(orderId, order);
    }

    public Order LoadOrder(int orderId)
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

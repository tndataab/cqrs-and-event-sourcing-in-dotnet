namespace Domain;

public class Order
{
    public Guid Id { get; set; }
    public OrderState orderState { get; set; } = OrderState.Unknown;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public decimal OrderValue { get; set; }
    public List<OrderLine> orderLines { get; set; } = new();
}

public class OrderLine
{
    public Guid Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public enum OrderState
{
    Unknown,
    New,
    Paid,
    cancel
}

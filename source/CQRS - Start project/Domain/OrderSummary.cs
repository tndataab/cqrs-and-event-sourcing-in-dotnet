namespace Domain;

public class OrderSummary
{
    public int Id { get; set; }
    public OrderState orderState { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public decimal OrderValue { get; set; }
}

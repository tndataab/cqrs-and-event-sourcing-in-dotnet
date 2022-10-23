namespace Domain;

public interface IOrderService_WriteSide
{
    //Commands
    void CreateOrder(Guid orderId, int customerId, string customerName);
    void UpdateOrderState(Guid orderId, OrderState orderState);
    void DeleteOrderLine(Guid orderId, Guid orderLineId);
    void AddOrderLine(Guid orderId, OrderLine orderLine);
}

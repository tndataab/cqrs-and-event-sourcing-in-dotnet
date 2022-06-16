namespace Domain;

public interface IOrderService_WriteSide
{
    //Commands
    void CreateOrder(Guid OrderId, int customerId, string customerName);
    void UpdateOrderState(Guid orderId, OrderState orderState);
    void DeleteOrderLine(Guid orderId, Guid orderLineId);
    void AddOrderLine(Guid orderId, OrderLine orderLine);
}

public interface IOrderService_ReadSide
{
    //Queries
    Order LoadOrder(Guid orderId);
    List<OrderSummary> LoadAllOrders();
}
namespace Domain;

public interface IOrderService_ReadSide
{
    //Queries
    Order LoadOrder(Guid orderId);
    List<OrderSummary> LoadAllOrders();
}

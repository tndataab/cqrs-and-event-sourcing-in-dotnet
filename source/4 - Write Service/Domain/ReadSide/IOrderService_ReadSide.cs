namespace Domain.ReadSide;

public interface IOrderService_ReadSide
{
    //Queries
    Order LoadOrder(Guid orderId);
    List<OrderSummary> LoadAllOrders();
}

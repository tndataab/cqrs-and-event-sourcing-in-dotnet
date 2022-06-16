namespace Domain;

public interface IOrderService_WriteSide
{
    //Commands
}

public interface IOrderService_ReadSide
{
    //Queries
    Order LoadOrder(Guid orderId);
    List<OrderSummary> LoadAllOrders();
}
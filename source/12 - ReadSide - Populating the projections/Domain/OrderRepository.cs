//namespace Domain;


//public interface IOrderRepository
//{
//    //Commands
//    void Insert(Guid OrderId, Order order);
//    void Update(Guid orderId, Order order);
    
//    //Queries
//    Order Load(Guid orderId);
//    List<Order> LoadAllOrders();
//}

//public class OrderRepository : IOrderRepository
//{
//    private Dictionary<Guid, Order> ordersDb = new();

//    public OrderRepository()
//    {
//        var order1 = new Order()
//        {
//            Id = Guid.NewGuid(),
//            CustomerId = 1001,
//            CustomerName = "Bob",
//            orderState = OrderState.New,
//            orderLines = new()
//            {
//                new OrderLine()
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "Widget A",
//                    ProductId = 101,
//                    Price = 5.95m,
//                    Quantity = 10
//                },
//                new OrderLine()
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "MegaMax B",
//                    ProductId = 102,
//                    Price = 24.95m,
//                    Quantity = 1
//                }
//            }
//        };

//        var order2 = new Order()
//        {
//            Id = Guid.NewGuid(),
//            CustomerId = 1002,
//            CustomerName = "Alice",
//            orderState = OrderState.Paid,
//            orderLines = new()
//            {
//                new OrderLine()
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "SuperMax",
//                    ProductId = 103,
//                    Price = 85.95m,
//                    Quantity = 2
//                },
//                new OrderLine()
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "MiniMax",
//                    ProductId = 104,
//                    Price = 19.95m,
//                    Quantity = 3
//                }
//            }
//        };

//        var order3 = new Order()
//        {
//            Id = Guid.NewGuid(),
//            CustomerId = 1003,
//            CustomerName = "Joe",
//            orderState = OrderState.cancel,
//            orderLines = new()
//            {
//                new OrderLine()
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "SuperMax",
//                    ProductId = 103,
//                    Price = 85.95m,
//                    Quantity = 2
//                },
//                new OrderLine()
//                {
//                    Id = Guid.NewGuid(),
//                    Name = "MiniMax",
//                    ProductId = 106,
//                    Price = 29.95m,
//                    Quantity = 1
//                }
//            }
//        };

//        ordersDb.Add(order1.Id, order1);
//        ordersDb.Add(order2.Id, order2);
//        ordersDb.Add(order3.Id, order3);
//    }

//    public void Insert(Guid orderId, Order order)
//    { 
//        order.Id = orderId;

//        ordersDb.Add(orderId, order);
//    }

//    public Order Load(Guid orderId)
//    {
//        return ordersDb[orderId];
//    }

//    public List<Order> LoadAllOrders()
//    {
//        return ordersDb.Values.ToList();
//    }

//    public void Update(Guid orderId, Order order)
//    {
//        ordersDb[orderId] = order;
//    }
//}

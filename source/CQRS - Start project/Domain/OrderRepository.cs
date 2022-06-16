namespace Domain;


public interface IOrderRepository
{
    int Insert(Order order);
    void Update(int orderId, Order order);
    Order Load(int orderId);
    List<Order> LoadAllOrders();
}

public class OrderRepository : IOrderRepository
{
    private Dictionary<int, Order> ordersDb = new();

    public OrderRepository()
    {
        var order1 = new Order()
        {
            Id = 1,
            CustomerId = 1001,
            CustomerName = "Bob",
            orderState = OrderState.New,
            orderLines = new()
            {
                new OrderLine()
                {
                    Id = 1,
                    Name = "Widget A",
                    ProductId = 101,
                    Price = 5.95m,
                    Quantity = 10
                },
                new OrderLine()
                {
                    Id = 2,
                    Name = "MegaMax B",
                    ProductId = 102,
                    Price = 24.95m,
                    Quantity = 1
                }
            }
        };

        var order2 = new Order()
        {
            Id = 2,
            CustomerId = 1002,
            CustomerName = "Alice",
            orderState = OrderState.Paid,
            orderLines = new()
            {
                new OrderLine()
                {
                    Id = 3,
                    Name = "SuperMax",
                    ProductId = 103,
                    Price = 85.95m,
                    Quantity = 2
                },
                new OrderLine()
                {
                    Id = 4,
                    Name = "MiniMax",
                    ProductId = 104,
                    Price = 19.95m,
                    Quantity = 3
                }
            }
        };

        var order3 = new Order()
        {
            Id = 3,
            CustomerId = 1003,
            CustomerName = "Joe",
            orderState = OrderState.cancel,
            orderLines = new()
            {
                new OrderLine()
                {
                    Id = 5,
                    Name = "SuperMax",
                    ProductId = 103,
                    Price = 85.95m,
                    Quantity = 2
                },
                new OrderLine()
                {
                    Id = 6,
                    Name = "MiniMax",
                    ProductId = 106,
                    Price = 29.95m,
                    Quantity = 1
                }
            }
        };

        ordersDb.Add(order1.Id, order1);
        ordersDb.Add(order2.Id, order2);
        ordersDb.Add(order3.Id, order3);
    }

    public int Insert(Order order)
    {
        int newOrderId = ordersDb.Count() + 1;
        order.Id = newOrderId;

        ordersDb.Add(newOrderId, order);

        return newOrderId;
    }

    public Order Load(int orderId)
    {
        return ordersDb[orderId];
    }

    public List<Order> LoadAllOrders()
    {
        return ordersDb.Values.ToList();
    }

    public void Update(int orderId, Order order)
    {
        ordersDb[orderId] = order;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ReadSide
{
public abstract class Projection
{

}

    public interface IBuildFrom<TEvent>
    {
        void Apply(TEvent e);
    }

    public class OrderListProjection :  Projection,
                                        IBuildFrom<OrderCreated>,
                                        IBuildFrom<OrderCancelled>,
                                        IBuildFrom<OrderLineAdded>,
                                        IBuildFrom<OrderLineDeleted>
    {
        private class OrderDetails
        {
            public Guid Id { get; set; }
            public OrderState orderState { get; set; }
            public int CustomerId { get; set; }
            public string CustomerName { get; set; }
            public decimal OrderValue { get; set; }
            public List<OrderLine> orderLines { get; set; }
        }

        private Dictionary<Guid, OrderDetails> orderDict = new();

        public void Apply(OrderCreated e)
        {
            orderDict.Add(e.Id, new OrderDetails()
            {
                Id = e.Id,
                CustomerId = e.CustomerId,
                CustomerName = e.CustomerName,
                orderState = OrderState.New,
                OrderValue = 0,
                orderLines = new List<OrderLine>()
            });

        }

        public void Apply(OrderCancelled e)
        {
            orderDict[e.Id].orderState = OrderState.cancel;
        }

        public void Apply(OrderLineAdded e)
        {
            orderDict[e.Id].orderLines.Add(e.OrderLine);
            orderDict[e.Id].OrderValue = CalculateOrderValue(orderDict[e.Id]);

        }

        public void Apply(OrderLineDeleted e)
        {
            orderDict[e.Id].orderLines.RemoveAll(ol => ol.Id == e.Id);
            orderDict[e.Id].OrderValue = CalculateOrderValue(orderDict[e.Id]);
        }

        private decimal CalculateOrderValue(OrderDetails order)
        {
            decimal totalValue = 0;
            foreach (var line in order.orderLines)
            {
                totalValue = totalValue + line.Price * line.Quantity;
            }

            return totalValue;
        }



        public List<OrderSummary> GetOrdersummaryList()
        {
            return orderDict.Values.Select(o =>
                    new OrderSummary()
                    {
                        Id = o.Id,
                        orderState = o.orderState,
                        CustomerId = o.CustomerId,
                        CustomerName = o.CustomerName,
                        OrderValue = o.OrderValue,
                    }
            ).ToList();
        }

    }
}

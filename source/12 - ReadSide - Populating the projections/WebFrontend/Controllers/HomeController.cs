using Domain;
using Domain.ReadSide;
using Domain.WriteSide;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebFrontend.Models;

namespace WebFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWriteService writeService;
        private readonly IReadService readService;

        public HomeController(ILogger<HomeController> logger,
                              IWriteService writeService,
                              IReadService readService)
        {
            _logger = logger;
            this.writeService = writeService;
            this.readService = readService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            //Generate sample test orders

            var result1 = (RedirectToActionResult)CreateNewOrder(1, "Tore");
            var orderid1 = Guid.Parse(result1.RouteValues["id"].ToString());

            AddOrderLine(orderid1, new OrderLine()
            {
                Id = Guid.NewGuid(),
                Name = "Widget 1",
                Price = 10,
                Quantity = 10,
                ProductId = 1
            });
            AddOrderLine(orderid1, new OrderLine()
            {
                Id = Guid.NewGuid(),
                Name = "Widget 2",
                Price = 100,
                Quantity = 1,
                ProductId = 2
            });

            AddOrderLine(orderid1, new OrderLine()
            {
                Id = Guid.NewGuid(),
                Name = "Widget 3",
                Price = 1,
                Quantity = 100,
                ProductId = 3
            });

            var result2 = (RedirectToActionResult)CreateNewOrder(1, "Tore");
            var orderid2 = Guid.Parse(result2.RouteValues["id"].ToString());

            AddOrderLine(orderid2, new OrderLine()
            {
                Id = Guid.NewGuid(),
                Name = "MegaMax 1",
                Price = 10,
                Quantity = 100,
                ProductId = 4
            });
            AddOrderLine(orderid2, new OrderLine()
            {
                Id = Guid.NewGuid(),
                Name = "MegaMax 2",
                Price = 1000,
                Quantity = 1,
                ProductId = 5
            });

            AddOrderLine(orderid2, new OrderLine()
            {
                Id = Guid.NewGuid(),
                Name = "MegaMax 3",
                Price = 1,
                Quantity = 1000,
                ProductId = 6
            });

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CreateNewOrder()
        {
            return View(new CreateOrderModel());
        }

        [HttpPost]
        public IActionResult CreateNewOrder(int CustomerId, string CustomerName)
        {
            var orderId = Guid.NewGuid();

            writeService.HandleCommand(new CreateOrder()
            {
                Id = orderId,
                CustomerId = CustomerId,
                CustomerName = CustomerName
            });

            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        [HttpPost]
        public IActionResult CancelOrder(Guid OrderId)
        {
            writeService.HandleCommand(new CancelOrder()
            {
                Id = OrderId
            });

            return RedirectToAction("OrderDetails", new { id = OrderId });
        }

        [HttpPost]
        public IActionResult DeleteOrderLine(Guid orderId, Guid orderLineId)
        {
            writeService.HandleCommand(new DeleteOrderLine()
            {
                Id = orderId,
                OrderLineId = orderLineId
            });

            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        [HttpPost]
        public IActionResult AddOrderLine(Guid orderId, OrderLine orderLine)
        {
            orderLine.Id = Guid.NewGuid();

            writeService.HandleCommand(new AddOrderLine()
            {
                Id = orderId,
                OrderLine = orderLine
            });

            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        public IActionResult OrderDetails(Guid id)
        {
            var order = writeService.QueryAggregate<OrderAggregate, Order>(id, agg =>
            {
                return agg.GetOrderQuery();
            });

            return View(order);
        }

        public IActionResult ListAllOrders()
        {
            var orders = readService.Query<OrderListProjection, List<OrderSummary>>(proj =>
            {
                return proj.GetOrderSummaryList();
            });

            return View(orders);
        }
    }
}

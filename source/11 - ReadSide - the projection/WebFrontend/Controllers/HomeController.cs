using Domain;
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

        public HomeController(ILogger<HomeController> logger,
                              IWriteService writeService)
        {
            _logger = logger;
            this.writeService = writeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
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
            //var orders = orderService_ReadSide.LoadAllOrders();
            var orders = new List<OrderSummary>();
            return View(orders);
        }
    }
}

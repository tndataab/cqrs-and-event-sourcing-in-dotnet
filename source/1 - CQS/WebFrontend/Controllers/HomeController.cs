using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebFrontend.Models;

namespace WebFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOrderService orderService;

        public HomeController(ILogger<HomeController> logger, IOrderService orderService)
        {
            _logger = logger;
            this.orderService = orderService;
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

        public IActionResult DeleteOrderLine(Guid orderId, Guid orderLineId)
        {
            orderService.DeleteOrderLine(orderId, orderLineId);

            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        public IActionResult CreateNewOrder()
        {
            return View(new CreateOrderModel());
        }

        [HttpPost]
        public IActionResult CreateNewOrder(int CustomerId, string CustomerName)
        {
            var orderId = Guid.NewGuid();
            orderService.CreateOrder(orderId, CustomerId, CustomerName);

            return RedirectToAction("OrderDetails", new { id = orderId });
        }


        [HttpPost]
        public IActionResult AddOrderLine(Guid orderId, OrderLine orderline)
        {
            orderService.AddOrderLine(orderId, orderline);

            return RedirectToAction("OrderDetails", new { id = orderId });
        }


        public IActionResult ListAllOrders()
        {
            var orders = orderService.LoadAllOrders();

            return View(orders);
        }

        public IActionResult OrderDetails(Guid id)
        {
            var order = orderService.LoadOrder(id);

            return View(order);
        }

        [HttpPost]
        public IActionResult CancelOrder(Guid OrderId)
        {
            orderService.UpdateOrderState(OrderId, OrderState.cancel);

            return RedirectToAction("OrderDetails", new { id = OrderId });
        }
    }
}

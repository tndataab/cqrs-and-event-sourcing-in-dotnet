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

        public IActionResult DeleteOrderLine(int orderId, int orderLineId)
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
            int orderId = orderService.CreateOrder(CustomerId, CustomerName);

            return RedirectToAction("OrderDetails", new { id = orderId });
        }


        [HttpPost]
        public IActionResult AddOrderLine(int orderId, OrderLine orderline)
        {
            orderService.AddOrderLine(orderId, orderline);

            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        [HttpPost]
        public IActionResult CancelOrder(int OrderId)
        {
            orderService.UpdateOrderState(OrderId, OrderState.cancel);

            return RedirectToAction("OrderDetails", new { id = OrderId });
        }

        public IActionResult ListAllOrders()
        {
            var orders = orderService.LoadAllOrders();

            return View(orders);
        }

        public IActionResult OrderDetails(int id)
        {
            var order = orderService.LoadOrder(id);

            return View(order);
        }
    }
}

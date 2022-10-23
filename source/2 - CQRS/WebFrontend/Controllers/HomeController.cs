using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebFrontend.Models;

namespace WebFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOrderService_WriteSide orderService_WriteSide;
        private readonly IOrderService_ReadSide orderService_ReadSide;

        public HomeController(ILogger<HomeController> logger,
                              IOrderService_WriteSide orderService_WriteSide,
                              IOrderService_ReadSide orderService_ReadSide)
        {
            _logger = logger;
            this.orderService_WriteSide = orderService_WriteSide;
            this.orderService_ReadSide = orderService_ReadSide;
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
            orderService_WriteSide.CreateOrder(orderId, CustomerId, CustomerName);

            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        [HttpPost]
        public IActionResult CancelOrder(Guid OrderId)
        {
            orderService_WriteSide.UpdateOrderState(OrderId, OrderState.cancel);

            return RedirectToAction("OrderDetails", new { id = OrderId });
        }

        public IActionResult DeleteOrderLine(Guid orderId, Guid orderLineId)
        {
            orderService_WriteSide.DeleteOrderLine(orderId, orderLineId);

            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        [HttpPost]
        public IActionResult AddOrderLine(Guid orderId, OrderLine orderline)
        {
            orderline.Id = Guid.NewGuid();
            orderService_WriteSide.AddOrderLine(orderId, orderline);

            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        public IActionResult OrderDetails(Guid id)
        {
            var order = orderService_ReadSide.LoadOrder(id);

            return View(order);
        }

        public IActionResult ListAllOrders()
        {
            var orders = orderService_ReadSide.LoadAllOrders();

            return View(orders);
        }
    }
}

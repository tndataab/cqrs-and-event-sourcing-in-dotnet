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
            var writeside = (IHandleCommand<CreateOrder>)orderService_WriteSide;

            var orderId = Guid.NewGuid();

            writeside.Handle(new CreateOrder()
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
            var writeside = (IHandleCommand<CancelOrder>)orderService_WriteSide;

            writeside.Handle(new CancelOrder()
            {
                Id = OrderId
            });

            return RedirectToAction("OrderDetails", new { id = OrderId });
        }

        public IActionResult DeleteOrderLine(Guid orderId, Guid orderLineId)
        {
            var writeside = (IHandleCommand<DeleteOrderLine>)orderService_WriteSide;
            writeside.Handle(new DeleteOrderLine()
            {
                Id = orderId,
                OrderLineId = orderLineId
            });

            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        [HttpPost]
        public IActionResult AddOrderLine(Guid orderId, OrderLine orderline)
        {
            orderline.Id = Guid.NewGuid();

            var writeside = (IHandleCommand<AddOrderLine>)orderService_WriteSide;

            writeside.Handle(new AddOrderLine()
            {
                Id = orderId,
                OrderLine = orderline
            });

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

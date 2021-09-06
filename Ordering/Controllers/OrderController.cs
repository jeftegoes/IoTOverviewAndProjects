using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ordering.Persistence;

namespace Ordering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var data = await _orderRepository.GetOrdersAsync();
            
            return Ok(data);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId)
        {
            var order = await _orderRepository.GetOrderAsync(Guid.Parse(orderId));
            
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}
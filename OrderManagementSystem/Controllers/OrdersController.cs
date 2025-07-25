using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderServices _orderService;
        private readonly OrderManagementDbContext _context;
        public OrdersController(OrderServices orderService, OrderManagementDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            try
            {
                var created = await _orderService.CreateOrderAsync(order);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            return order == null ? NotFound() : Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Orders.Include(o => o.OrderItems).ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] string status)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, status);
                return Ok("Order status updated");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

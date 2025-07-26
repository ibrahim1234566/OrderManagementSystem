using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.OrderDTO;
using OrderManagementSystem.Repositories.Repository;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderServices _orderService;
        private readonly OrderRepository _orderRepo;

        public OrdersController(OrderServices orderService, OrderRepository orderRepo)
        {
            _orderService = orderService;
            _orderRepo = orderRepo;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderDto orderDto)
        {
            try
            {
                var created = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetById), new { orderId = created.OrderId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            return order == null ? NotFound() : Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _orderRepo.GetAllAsync());
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

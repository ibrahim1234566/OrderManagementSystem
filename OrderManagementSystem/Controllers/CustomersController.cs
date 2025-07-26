using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Repository;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerRepository _customerRepo;
        private readonly OrderRepository _orderRepo;

        public CustomersController(CustomerRepository customerRepo, OrderRepository orderRepo)
        {
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string name, [FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Name and Email are required.");
            }

            var newCustomer = new Customer
            {
                Name = name,
                Email = email
            };

            await _customerRepo.AddAsync(newCustomer);
            await _customerRepo.SaveChangesAsync();

            return Ok(newCustomer);
        }

        [HttpGet("{customerId}/orders")]
        public async Task<IActionResult> GetCustomerOrders(int customerId)
        {
            var orders = await _orderRepo.FindAsync(o => o.CustomerId == customerId);
            return Ok(orders);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Repository;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _productRepo;

        public ProductsController(ProductRepository productRepo) => _productRepo = productRepo;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _productRepo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(
     [FromQuery] string name,
     [FromQuery] decimal price,
     [FromQuery] int stock)
        {
            if (string.IsNullOrWhiteSpace(name) || price <= 0 || stock < 0)
            {
                return BadRequest("Name is required, price must be > 0, and stock cannot be negative.");
            }

            var newProduct = new Product
            {
                Name = name,
                Price = price,
                Stock = stock
            };

            await _productRepo.AddAsync(newProduct);
            await _productRepo.SaveChangesAsync();

            return Ok(newProduct);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.ProductId) return BadRequest();
            await _productRepo.UpdateAsync(product);
            await _productRepo.SaveChangesAsync();
            return Ok(product);
        }
    }
}

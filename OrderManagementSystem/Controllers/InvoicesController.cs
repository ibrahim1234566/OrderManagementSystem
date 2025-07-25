using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly OrderManagementDbContext _context;
        public InvoicesController(OrderManagementDbContext context) => _context = context;

        [Authorize(Roles = "Admin")]
        [HttpGet("{invoiceId}")]
        public async Task<IActionResult> Get(int invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            return invoice == null ? NotFound() : Ok(invoice);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Invoices.ToListAsync());
        }
    }
}

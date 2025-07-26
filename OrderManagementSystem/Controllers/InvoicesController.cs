using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Repositories.Repository;

namespace OrderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceRepository _invoiceRepo;

        public InvoicesController(InvoiceRepository invoiceRepo)
        {
            _invoiceRepo = invoiceRepo;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{invoiceId}")]
        public async Task<IActionResult> Get(int invoiceId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);
            return invoice == null ? NotFound() : Ok(invoice);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _invoiceRepo.GetAllAsync());
        }
    }
}

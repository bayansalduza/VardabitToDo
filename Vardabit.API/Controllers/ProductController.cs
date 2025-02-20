using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vardabit.Domain.DTOs;
using Vardabit.Domain.Models;
using Vardabit.Service.Interfaces;

namespace Vardabit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();

            return Ok(products);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null) 
                return NotFound("Ürün bulunamadı.");

            return Ok(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var product = new Product
            {
                Name = dto.Name,
                Code = dto.Code,
                Amount = dto.Amount,
                CategoryId = dto.CategoryId
            };

            await _productService.AddAsync(product);

            return StatusCode(201);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id) 
                return BadRequest("ID eşleşmedi.");

            var product = new Product
            {
                Id = dto.Id,
                Name = dto.Name,
                Code = dto.Code,
                Amount = dto.Amount,
                CategoryId = dto.CategoryId
            };

            await _productService.UpdateAsync(product);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);

            return Ok();
        }
    }
}

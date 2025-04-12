using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DapperSRP.Common;
using DapperSRP.Service.Interface;
using Asp.Versioning;
using DapperSRP.Dto.Product.CreateProduct.Request;
using DapperSRP.Dto.Product.UpdateProduct.Request;
using DapperSRP.Dto.GetProductPaginated.Request;

namespace DapperSRP.WebApi.Controllers
{
    [ApiVersion("1")]
    public class ProductController : VersionApiController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.SuperAdmin}, {Roles.Admin}")]
        public async Task<IActionResult> CreateProduct(CreateProductRequest request)
        {
            var response = await _productService.AddAsync(request);
            return CreatedAtAction(nameof(GetProduct), new { id = response.Result }, response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProduct(int id)
        {
            var response = await _productService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _productService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("GetPaged")]
        [Authorize]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryRequest request)
        {
            var response = await _productService.GetPagedAsync(request);
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.SuperAdmin}, {Roles.Admin}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request)
        {
            var response = await _productService.UpdateAsync(id, request);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Roles.SuperAdmin}, {Roles.Admin}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteAsync(id);
            return Ok(response);
        }
    }
}

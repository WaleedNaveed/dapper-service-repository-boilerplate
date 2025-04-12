using Moq;
using Microsoft.AspNetCore.Mvc;
using DapperSRP.WebApi.Controllers;
using DapperSRP.Service.Interface;
using DapperSRP.Service.Exceptions;
using DapperSRP.Service;
using DapperSRP.Dto.Pagination;
using DapperSRP.Dto.Product.GetProduct.Response;
using DapperSRP.Dto.Product.UpdateProduct.Response;
using DapperSRP.Dto.GetProductPaginated.Response;
using DapperSRP.Dto.Product.CreateProduct.Request;
using DapperSRP.Dto.GetProductPaginated.Request;
using DapperSRP.Dto.Product.UpdateProduct.Request;

namespace DapperSRP.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductController _productController;

        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _productController = new ProductController(_mockProductService.Object);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtActionResult_WhenProductIsCreated()
        {
            // Arrange
            var request = new CreateProductRequest { Name = "Test Product", Price = 100 };
            var response = new ServiceResponse<int>
            {
                HasError = false,
                Result = 1
            };

            _mockProductService.Setup(service => service.AddAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _productController.CreateProduct(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(nameof(_productController.GetProduct), createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues["id"]);

            var returnValue = Assert.IsType<ServiceResponse<int>>(createdResult.Value);
            Assert.False(returnValue.HasError);
            Assert.Equal(1, returnValue.Result);
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResult_WhenProductExists()
        {
            // Arrange
            var response = new ServiceResponse<GetProductResponse>
            {
                HasError = false,
                Result = new GetProductResponse { Id = 1, Name = "Test Product" }
            };

            _mockProductService.Setup(service => service.GetByIdAsync(1)).ReturnsAsync(response);

            // Act
            var result = await _productController.GetProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ServiceResponse<GetProductResponse>>(okResult.Value);
            Assert.False(returnValue.HasError);
            Assert.Equal(1, returnValue.Result.Id);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsOkResult_WhenProductIsDeleted()
        {
            // Arrange
            var response = new ServiceResponse<bool> { HasError = false, Result = true };

            _mockProductService.Setup(service => service.DeleteAsync(1)).ReturnsAsync(response);

            // Act
            var result = await _productController.DeleteProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ServiceResponse<bool>>(okResult.Value);
            Assert.False(returnValue.HasError);
            Assert.True(returnValue.Result);
        }

        #region GetPagedAsync Tests

        [Fact]
        public async Task GetProducts_ShouldReturnPagedProducts()
        {
            // Arrange
            var request = new ProductQueryRequest { Page = 1, PageSize = 10 };
            var response = new ServiceResponse<PaginationResponse<ProductQueryResponse>>
            {
                HasError = false,
                Result = new PaginationResponse<ProductQueryResponse>
                {
                    Items = new List<ProductQueryResponse>
                    {
                        new ProductQueryResponse { Id = 1, Name = "Product1" },
                        new ProductQueryResponse { Id = 2, Name = "Product2" }
                    },
                    TotalCount = 2,
                    Page = 1,
                    PageSize = 10
                }
            };

            _mockProductService.Setup(s => s.GetPagedAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _productController.GetProducts(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var resultValue = result.Value as ServiceResponse<PaginationResponse<ProductQueryResponse>>;
            Assert.NotNull(resultValue);
            Assert.False(resultValue.HasError);
        }

        [Fact]
        public async Task GetProducts_ShouldReturnEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var request = new ProductQueryRequest { Page = 1, PageSize = 10 };
            var response = new ServiceResponse<PaginationResponse<ProductQueryResponse>>
            {
                HasError = false,
                Result = new PaginationResponse<ProductQueryResponse>
                {
                    Items = new List<ProductQueryResponse>(),
                    TotalCount = 0,
                    Page = 1,
                    PageSize = 10
                }
            };

            _mockProductService.Setup(s => s.GetPagedAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _productController.GetProducts(request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var resultValue = result.Value as ServiceResponse<PaginationResponse<ProductQueryResponse>>;
            Assert.NotNull(resultValue);
            Assert.False(resultValue.HasError);
            Assert.Empty(resultValue.Result.Items);
        }

        #endregion

        #region UpdateProduct Tests

        [Fact]
        public async Task UpdateProduct_ShouldReturnUpdatedProduct()
        {
            // Arrange
            int productId = 1;
            var request = new UpdateProductRequest { Name = "Updated Product" };
            var response = new ServiceResponse<UpdateProductResponse>
            {
                HasError = false,
                Result = new UpdateProductResponse { Id = productId, Name = "Updated Product", UpdatedBy = "Admin" }
            };

            _mockProductService.Setup(s => s.UpdateAsync(productId, request)).ReturnsAsync(response);

            // Act
            var result = await _productController.UpdateProduct(productId, request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var resultValue = result.Value as ServiceResponse<UpdateProductResponse>;
            Assert.NotNull(resultValue);
            Assert.False(resultValue.HasError);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;
            var request = new UpdateProductRequest { Name = "Updated Product" };

            _mockProductService.Setup(s => s.UpdateAsync(productId, request))
                               .ThrowsAsync(new NotFoundException("Product does not exist"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productController.UpdateProduct(productId, request));
        }

        #endregion
    }
}

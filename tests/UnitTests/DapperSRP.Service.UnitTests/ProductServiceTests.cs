using AutoMapper;
using DapperSRP.Dto.Product.UpdateProduct.Request;
using DapperSRP.Dto.Product.UpdateProduct.Response;
using DapperSRP.Persistence.Models;
using DapperSRP.Repository.Interface;
using DapperSRP.Service.Exceptions;
using DapperSRP.Service.Interface;
using DapperSRP.Service.Service;
using Moq;

namespace DapperSRP.Service.UnitTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ICommonService> _mockCommonService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCommonService = new Mock<ICommonService>();
            _mockMapper = new Mock<IMapper>();

            _productService = new ProductService(
                _mockProductRepository.Object,
                _mockUserRepository.Object,
                _mockMapper.Object,
                _mockCommonService.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockProductRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetByIdAsync(999));
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenProductDeleted()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1", Price = 100 };

            _mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mockProductRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(1);

            // Act
            var response = await _productService.DeleteAsync(1);

            // Assert
            Assert.False(response.HasError);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockProductRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.DeleteAsync(999));
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedProduct_WhenProductExists()
        {
            // Arrange
            var request = new UpdateProductRequest { Name = "Updated Product", Price = 150 };
            var product = new Product { Id = 1, Name = "Old Product", Price = 100 };
            var mappedProduct = new UpdateProductResponse { Id = 1, Name = "Updated Product", Price = 150 };

            _mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<UpdateProductResponse>(It.IsAny<Product>())).Returns(mappedProduct);

            // Act
            var response = await _productService.UpdateAsync(1, request);

            // Assert
            Assert.False(response.HasError);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockProductRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.UpdateAsync(999, new UpdateProductRequest()));
        }
    }
}

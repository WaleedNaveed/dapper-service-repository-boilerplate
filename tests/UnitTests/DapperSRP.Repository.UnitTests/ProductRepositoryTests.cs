using Moq;
using DapperSRP.Persistence.Models;
using DapperSRP.Persistence.Models.Custom.Product;
using DapperSRP.Repository.Interface;
using DapperSRP.Repository.Repository;

namespace DapperSRP.Repository.UnitTests
{
    public class ProductRepositoryTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ProductRepository _productRepository;

        public ProductRepositoryTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _productRepository = new ProductRepository(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnCorrectPagedResults()
        {
            // Arrange
            var query = new ProductQueryRequest { Page = 1, PageSize = 10, Search = "Test" };
            var expectedProducts = new List<ProductQueryResponse>
            {
                new ProductQueryResponse { Id = 1, Name = "Test Product 1" },
                new ProductQueryResponse { Id = 2, Name = "Test Product 2" }
            };
            var totalCount = 20;

            // Mock the QueryAsync and QuerySingleOrDefaultAsync methods
            _mockUnitOfWork.Setup(uow => uow.QueryAsync<ProductQueryResponse>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(expectedProducts);
            _mockUnitOfWork.Setup(uow => uow.QuerySingleOrDefaultAsync<int>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(totalCount);

            // Act
            var result = await _productRepository.GetPagedAsync(query);

            // Assert
            Assert.Equal(expectedProducts, result.Item1);
            Assert.Equal(totalCount, result.Item2);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var expectedProduct = new Product { Id = 1, Name = "Test Product" };

            // Mock the QuerySingleOrDefaultAsync method
            _mockUnitOfWork.Setup(uow => uow.QuerySingleOrDefaultAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(expectedProduct);

            // Act
            var result = await _productRepository.GetByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct.Id, result?.Id);
            Assert.Equal(expectedProduct.Name, result?.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;

            // Mock the QuerySingleOrDefaultAsync method to return null
            _mockUnitOfWork.Setup(uow => uow.QuerySingleOrDefaultAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync((Product?)null);

            // Act
            var result = await _productRepository.GetByIdAsync(productId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnAffectedRows_WhenProductIsUpdated()
        {
            // Arrange
            var updatedProduct = new Product { Id = 1, Name = "Updated Product" };
            var affectedRows = 1;

            // Mock the ExecuteAsync method to return the affected rows count
            _mockUnitOfWork.Setup(uow => uow.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(affectedRows);

            // Act
            var result = await _productRepository.UpdateAsync(updatedProduct);

            // Assert
            Assert.Equal(affectedRows, result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnAffectedRows_WhenProductIsDeleted()
        {
            // Arrange
            var productId = 1;
            var affectedRows = 1;

            // Mock the ExecuteAsync method to return the affected rows count
            _mockUnitOfWork.Setup(uow => uow.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(affectedRows);

            // Act
            var result = await _productRepository.DeleteAsync(productId);

            // Assert
            Assert.Equal(affectedRows, result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts_WhenProductsExist()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1" },
                new Product { Id = 2, Name = "Product 2" }
            };

            // Mock the QueryAsync method to return the expected products
            _mockUnitOfWork.Setup(uow => uow.QueryAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProducts.Count, result.Count());
            Assert.Equal(expectedProducts[0].Name, result.First().Name);
            Assert.Equal(expectedProducts[1].Name, result.Last().Name);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var expectedProducts = new List<Product>();

            // Mock the QueryAsync method to return an empty list
            _mockUnitOfWork.Setup(uow => uow.QueryAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                           .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productRepository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}

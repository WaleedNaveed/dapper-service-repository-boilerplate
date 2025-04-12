using DapperSRP.Persistence.Models;
using DapperSRP.Persistence.Models.Custom.Product;

namespace DapperSRP.Repository.Interface
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<(IEnumerable<ProductQueryResponse>, int)> GetPagedAsync(ProductQueryRequest query);
        Task<int> GetNewProductsCountAsync(DateTime date);
    }
}

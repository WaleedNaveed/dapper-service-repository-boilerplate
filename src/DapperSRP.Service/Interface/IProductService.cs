using DapperSRP.Dto.GetProductPaginated.Request;
using DapperSRP.Dto.GetProductPaginated.Response;
using DapperSRP.Dto.Pagination;
using DapperSRP.Dto.Product.CreateProduct.Request;
using DapperSRP.Dto.Product.GetProduct.Response;
using DapperSRP.Dto.Product.UpdateProduct.Request;
using DapperSRP.Dto.Product.UpdateProduct.Response;

namespace DapperSRP.Service.Interface
{
    public interface IProductService
    {
        Task<ServiceResponse<int>> AddAsync(CreateProductRequest request);
        Task<ServiceResponse<GetProductResponse>> GetByIdAsync(int id);
        Task<ServiceResponse<IEnumerable<GetProductResponse>>> GetAllAsync();
        Task<ServiceResponse<PaginationResponse<ProductQueryResponse>>> GetPagedAsync(ProductQueryRequest request);
        Task<ServiceResponse<UpdateProductResponse>> UpdateAsync(int Id, UpdateProductRequest request);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
        Task<ServiceResponse<int>> GetNewProductsCountAsync(DateTime date);
    }
}

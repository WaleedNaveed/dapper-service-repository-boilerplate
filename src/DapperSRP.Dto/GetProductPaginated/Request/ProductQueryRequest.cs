using DapperSRP.Dto.Pagination;

namespace DapperSRP.Dto.GetProductPaginated.Request
{
    public class ProductQueryRequest : PaginationRequest
    {
        public string? Search { get; set; }
    }
}

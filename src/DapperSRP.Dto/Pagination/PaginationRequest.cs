using DapperSRP.Common;

namespace DapperSRP.Dto.Pagination
{
    public class PaginationRequest
    {
        public int Page { get; set; } = PaginationConstants.DefaultPage;
        public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
    }
}

using DapperSRP.Common;

namespace DapperSRP.Persistence.Models.Custom
{
    public class PaginationRequest
    {
        public int Page { get; set; } = PaginationConstants.DefaultPage;
        public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
    }
}

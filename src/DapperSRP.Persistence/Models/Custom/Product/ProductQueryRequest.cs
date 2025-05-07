namespace DapperSRP.Persistence.Models.Custom.Product
{
    public class ProductQueryRequest : PaginationRequest
    {
        public string? Search { get; set; }
    }
}

using Dapper;
using DapperSRP.Persistence.Models;
using DapperSRP.Persistence.Models.Custom.Product;
using DapperSRP.Repository.Interface;

namespace DapperSRP.Repository.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IEnumerable<ProductQueryResponse>, int)> GetPagedAsync(ProductQueryRequest query)
        {
            var whereClause = "WHERE 1=1";
            var parameters = new DynamicParameters();

            // Filtering by Name
            if (!string.IsNullOrEmpty(query.Search))
            {
                whereClause += " AND p.Name LIKE @Search";
                parameters.Add("Search", $"%{query.Search}%");
            }

            // Pagination Calculation
            var offset = (query.Page - 1) * query.PageSize;

            // Query for paginated data
            var sql = $@"SELECT p.*, u1.Name AS CreatedBy, u2.Name AS UpdatedBy
                            FROM Products p
                            LEFT JOIN Users u1 ON p.CreatedBy = u1.Id
                            LEFT JOIN Users u2 ON p.UpdatedBy = u2.Id
                            {whereClause}
                            ORDER BY p.Id DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var sqlForCount = $@"SELECT COUNT(*) FROM Products p {whereClause};";

            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await _unitOfWork.QueryAsync<ProductQueryResponse>(sql, parameters);
            var totalCount = await _unitOfWork.QuerySingleOrDefaultAsync<int>(sqlForCount, parameters);

            return (items, totalCount);
        }

        public async Task<int> GetNewProductsCountAsync(DateTime date)
        {
            var query = "SELECT COUNT(*) FROM Products WHERE CreatedAt >= @Date";
            return await _unitOfWork.ExecuteScalarAsync<int>(query, new { Date = date });
        }
    }
}

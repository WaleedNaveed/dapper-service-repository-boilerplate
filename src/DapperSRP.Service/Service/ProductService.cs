using AutoMapper;
using DapperSRP.Repository.Interface;
using DapperSRP.Service.Interface;
using Model = DapperSRP.Persistence.Models;
using DapperSRP.Dto.Pagination;
using DapperSRP.Service.Exceptions;
using DapperSRP.Dto.Product.CreateProduct.Request;
using DapperSRP.Dto.Product.GetProduct.Response;
using DapperSRP.Dto.Product.UpdateProduct.Request;
using DapperSRP.Dto.Product.UpdateProduct.Response;
using DapperSRP.Dto.GetProductPaginated.Request;
using DapperSRP.Dto.GetProductPaginated.Response;
using DapperSRP.Common;

namespace DapperSRP.Service.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommonService _commonService;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ICommonService commonService)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _commonService = commonService;

        }
        public async Task<ServiceResponse<int>> AddAsync(CreateProductRequest request)
        {
            var loggedInUserId = _commonService.GetLoggedInUserId();

            var p = _mapper.Map<Model.Product>(request);

            p.CreatedBy = loggedInUserId;
            p.UpdatedBy = loggedInUserId;
            p.UpdatedAt = DateTime.UtcNow;

            var id = await _productRepository.AddAndReturnIdAsync(p);

            if (id <= 0)
            {
                throw new BadRequestException(MessagesConstants.ProductCreationFailed);
            }

            return new ServiceResponse<int>
            {
                HasError = false,
                Result = id
            };
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new NotFoundException(MessagesConstants.ProductDoesNotExist);
            }

            var res = await _productRepository.DeleteAsync(id);
            if (res <= 0)
            {
                throw new BadRequestException(MessagesConstants.ProductDeletionFailed);
            }

            return new ServiceResponse<bool> { HasError = false, Result = true };
        }

        public async Task<ServiceResponse<IEnumerable<GetProductResponse>>> GetAllAsync()
        {
            var productsList = await _productRepository.GetAllAsync();

            // Get all unique UserIds from CreatedBy and UpdatedBy fields
            var userIds = productsList.Select(p => p.CreatedBy)
                                      .Concat(productsList.Select(p => p.UpdatedBy))
                                      .Distinct()
                                      .ToList();

            // Fetch all users in one query
            var users = await _userRepository.GetUsersByIdsAsync(userIds);
            var userDictionary = users.ToDictionary(u => u.Id, u => u.Name);

            var products = _mapper.Map<List<GetProductResponse>>(productsList);

            foreach (var product in products)
            {
                product.CreatedBy = userDictionary.GetValueOrDefault(int.Parse(product.CreatedBy), "Unknown");
                product.UpdatedBy = userDictionary.GetValueOrDefault(int.Parse(product.UpdatedBy), "Unknown");
            }

            return new ServiceResponse<IEnumerable<GetProductResponse>>
            {
                HasError = false,
                Result = products
            };
        }

        public async Task<ServiceResponse<PaginationResponse<ProductQueryResponse>>> GetPagedAsync(ProductQueryRequest request)
        {
            var query = _mapper.Map<DapperSRP.Persistence.Models.Custom.Product.ProductQueryRequest>(request);

            var (products, totalCount) = await _productRepository.GetPagedAsync(query);

            var response = _mapper.Map<List<ProductQueryResponse>>(products);

            var paginationResponse = new PaginationResponse<ProductQueryResponse>
            {
                Items = response,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };

            return new ServiceResponse<PaginationResponse<ProductQueryResponse>>
            {
                HasError = false,
                Result = paginationResponse
            };
        }

        public async Task<ServiceResponse<GetProductResponse>> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new NotFoundException(MessagesConstants.ProductDoesNotExist);
            }
            var map = _mapper.Map<GetProductResponse>(product);

            var createdByUser = await _userRepository.GetByIdAsync(int.Parse(map.CreatedBy));
            var updatedByUser = await _userRepository.GetByIdAsync(int.Parse(map.UpdatedBy));

            map.CreatedBy = createdByUser.Name;
            map.UpdatedBy = updatedByUser.Name;

            return new ServiceResponse<GetProductResponse>
            {
                HasError = false,
                Result = map
            };
        }

        public async Task<ServiceResponse<UpdateProductResponse>> UpdateAsync(int Id, UpdateProductRequest request)
        {
            var loggedInUserId = _commonService.GetLoggedInUserId();

            var product = await _productRepository.GetByIdAsync(Id);
            if (product == null)
            {
                throw new NotFoundException(MessagesConstants.ProductDoesNotExist);
            }

            _mapper.Map(request, product);

            product.UpdatedBy = loggedInUserId;
            product.UpdatedAt = DateTime.UtcNow;

            var res = await _productRepository.UpdateAsync(product);
            if (res <= 0)
            {
                throw new BadRequestException(MessagesConstants.CouldNotUpdateProduct);
            }

            // Fetch CreatedBy and UpdatedBy user names
            var userIds = new List<int> { product.CreatedBy, product.UpdatedBy };
            var users = await _userRepository.GetUsersByIdsAsync(userIds);
            var userDictionary = users.ToDictionary(u => u.Id, u => u.Name);

            var response = _mapper.Map<UpdateProductResponse>(product);
            response.CreatedBy = userDictionary.GetValueOrDefault(product.CreatedBy, "Unknown");
            response.UpdatedBy = userDictionary.GetValueOrDefault(product.UpdatedBy, "Unknown");

            return new ServiceResponse<UpdateProductResponse>
            {
                HasError = false,
                Result = response
            };
        }

        public async Task<ServiceResponse<int>> GetNewProductsCountAsync(DateTime date)
        {
            var count = await _productRepository.GetNewProductsCountAsync(date);
            return new ServiceResponse<int> { HasError = false, Result = count };
        }
    }
}

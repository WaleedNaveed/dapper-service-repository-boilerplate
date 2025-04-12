using AutoMapper;
using DapperSRP.Dto.GetProductPaginated.Request;
using DapperSRP.Dto.GetProductPaginated.Response;
using DapperSRP.Dto.Product.CreateProduct.Request;
using DapperSRP.Dto.Product.GetProduct.Response;
using DapperSRP.Dto.Product.UpdateProduct.Response;
using DapperSRP.Dto.Role.Response;
using DapperSRP.Persistence.Models;

namespace DapperSRP.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Product
            CreateMap<Product, GetProductResponse>();
            CreateMap<Product, UpdateProductResponse>();
            CreateMap<CreateProductRequest, Product>();
            CreateMap<ProductQueryRequest, DapperSRP.Persistence.Models.Custom.Product.ProductQueryRequest>();
            CreateMap<DapperSRP.Persistence.Models.Custom.Product.ProductQueryResponse, ProductQueryResponse>();
            #endregion

            #region Role
            CreateMap<Role, GetRoleResponse>();
            #endregion
        }
    }
}

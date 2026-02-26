using Application.Dto.Product;
using AutoMapper;
using Domain.Interface.Repository;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Product.Queries.GetAllProduct
{
    public class GetAllProductHandler : IRequestHandler<GetAllProductQueries, PagedResult<ProductResponse>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public GetAllProductHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<PagedResult<ProductResponse>> Handle(GetAllProductQueries request, CancellationToken cancellationToken)
        {
            var (products, totalCount) = await _productRepository.GetPagedProductsAsync(request.Keyword, request.PageIndex, request.PageSize);
            var productResponses = _mapper.Map<List<ProductResponse>>(products);
            return new PagedResult<ProductResponse>(productResponses, request.PageIndex, request.PageSize, totalCount, "Lấy danh sách sản phẩm thành công.");
        }
    }
}
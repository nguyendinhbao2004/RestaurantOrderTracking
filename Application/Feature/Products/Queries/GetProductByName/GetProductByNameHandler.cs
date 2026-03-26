using Application.Dto.Product;
using AutoMapper;
using Domain.Interface.Repository;
using MediatR;

namespace Application.Feature.Products.Queries.GetProductByName
{
    public class GetProductByNameHandler : IRequestHandler<GetProductByNameQuery, ProductResponse?>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public GetProductByNameHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<ProductResponse?> Handle(GetProductByNameQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByNameAsync(request.Name);
            if (product == null) return null;
            return _mapper.Map<ProductResponse>(product);
        }
    }
}
using Application.Feature.Product.Commands.Create;
using Domain.Interface.Repository;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;

namespace Application.Feature.Products.Commands.Create
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new RestaurantOrderTracking.Domain.Entities.Product(
                categoryId: request.CategoryId,
                name: request.Name,
                price: request.Price,
                description: request.Description,
                isActive: true,
                imageUrl: request.ImageUrl
            );

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Product created successfully",product.Id);
        }
    }
}
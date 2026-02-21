using Domain.Interface.Repository;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;

namespace Application.Feature.Products.Commands.Update.UpdateInfo
{
    public class UpdateInfoProductHandler : IRequestHandler<UpdateInfoProductCommand, Result<Guid>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateInfoProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(UpdateInfoProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return Result<Guid>.Failure("Product not found.");
            }

            product.UpdateInfo(request.Name, request.Price, request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Update Info Successfully",product.Id);
        }
    }
}
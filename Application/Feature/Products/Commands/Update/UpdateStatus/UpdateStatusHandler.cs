using Domain.Interface.Repository;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;

namespace Application.Feature.Products.Commands.Update.UpdateStatus
{
    public class UpdateStatusHandler : IRequestHandler<UpdateStatusProductCommand, Result<Guid>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateStatusHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(UpdateStatusProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return Result<Guid>.Failure("Product not found.");
            }
            
            if (product.IsActive)
            {
                product.Deactivate();
            }
            else
            {
                product.Activate();
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success("Update Status Successfully", product.Id);
        }
    }
}
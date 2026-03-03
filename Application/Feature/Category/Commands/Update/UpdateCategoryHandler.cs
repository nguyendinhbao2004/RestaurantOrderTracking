using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using CategoryEntity = RestaurantOrderTracking.Domain.Entities.Category;

namespace RestaurantOrderTracking.Application.Feature.Category.Commands.Update
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly IGenericRepository<CategoryEntity> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryHandler(IGenericRepository<CategoryEntity> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category == null)
                return Result.Failure("Category not found.");

            if (!string.IsNullOrEmpty(request.Name))
                category.UpdateName(request.Name);

            if (request.Description != null)
                category.UpdateDescription(request.Description);

            if (request.ImageUrl != null)
                category.UpdateImage(request.ImageUrl);

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                    category.Activate();
                else
                    category.Deactivate();
            }

            _categoryRepository.Update(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Category updated successfully.");
        }
    }
}

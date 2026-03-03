using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using CategoryEntity = RestaurantOrderTracking.Domain.Entities.Category;

namespace RestaurantOrderTracking.Application.Feature.Category.Commands.Delete
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly IGenericRepository<CategoryEntity> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryHandler(IGenericRepository<CategoryEntity> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (category == null)
                return Result.Failure("Category not found.");

            // Kiểm tra có sản phẩm thuộc category không
            if (category.Products.Any())
                return Result.Failure("Cannot delete category that has products. Please remove or reassign products first.");

            _categoryRepository.Delete(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Category deleted successfully.");
        }
    }
}

using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using CategoryEntity = RestaurantOrderTracking.Domain.Entities.Category;

namespace RestaurantOrderTracking.Application.Feature.Category.Commands.Create
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<int>>
    {
        private readonly IGenericRepository<CategoryEntity> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCategoryHandler(IGenericRepository<CategoryEntity> categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra trùng tên
            var existing = await _categoryRepository.FindAsync(c => c.Name == request.Name);
            if (existing.Any())
                return Result<int>.Failure("Category name already exists.");

            var category = new CategoryEntity(
                id: request.Id,
                name: request.Name,
                description: request.Description,
                imageUrl: request.ImageUrl
            );

            await _categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success("Category created successfully.", category.Id);
        }
    }
}

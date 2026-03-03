using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Application.Dto.Category;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using CategoryEntity = RestaurantOrderTracking.Domain.Entities.Category;

namespace RestaurantOrderTracking.Application.Feature.Category.Queries.GetAll
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, Result<List<CategoryResponse>>>
    {
        private readonly IGenericRepository<CategoryEntity> _categoryRepository;
        private readonly IMapper _mapper;

        public GetAllCategoriesHandler(IGenericRepository<CategoryEntity> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryResponse>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync();
            var response = _mapper.Map<List<CategoryResponse>>(categories);
            return Result<List<CategoryResponse>>.Success("Get all categories successfully.", response);
        }
    }
}

using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Product.Commands.Create
{
    public record CreateProductCommand(string Name, string Description, decimal Price, string ImageUrl, int CategoryId) : IRequest<Result<Guid>>
    {
        
    }
}
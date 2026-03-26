using Application.Dto.Product;
using MediatR;

namespace Application.Feature.Products.Queries.GetProductByName
{
    public record GetProductByNameQuery(string Name) : IRequest<ProductResponse?>
    {
    }
}
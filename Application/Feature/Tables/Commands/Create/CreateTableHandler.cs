using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Tables.Commands.Create
{
    public class CreateTableHandler : IRequestHandler<CreateTableCommand, Result<Guid>>
    {
        private readonly IGenericRepository<RestaurantOrderTracking.Domain.Entities.Table> _tableRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTableHandler(IGenericRepository<RestaurantOrderTracking.Domain.Entities.Table> tableRepository, IUnitOfWork unitOfWork)
        {
            _tableRepository = tableRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateTableCommand request, CancellationToken cancellationToken)
        {
            var table = new RestaurantOrderTracking.Domain.Entities.Table(request.TableNumber, request.AreaId, qrCode: request.QRCode);
            await _tableRepository.AddAsync(table);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(table.Id);
        }
    }
}
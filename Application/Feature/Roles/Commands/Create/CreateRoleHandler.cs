using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Roles.Commands.Create
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Result<int>>
    {
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoleHandler(IGenericRepository<Role> roleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var normalizedName = request.Name.Trim().ToLower();

            var existingRole = await _roleRepository.FindAsync(r => r.Name.ToLower() == normalizedName);
            if (existingRole.Any())
            {
                return Result<int>.Failure("Role name already exists.");
            }

            // Id = 0 lets EF Core use the database identity value.
            var role = new Role(0, request.Name.Trim(), request.Description.Trim());

            await _roleRepository.AddAsync(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success("Role created successfully.", role.Id);
        }
    }
}
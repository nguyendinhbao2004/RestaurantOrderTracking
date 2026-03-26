using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var normalizedName = request.Name?.Trim();
            var description = request.Description?.Trim();

            var existingRole = await _roleRepository.FindAsync(r => r.Name == normalizedName);
            if (existingRole.Any())
            {
                return Result<int>.Failure("Role name already exists.");
            }

            
            var role = new Role(normalizedName, description);

            await _roleRepository.AddAsync(role);
            try
            {
                await _roleRepository.AddAsync(role);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<int>.Success("Role created successfully.", role.Id);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception (not implemented here)
                return Result<int>.Failure("Role already exists (DB constraint).");
            }

        }
    }
}
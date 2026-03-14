using Application.Dto.Dashboard;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Feature.Dashboard.Queries.GetDashboardSummary
{
    public record GetDashboardSummaryQueries : IRequest<Result<DashboardSummaryResponse>>
    {
    }
}

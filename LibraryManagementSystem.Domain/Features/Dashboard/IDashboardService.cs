using System.Threading.Tasks;
using LibraryManagementSystem.Domain.Models;

namespace LibraryManagementSystem.Domain.Features.Dashboard;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardMetricsAsync();
}

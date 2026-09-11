using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.Enums;
using HelpEmpowermentApi.Payments.Application;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.IRepositories;

public interface IRevenueManagementRepository
{
    Task<PagedResult<UserCourseAssignmentDto>> SearchAssignmentsAsync(DataRequest request, CancellationToken ct);
    Task<UserCourseAssignmentDto?> GetAssignmentByIdAsync(Guid id, CancellationToken ct);
    Task<PagedResult<CourseRevenueShareDto>> SearchSharesAsync(Guid courseId, DataRequest request, CancellationToken ct);
    Task<PagedResult<CourseRevenueDetailsDto>> SearchCourseRevenueAsync(DataRequest request, Guid userId, bool globalAccess, CancellationToken ct);
    Task<CourseRevenueShareDto?> GetShareByIdAsync(Guid courseId, Guid id, CancellationToken ct);
    Task<PagedResult<RevenueSettlementDto>> SearchSettlementsAsync(DataRequest request, CancellationToken ct);
    Task<RevenueSettlementDto?> GetSettlementByIdAsync(Guid id, CancellationToken ct);
    Task<bool> CanAccessCourseAsync(Guid userId, Guid courseId, bool globalAccess, CancellationToken ct);
    Task<IReadOnlyList<UserCourseAssignmentDto>> GetAssignmentsAsync(Guid? userId, Guid? courseId, Guid? assignmentType, bool? isActive, CancellationToken ct);
    Task<ServiceResult<UserCourseAssignmentDto>> CreateAssignmentAsync(SaveUserCourseAssignmentDto dto, Guid actorId, CancellationToken ct);
    Task<ServiceResult<UserCourseAssignmentDto>> UpdateAssignmentAsync(Guid id, SaveUserCourseAssignmentDto dto, Guid actorId, CancellationToken ct);
    Task<bool> DeleteAssignmentAsync(Guid id, Guid actorId, CancellationToken ct);
    Task<IReadOnlyList<AssignedCourseDto>> GetUserCoursesAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<CourseRevenueShareDto>> GetSharesAsync(Guid courseId, CancellationToken ct);
    Task<ServiceResult<CourseRevenueShareDto>> CreateShareAsync(Guid courseId, SaveCourseRevenueShareDto dto, Guid actorId, CancellationToken ct);
    Task<ServiceResult<CourseRevenueShareDto>> UpdateShareAsync(Guid courseId, Guid id, SaveCourseRevenueShareDto dto, Guid actorId, CancellationToken ct);
    Task<bool> DeleteShareAsync(Guid courseId, Guid id, Guid actorId, CancellationToken ct);
    Task<MyRevenueDto> GetMyRevenueAsync(Guid userId, Guid? courseId, DateTime? dateFrom, DateTime? dateTo, RevenueDistributionStatus? status, CancellationToken ct);
    Task<CourseRevenueSummaryDto?> GetCourseSummaryAsync(Guid courseId, CancellationToken ct);
    Task<AssignedDashboardDto> GetDashboardAsync(Guid userId, bool globalAccess, CancellationToken ct);
    Task<IReadOnlyList<RevenueSettlementDto>> GetSettlementsAsync(Guid? beneficiaryUserId, RevenueSettlementStatus? status, CancellationToken ct);
    Task<ServiceResult<RevenueSettlementDto>> CreateSettlementAsync(CreateRevenueSettlementDto dto, Guid actorId, CancellationToken ct);
    Task<ServiceResult<RevenueSettlementDto>> UpdateSettlementStatusAsync(Guid id, UpdateRevenueSettlementStatusDto dto, Guid actorId, CancellationToken ct);
}

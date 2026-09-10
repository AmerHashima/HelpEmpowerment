using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.Enums;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Payments.Application;
using HelpEmpowermentApi.Common;

namespace HelpEmpowermentApi.Services;

public sealed class RevenueManagementService(IRevenueManagementRepository repository) : IRevenueManagementService
{
    public async Task<PagedResponse<UserCourseAssignmentDto>> SearchAssignmentsAsync(DataRequest request, CancellationToken ct) => ToPagedResponse(await repository.SearchAssignmentsAsync(request, ct));
    public async Task<ApiResponse<UserCourseAssignmentDto>> GetAssignmentByIdAsync(Guid id, CancellationToken ct) => ToResponse(await repository.GetAssignmentByIdAsync(id, ct), "Assignment not found");
    public async Task<PagedResponse<CourseRevenueShareDto>> SearchSharesAsync(Guid courseId, DataRequest request, CancellationToken ct) => ToPagedResponse(await repository.SearchSharesAsync(courseId, request, ct));
    public async Task<ApiResponse<CourseRevenueShareDto>> GetShareByIdAsync(Guid courseId, Guid id, CancellationToken ct) => ToResponse(await repository.GetShareByIdAsync(courseId, id, ct), "Revenue share not found");
    public async Task<PagedResponse<RevenueSettlementDto>> SearchSettlementsAsync(DataRequest request, CancellationToken ct) => ToPagedResponse(await repository.SearchSettlementsAsync(request, ct));
    public async Task<ApiResponse<RevenueSettlementDto>> GetSettlementByIdAsync(Guid id, CancellationToken ct) => ToResponse(await repository.GetSettlementByIdAsync(id, ct), "Settlement not found");

    private static ApiResponse<T> ToResponse<T>(T? data, string notFoundMessage) where T : class =>
        data is null ? ApiResponse<T>.ErrorResponse(notFoundMessage) : ApiResponse<T>.SuccessResponse(data);

    private static PagedResponse<T> ToPagedResponse<T>(PagedResult<T> result) => new()
    {
        Success = true,
        Data = result.Items.ToList(),
        TotalCount = result.TotalCount,
        PageNumber = result.PageNumber,
        PageSize = result.PageSize
    };

    public Task<bool> CanAccessCourseAsync(Guid userId, Guid courseId, bool globalAccess, CancellationToken ct) => repository.CanAccessCourseAsync(userId, courseId, globalAccess, ct);
    public Task<IReadOnlyList<UserCourseAssignmentDto>> GetAssignmentsAsync(Guid? userId, Guid? courseId, Guid? assignmentType, bool? isActive, CancellationToken ct) => repository.GetAssignmentsAsync(userId, courseId, assignmentType, isActive, ct);
    public Task<ServiceResult<UserCourseAssignmentDto>> CreateAssignmentAsync(SaveUserCourseAssignmentDto dto, Guid actorId, CancellationToken ct) => repository.CreateAssignmentAsync(dto, actorId, ct);
    public Task<ServiceResult<UserCourseAssignmentDto>> UpdateAssignmentAsync(Guid id, SaveUserCourseAssignmentDto dto, Guid actorId, CancellationToken ct) => repository.UpdateAssignmentAsync(id, dto, actorId, ct);
    public Task<bool> DeleteAssignmentAsync(Guid id, Guid actorId, CancellationToken ct) => repository.DeleteAssignmentAsync(id, actorId, ct);
    public Task<IReadOnlyList<AssignedCourseDto>> GetUserCoursesAsync(Guid userId, CancellationToken ct) => repository.GetUserCoursesAsync(userId, ct);
    public Task<IReadOnlyList<CourseRevenueShareDto>> GetSharesAsync(Guid courseId, CancellationToken ct) => repository.GetSharesAsync(courseId, ct);
    public Task<ServiceResult<CourseRevenueShareDto>> CreateShareAsync(Guid courseId, SaveCourseRevenueShareDto dto, Guid actorId, CancellationToken ct) => repository.CreateShareAsync(courseId, dto, actorId, ct);
    public Task<ServiceResult<CourseRevenueShareDto>> UpdateShareAsync(Guid courseId, Guid id, SaveCourseRevenueShareDto dto, Guid actorId, CancellationToken ct) => repository.UpdateShareAsync(courseId, id, dto, actorId, ct);
    public Task<bool> DeleteShareAsync(Guid courseId, Guid id, Guid actorId, CancellationToken ct) => repository.DeleteShareAsync(courseId, id, actorId, ct);
    public Task<MyRevenueDto> GetMyRevenueAsync(Guid userId, Guid? courseId, DateTime? dateFrom, DateTime? dateTo, RevenueDistributionStatus? status, CancellationToken ct) => repository.GetMyRevenueAsync(userId, courseId, dateFrom, dateTo, status, ct);
    public Task<CourseRevenueSummaryDto?> GetCourseSummaryAsync(Guid courseId, CancellationToken ct) => repository.GetCourseSummaryAsync(courseId, ct);
    public Task<AssignedDashboardDto> GetDashboardAsync(Guid userId, bool globalAccess, CancellationToken ct) => repository.GetDashboardAsync(userId, globalAccess, ct);
    public Task<IReadOnlyList<RevenueSettlementDto>> GetSettlementsAsync(Guid? beneficiaryUserId, RevenueSettlementStatus? status, CancellationToken ct) => repository.GetSettlementsAsync(beneficiaryUserId, status, ct);
    public Task<ServiceResult<RevenueSettlementDto>> CreateSettlementAsync(CreateRevenueSettlementDto dto, Guid actorId, CancellationToken ct) => repository.CreateSettlementAsync(dto, actorId, ct);
    public Task<ServiceResult<RevenueSettlementDto>> UpdateSettlementStatusAsync(Guid id, UpdateRevenueSettlementStatusDto dto, Guid actorId, CancellationToken ct) => repository.UpdateSettlementStatusAsync(id, dto, actorId, ct);
}

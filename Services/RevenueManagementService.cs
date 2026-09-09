using System.Data;
using HelpEmpowermentApi.Data;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.Enums;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Payments.Application;
using Microsoft.EntityFrameworkCore;

namespace HelpEmpowermentApi.Services;

public sealed class RevenueManagementService(ApplicationDbContext db) : IRevenueManagementService
{
    public Task<bool> CanAccessCourseAsync(Guid userId, Guid courseId, bool globalAccess, CancellationToken ct) =>
        globalAccess
            ? db.Courses.AnyAsync(x => x.Oid == courseId && !x.IsDeleted, ct)
            : db.UserCourseAssignments.AnyAsync(x => x.UserId == userId && x.CourseId == courseId
                && x.IsActive && !x.IsDeleted, ct);

    public async Task<IReadOnlyList<UserCourseAssignmentDto>> GetAssignmentsAsync(
        Guid? userId, Guid? courseId, Guid? assignmentType, bool? isActive, CancellationToken ct)
    {
        var query = db.UserCourseAssignments.AsNoTracking().Where(x => !x.IsDeleted);
        if (userId.HasValue) query = query.Where(x => x.UserId == userId);
        if (courseId.HasValue) query = query.Where(x => x.CourseId == courseId);
        if (assignmentType.HasValue) query = query.Where(x => x.AssignmentTypeLookupId == assignmentType);
        if (isActive.HasValue) query = query.Where(x => x.IsActive == isActive);

        return await query.OrderBy(x => x.Course.CourseName).ThenBy(x => x.User.Username)
            .Select(x => new UserCourseAssignmentDto(x.Oid, x.UserId, x.User.Username,
                x.CourseId, x.Course.CourseName, x.AssignmentTypeLookupId,
                x.AssignmentType.LookupNameEn ?? x.AssignmentType.LookupValue, x.IsPrimary, x.IsActive))
            .ToListAsync(ct);
    }

    public async Task<ServiceResult<UserCourseAssignmentDto>> CreateAssignmentAsync(
        SaveUserCourseAssignmentDto dto, Guid actorId, CancellationToken ct)
    {
        var error = await ValidateAssignmentAsync(dto, null, ct);
        if (error is not null) return ServiceResult<UserCourseAssignmentDto>.Failure("INVALID_ASSIGNMENT", error);
        var entity = new UserCourseAssignment
        {
            Oid = Guid.NewGuid(), UserId = dto.UserId, CourseId = dto.CourseId,
            AssignmentTypeLookupId = dto.AssignmentTypeId, IsPrimary = dto.IsPrimary,
            IsActive = dto.IsActive, CreatedBy = actorId, CreatedAt = DateTime.UtcNow
        };
        db.UserCourseAssignments.Add(entity);
        await db.SaveChangesAsync(ct);
        return ServiceResult<UserCourseAssignmentDto>.Success((await GetAssignmentAsync(entity.Oid, ct))!);
    }

    public async Task<ServiceResult<UserCourseAssignmentDto>> UpdateAssignmentAsync(
        Guid id, SaveUserCourseAssignmentDto dto, Guid actorId, CancellationToken ct)
    {
        var entity = await db.UserCourseAssignments.AsTracking().SingleOrDefaultAsync(x => x.Oid == id && !x.IsDeleted, ct);
        if (entity is null) return ServiceResult<UserCourseAssignmentDto>.Failure("ASSIGNMENT_NOT_FOUND", "Assignment was not found.");
        var error = await ValidateAssignmentAsync(dto, id, ct);
        if (error is not null) return ServiceResult<UserCourseAssignmentDto>.Failure("INVALID_ASSIGNMENT", error);
        entity.UserId = dto.UserId; entity.CourseId = dto.CourseId;
        entity.AssignmentTypeLookupId = dto.AssignmentTypeId; entity.IsPrimary = dto.IsPrimary;
        entity.IsActive = dto.IsActive; entity.UpdatedBy = actorId; entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return ServiceResult<UserCourseAssignmentDto>.Success((await GetAssignmentAsync(entity.Oid, ct))!);
    }

    public async Task<bool> DeleteAssignmentAsync(Guid id, Guid actorId, CancellationToken ct)
    {
        var entity = await db.UserCourseAssignments.AsTracking().SingleOrDefaultAsync(x => x.Oid == id && !x.IsDeleted, ct);
        if (entity is null) return false;
        entity.IsDeleted = true; entity.IsActive = false; entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow; entity.UpdatedBy = actorId;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<AssignedCourseDto>> GetUserCoursesAsync(Guid userId, CancellationToken ct) =>
        await db.UserCourseAssignments.AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive && !x.IsDeleted && x.Course.IsActive && !x.Course.IsDeleted)
            .OrderBy(x => x.Course.CourseName)
            .Select(x => new AssignedCourseDto(x.CourseId, x.Course.CourseCode, x.Course.CourseName,
                x.AssignmentTypeLookupId, x.AssignmentType.LookupNameEn ?? x.AssignmentType.LookupValue,
                x.IsPrimary, x.IsActive))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CourseRevenueShareDto>> GetSharesAsync(Guid courseId, CancellationToken ct) =>
        await db.CourseRevenueShares.AsNoTracking().Where(x => x.CourseId == courseId && !x.IsDeleted)
            .OrderBy(x => x.ShareType.OrderNo).ThenBy(x => x.BeneficiaryUser!.Username)
            .Select(x => new CourseRevenueShareDto(x.Oid, x.CourseId, x.BeneficiaryUserId,
                x.BeneficiaryUser == null ? null : x.BeneficiaryUser.Username, x.ShareTypeLookupId,
                x.ShareType.LookupNameEn ?? x.ShareType.LookupValue, x.CalculationType, x.Value,
                x.IsActive, x.EffectiveFrom, x.EffectiveTo, x.Notes))
            .ToListAsync(ct);

    public async Task<ServiceResult<CourseRevenueShareDto>> CreateShareAsync(
        Guid courseId, SaveCourseRevenueShareDto dto, Guid actorId, CancellationToken ct)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var error = await ValidateShareAsync(courseId, dto, null, ct);
            if (error is not null) return ServiceResult<CourseRevenueShareDto>.Failure("INVALID_REVENUE_SHARE", error);
            var entity = new CourseRevenueShare
            {
                Oid = Guid.NewGuid(), CourseId = courseId, BeneficiaryUserId = dto.BeneficiaryUserId,
                ShareTypeLookupId = dto.ShareTypeId, CalculationType = dto.CalculationType, Value = dto.Value,
                IsActive = dto.IsActive, EffectiveFrom = dto.EffectiveFrom, EffectiveTo = dto.EffectiveTo,
                Notes = dto.Notes, CreatedBy = actorId, CreatedAt = DateTime.UtcNow
            };
            db.CourseRevenueShares.Add(entity);
            await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct);
            return ServiceResult<CourseRevenueShareDto>.Success((await GetShareAsync(entity.Oid, ct))!);
        });
    }

    public async Task<ServiceResult<CourseRevenueShareDto>> UpdateShareAsync(
        Guid courseId, Guid id, SaveCourseRevenueShareDto dto, Guid actorId, CancellationToken ct)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var entity = await db.CourseRevenueShares.AsTracking()
                .SingleOrDefaultAsync(x => x.Oid == id && x.CourseId == courseId && !x.IsDeleted, ct);
            if (entity is null) return ServiceResult<CourseRevenueShareDto>.Failure("REVENUE_SHARE_NOT_FOUND", "Revenue share was not found.");
            var error = await ValidateShareAsync(courseId, dto, id, ct);
            if (error is not null) return ServiceResult<CourseRevenueShareDto>.Failure("INVALID_REVENUE_SHARE", error);
            entity.BeneficiaryUserId = dto.BeneficiaryUserId; entity.ShareTypeLookupId = dto.ShareTypeId;
            entity.CalculationType = dto.CalculationType; entity.Value = dto.Value; entity.IsActive = dto.IsActive;
            entity.EffectiveFrom = dto.EffectiveFrom; entity.EffectiveTo = dto.EffectiveTo; entity.Notes = dto.Notes;
            entity.UpdatedBy = actorId; entity.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct);
            return ServiceResult<CourseRevenueShareDto>.Success((await GetShareAsync(entity.Oid, ct))!);
        });
    }

    public async Task<bool> DeleteShareAsync(Guid courseId, Guid id, Guid actorId, CancellationToken ct)
    {
        var entity = await db.CourseRevenueShares.AsTracking()
            .SingleOrDefaultAsync(x => x.Oid == id && x.CourseId == courseId && !x.IsDeleted, ct);
        if (entity is null) return false;
        entity.IsDeleted = true; entity.IsActive = false; entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedBy = actorId; entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<MyRevenueDto> GetMyRevenueAsync(Guid userId, Guid? courseId, DateTime? dateFrom,
        DateTime? dateTo, RevenueDistributionStatus? status, CancellationToken ct)
    {
        var query = db.CourseRevenueDistributions.AsNoTracking()
            .Where(x => !x.IsDeleted && x.BeneficiaryUserId == userId);
        if (courseId.HasValue) query = query.Where(x => x.CourseId == courseId);
        if (dateFrom.HasValue) query = query.Where(x => x.CreatedAt >= dateFrom);
        if (dateTo.HasValue) query = query.Where(x => x.CreatedAt < dateTo.Value.Date.AddDays(1));
        if (status.HasValue) query = query.Where(x => x.Status == status);
        var aggregates = await query
            .GroupBy(x => new { x.CourseId, x.Course.CourseName })
            .Select(group => new
            {
                group.Key.CourseId,
                group.Key.CourseName,
                Earned = group.Sum(x => x.ShareAmount),
                Pending = group.Sum(x => x.Status == RevenueDistributionStatus.Pending ? x.ShareAmount : 0m),
                Paid = group.Sum(x => x.Status == RevenueDistributionStatus.Paid ? x.ShareAmount : 0m)
            })
            .OrderBy(x => x.CourseName)
            .ToListAsync(ct);
        var rows = aggregates
            .Select(x => new RevenueByCourseDto(x.CourseId, x.CourseName, x.Earned, x.Pending, x.Paid))
            .ToList();
        return new(rows.Sum(x => x.Earned), rows.Sum(x => x.Pending), rows.Sum(x => x.Paid), rows);
    }

    public async Task<CourseRevenueSummaryDto?> GetCourseSummaryAsync(Guid courseId, CancellationToken ct)
    {
        if (!await db.Courses.AnyAsync(x => x.Oid == courseId && !x.IsDeleted, ct)) return null;
        var totalRevenue = await db.InvoiceItems.AsNoTracking()
            .Where(x => x.CourseId == courseId && x.Invoice.IsPaid).SumAsync(x => (decimal?)x.LineTotal, ct) ?? 0;
        var shares = await db.CourseRevenueDistributions.AsNoTracking()
            .Where(x => x.CourseId == courseId && !x.IsDeleted)
            .GroupBy(x => new { x.BeneficiaryUserId, UserName = x.BeneficiaryUser == null ? null : x.BeneficiaryUser.Username,
                Type = x.ShareType.LookupNameEn, x.AppliedPercentage })
            .Select(g => new RevenueShareBreakdownDto(g.Key.BeneficiaryUserId, g.Key.UserName,
                g.Key.Type ?? string.Empty, g.Key.AppliedPercentage, g.Sum(x => x.ShareAmount),
                g.Sum(x => x.Status == RevenueDistributionStatus.Pending ? x.ShareAmount : 0m),
                g.Sum(x => x.Status == RevenueDistributionStatus.Paid ? x.ShareAmount : 0m)))
            .ToListAsync(ct);
        return new(courseId, totalRevenue, shares.Sum(x => x.Amount), shares.Sum(x => x.Pending), shares.Sum(x => x.Paid), shares);
    }

    public async Task<AssignedDashboardDto> GetDashboardAsync(Guid userId, bool globalAccess, CancellationToken ct)
    {
        var courseIds = globalAccess
            ? db.Courses.Where(x => !x.IsDeleted).Select(x => x.Oid)
            : db.UserCourseAssignments.Where(x => x.UserId == userId && x.IsActive && !x.IsDeleted).Select(x => x.CourseId).Distinct();
        var assigned = await courseIds.CountAsync(ct);
        var active = await db.Courses.CountAsync(x => courseIds.Contains(x.Oid) && x.IsActive && !x.IsDeleted, ct);
        var students = await db.StudentCourses.Where(x => courseIds.Contains(x.CourseId) && !x.IsDeleted)
            .Select(x => x.StudentId).Distinct().CountAsync(ct);
        var reservations = await db.StudentCourseReservations.CountAsync(x =>
            courseIds.Contains(x.StudentCourse.CourseId) && x.IsReserved && !x.IsDeleted, ct);
        var totalRevenue = await db.InvoiceItems.Where(x => courseIds.Contains(x.CourseId) && x.Invoice.IsPaid)
            .SumAsync(x => (decimal?)x.LineTotal, ct) ?? 0;
        var myRevenueQuery = db.CourseRevenueDistributions.Where(x => courseIds.Contains(x.CourseId) && !x.IsDeleted);
        if (!globalAccess) myRevenueQuery = myRevenueQuery.Where(x => x.BeneficiaryUserId == userId);
        var revenue = await myRevenueQuery.GroupBy(_ => 1).Select(g => new
        {
            Total = g.Sum(x => x.ShareAmount),
            Pending = g.Sum(x => x.Status == RevenueDistributionStatus.Pending ? x.ShareAmount : 0m),
            Paid = g.Sum(x => x.Status == RevenueDistributionStatus.Paid ? x.ShareAmount : 0m)
        }).SingleOrDefaultAsync(ct);
        var upcoming = await db.CourseLiveSessions.CountAsync(x => courseIds.Contains(x.CourseOid)
            && x.Active && !x.IsDeleted && x.Date >= DateTime.UtcNow.Date, ct);
        return new(assigned, active, students, reservations, totalRevenue, revenue?.Total ?? 0,
            revenue?.Pending ?? 0, revenue?.Paid ?? 0, upcoming);
    }

    public async Task<IReadOnlyList<RevenueSettlementDto>> GetSettlementsAsync(
        Guid? beneficiaryUserId, RevenueSettlementStatus? status, CancellationToken ct)
    {
        var query = db.RevenueSettlements.AsNoTracking().Where(x => !x.IsDeleted);
        if (beneficiaryUserId.HasValue) query = query.Where(x => x.BeneficiaryUserId == beneficiaryUserId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        return await query.OrderByDescending(x => x.CreatedAt)
            .Select(x => new RevenueSettlementDto(x.Oid, x.BeneficiaryUserId, x.BeneficiaryUser.Username,
                x.SettlementNumber, x.PeriodFrom, x.PeriodTo, x.TotalAmount, x.Status,
                x.PaidAt, x.PaymentReference, x.Notes)).ToListAsync(ct);
    }

    public async Task<ServiceResult<RevenueSettlementDto>> CreateSettlementAsync(
        CreateRevenueSettlementDto dto, Guid actorId, CancellationToken ct)
    {
        if (dto.PeriodFrom > dto.PeriodTo)
            return ServiceResult<RevenueSettlementDto>.Failure("INVALID_PERIOD", "PeriodFrom cannot be after PeriodTo.");
        if (!await db.Users.AnyAsync(x => x.Oid == dto.BeneficiaryUserId && !x.IsDeleted, ct))
            return ServiceResult<RevenueSettlementDto>.Failure("USER_NOT_FOUND", "Beneficiary user was not found.");
        var strategy = db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var distributions = await db.CourseRevenueDistributions.AsTracking().Where(x =>
                x.BeneficiaryUserId == dto.BeneficiaryUserId && x.Status == RevenueDistributionStatus.Pending
                && x.SettlementId == null && !x.IsDeleted && x.CreatedAt >= dto.PeriodFrom
                && x.CreatedAt < dto.PeriodTo.Date.AddDays(1)).ToListAsync(ct);
            if (distributions.Count == 0)
                return ServiceResult<RevenueSettlementDto>.Failure("NO_PENDING_REVENUE", "No pending revenue exists for this period.");
            var now = DateTime.UtcNow;
            var settlement = new RevenueSettlement
            {
                Oid = Guid.NewGuid(), BeneficiaryUserId = dto.BeneficiaryUserId,
                SettlementNumber = $"SET-{now:yyyyMMdd}-{Guid.NewGuid():N}"[..25],
                PeriodFrom = dto.PeriodFrom, PeriodTo = dto.PeriodTo, TotalAmount = distributions.Sum(x => x.ShareAmount),
                Status = RevenueSettlementStatus.Draft, Notes = dto.Notes, CreatedAt = now, CreatedBy = actorId
            };
            db.RevenueSettlements.Add(settlement);
            foreach (var distribution in distributions) distribution.SettlementId = settlement.Oid;
            await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct);
            return ServiceResult<RevenueSettlementDto>.Success((await GetSettlementAsync(settlement.Oid, ct))!);
        });
    }

    public async Task<ServiceResult<RevenueSettlementDto>> UpdateSettlementStatusAsync(
        Guid id, UpdateRevenueSettlementStatusDto dto, Guid actorId, CancellationToken ct)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            var entity = await db.RevenueSettlements.AsTracking().Include(x => x.Distributions)
                .SingleOrDefaultAsync(x => x.Oid == id && !x.IsDeleted, ct);
            if (entity is null) return ServiceResult<RevenueSettlementDto>.Failure("SETTLEMENT_NOT_FOUND", "Settlement was not found.");
            var valid = (entity.Status, dto.Status) switch
            {
                (RevenueSettlementStatus.Draft, RevenueSettlementStatus.Approved or RevenueSettlementStatus.Cancelled) => true,
                (RevenueSettlementStatus.Approved, RevenueSettlementStatus.Paid or RevenueSettlementStatus.Cancelled) => true,
                _ when entity.Status == dto.Status => true,
                _ => false
            };
            if (!valid) return ServiceResult<RevenueSettlementDto>.Failure("INVALID_STATUS_TRANSITION", $"Cannot change {entity.Status} to {dto.Status}.");
            var now = DateTime.UtcNow; entity.Status = dto.Status; entity.PaymentReference = dto.PaymentReference;
            entity.UpdatedAt = now; entity.UpdatedBy = actorId;
            if (dto.Status == RevenueSettlementStatus.Paid)
            {
                entity.PaidAt = now;
                foreach (var distribution in entity.Distributions)
                { distribution.Status = RevenueDistributionStatus.Paid; distribution.PaidAt = now; distribution.UpdatedAt = now; }
            }
            else if (dto.Status == RevenueSettlementStatus.Cancelled)
            {
                foreach (var distribution in entity.Distributions) distribution.SettlementId = null;
            }
            await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct);
            return ServiceResult<RevenueSettlementDto>.Success((await GetSettlementAsync(entity.Oid, ct))!);
        });
    }

    private async Task<string?> ValidateAssignmentAsync(SaveUserCourseAssignmentDto dto, Guid? currentId, CancellationToken ct)
    {
        if (!await db.Users.AnyAsync(x => x.Oid == dto.UserId && !x.IsDeleted, ct)) return "User was not found.";
        if (!await db.Courses.AnyAsync(x => x.Oid == dto.CourseId && !x.IsDeleted, ct)) return "Course was not found.";
        if (!await IsAssignmentTypeAsync(dto.AssignmentTypeId, ct)) return "Assignment type is invalid.";
        if (dto.IsActive && await db.UserCourseAssignments.AnyAsync(x => x.Oid != currentId && !x.IsDeleted && x.IsActive
            && x.UserId == dto.UserId && x.CourseId == dto.CourseId && x.AssignmentTypeLookupId == dto.AssignmentTypeId, ct))
            return "An active assignment already exists for this user, course, and assignment type.";
        return null;
    }

    private async Task<string?> ValidateShareAsync(Guid courseId, SaveCourseRevenueShareDto dto, Guid? currentId, CancellationToken ct)
    {
        if (!await db.Courses.AnyAsync(x => x.Oid == courseId && !x.IsDeleted, ct)) return "Course was not found.";
        if (dto.BeneficiaryUserId.HasValue && !await db.Users.AnyAsync(x => x.Oid == dto.BeneficiaryUserId && !x.IsDeleted, ct)) return "Beneficiary user was not found.";
        if (!await IsAssignmentTypeAsync(dto.ShareTypeId, ct)) return "Share type is invalid.";
        if (dto.Value < 0) return "Value cannot be negative.";
        if (dto.CalculationType == RevenueCalculationType.Percentage && dto.Value > 100) return "Percentage cannot exceed 100.";
        if (dto.EffectiveFrom.HasValue && dto.EffectiveTo.HasValue && dto.EffectiveFrom > dto.EffectiveTo)
            return "EffectiveFrom cannot be after EffectiveTo.";
        if (dto.IsActive && await db.CourseRevenueShares.AnyAsync(x => x.Oid != currentId && !x.IsDeleted && x.IsActive
            && x.CourseId == courseId && x.BeneficiaryUserId == dto.BeneficiaryUserId && x.ShareTypeLookupId == dto.ShareTypeId, ct))
            return "An active share already exists for this course, beneficiary, and share type.";
        if (dto.IsActive && dto.CalculationType == RevenueCalculationType.Percentage)
        {
            var total = await db.CourseRevenueShares.Where(x => x.Oid != currentId && x.CourseId == courseId
                && x.IsActive && !x.IsDeleted && x.CalculationType == RevenueCalculationType.Percentage)
                .SumAsync(x => (decimal?)x.Value, ct) ?? 0;
            if (total + dto.Value > 100) return "Active percentage shares for a course cannot exceed 100%.";
        }
        return null;
    }

    private Task<bool> IsAssignmentTypeAsync(Guid id, CancellationToken ct) => db.AppLookupDetails.AnyAsync(x =>
        x.Oid == id && x.IsActive && !x.IsDeleted && x.LookupHeader.LookupCode == "COURSE_ASSIGNMENT_TYPE", ct);

    private Task<UserCourseAssignmentDto?> GetAssignmentAsync(Guid id, CancellationToken ct) =>
        db.UserCourseAssignments.AsNoTracking().Where(x => x.Oid == id)
            .Select(x => new UserCourseAssignmentDto(x.Oid, x.UserId, x.User.Username, x.CourseId,
                x.Course.CourseName, x.AssignmentTypeLookupId, x.AssignmentType.LookupNameEn ?? x.AssignmentType.LookupValue,
                x.IsPrimary, x.IsActive)).SingleOrDefaultAsync(ct);

    private Task<CourseRevenueShareDto?> GetShareAsync(Guid id, CancellationToken ct) =>
        db.CourseRevenueShares.AsNoTracking().Where(x => x.Oid == id)
            .Select(x => new CourseRevenueShareDto(x.Oid, x.CourseId, x.BeneficiaryUserId,
                x.BeneficiaryUser == null ? null : x.BeneficiaryUser.Username, x.ShareTypeLookupId,
                x.ShareType.LookupNameEn ?? x.ShareType.LookupValue, x.CalculationType, x.Value,
                x.IsActive, x.EffectiveFrom, x.EffectiveTo, x.Notes)).SingleOrDefaultAsync(ct);

    private Task<RevenueSettlementDto?> GetSettlementAsync(Guid id, CancellationToken ct) =>
        db.RevenueSettlements.AsNoTracking().Where(x => x.Oid == id)
            .Select(x => new RevenueSettlementDto(x.Oid, x.BeneficiaryUserId, x.BeneficiaryUser.Username,
                x.SettlementNumber, x.PeriodFrom, x.PeriodTo, x.TotalAmount, x.Status,
                x.PaidAt, x.PaymentReference, x.Notes)).SingleOrDefaultAsync(ct);
}

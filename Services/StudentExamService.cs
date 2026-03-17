using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class StudentExamService : IStudentExamService
    {
        private readonly IStudentExamRepository _studentExamRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICoursesMasterExamRepository _masterExamRepository;
        private readonly IAppLookupDetailRepository _lookupDetailRepository;
        private readonly IStudentExamQuestionRepository _studentExamQuestionRepository;
        private readonly ICourseQuestionRepository _courseQuestionRepository;
        private readonly ICourseAnswerRepository _courseAnswerRepository;

        public StudentExamService(
            IStudentExamRepository studentExamRepository,
            IStudentRepository studentRepository,
            ICoursesMasterExamRepository masterExamRepository,
            IAppLookupDetailRepository lookupDetailRepository,
            IStudentExamQuestionRepository studentExamQuestionRepository,
            ICourseQuestionRepository courseQuestionRepository,
            ICourseAnswerRepository courseAnswerRepository)
        {
            _studentExamRepository = studentExamRepository;
            _studentRepository = studentRepository;
            _masterExamRepository = masterExamRepository;
            _lookupDetailRepository = lookupDetailRepository;
            _studentExamQuestionRepository = studentExamQuestionRepository;
            _courseQuestionRepository = courseQuestionRepository;
            _courseAnswerRepository = courseAnswerRepository;
        }

        public async Task<PagedResponse<StudentExamDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _studentExamRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<StudentExamDto>
                {
                    Success = true,
                    Data = dtos,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };
            }
            catch (Exception ex)
            {
                return new PagedResponse<StudentExamDto>
                {
                    Success = false,
                    Message = $"Error retrieving student exams: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<StudentExamDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var studentExam = await _studentExamRepository.GetByIdAsync(id);
                if (studentExam == null)
                    return ApiResponse<StudentExamDto>.ErrorResponse("Student exam not found");

                return ApiResponse<StudentExamDto>.SuccessResponse(MapToDto(studentExam));
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentExamDto>.ErrorResponse($"Error retrieving student exam: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<StudentExamDto>>> GetByStudentIdAsync(Guid studentId)
        {
            try
            {
                var studentExams = await _studentExamRepository.GetByStudentIdAsync(studentId);
                var dtos = studentExams.Select(MapToDto).ToList();

                return ApiResponse<List<StudentExamDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<StudentExamDto>>.ErrorResponse($"Error retrieving student exams: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentExamDto>> GetWithQuestionsAsync(Guid id)
        {
            try
            {
                var studentExam = await _studentExamRepository.GetWithQuestionsAsync(id);
                if (studentExam == null)
                    return ApiResponse<StudentExamDto>.ErrorResponse("Student exam not found");

                return ApiResponse<StudentExamDto>.SuccessResponse(MapToDtoWithQuestions(studentExam));
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentExamDto>.ErrorResponse($"Error retrieving student exam: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentExamDto>> StartExamAsync(CreateStudentExamDto dto)
        {
            try
            {
                // Validate Student exists
                var studentExists = await _studentRepository.ExistsAsync(s => s.Oid == dto.StudentOid && !s.IsDeleted);
                if (!studentExists)
                    return ApiResponse<StudentExamDto>.ErrorResponse("Invalid Student. Please select a valid student.");

                // Validate Master Exam exists
                var masterExam = await _masterExamRepository.GetByIdAsync(dto.CoursesMasterExamOid);
                if (masterExam == null)
                    return ApiResponse<StudentExamDto>.ErrorResponse("Invalid Master Exam. Please select a valid exam.");

                // Check max attempts
                var attemptCount = await _studentExamRepository.GetAttemptCountAsync(dto.StudentOid, dto.CoursesMasterExamOid);
                if (masterExam.MaxAttempts.HasValue && attemptCount >= masterExam.MaxAttempts.Value)
                    return ApiResponse<StudentExamDto>.ErrorResponse($"Maximum attempts ({masterExam.MaxAttempts.Value}) reached for this exam.");

                // Create student exam
                var studentExam = new StudentExam
                {
                    StudentOid = dto.StudentOid,
                    CoursesMasterExamOid = dto.CoursesMasterExamOid,
                    AttemptNo = attemptCount + 1,
                    PassPercent = masterExam.PassPercent,
                    ExamModeLookupId = dto.ExamModeLookupId,
                    StartedAt = DateTime.UtcNow,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var createdExam = await _studentExamRepository.AddAsync(studentExam);

                // Get questions for this exam
                var questions = await _courseQuestionRepository.GetByMasterExamIdAsync(dto.CoursesMasterExamOid);
                
                // Shuffle questions if required
                if (masterExam.ShuffleQuestions)
                {
                    questions = questions.OrderBy(x => Guid.NewGuid()).ToList();
                }

                // Create student exam questions
                var questionStatusNotAnswered = Guid.Parse("44444444-4444-4444-4444-444444444403");
                int totalScore = 0;
                foreach (var question in questions)
                {
                    var studentExamQuestion = new StudentExamQuestion
                    {
                        StudentExamOid = createdExam.Oid,
                        QuestionOid = question.Oid,
                        QuestionStatusLookupId = questionStatusNotAnswered,
                        QuestionScore = question.QuestionScore,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _studentExamQuestionRepository.AddAsync(studentExamQuestion);
                    totalScore += question.QuestionScore ;
                }

                // Update total score
                createdExam.TotalScore = 0;
                await _studentExamRepository.UpdateAsync(createdExam);

                return ApiResponse<StudentExamDto>.SuccessResponse(MapToDto(createdExam), "Exam started successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentExamDto>.ErrorResponse($"Error starting exam: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentExamDto>> SubmitExamAsync(SubmitStudentExamDto dto)
        {
            try
            {
                var studentExam = await _studentExamRepository.GetWithQuestionsAsync(dto.StudentExamOid);
                if (studentExam == null)
                    return ApiResponse<StudentExamDto>.ErrorResponse("Student exam not found");

                // Calculate scores from existing question rows
                int totalScore = studentExam.ExamQuestions
                    .Where(q => !q.IsDeleted)
                    .Sum(q => q.QuestionScore ?? 0);

                int obtainedScore = studentExam.ExamQuestions
                    .Where(q => !q.IsDeleted)
                    .Sum(q => q.ObtainedScore ?? 0);

                // Calculate percentage and pass status
                decimal percentage = totalScore > 0
                    ? (decimal)obtainedScore / totalScore * 100
                    : 0;
                bool isPassed = percentage >= (studentExam.PassPercent ?? 60);

                // Update student exam
                studentExam.TotalScore = totalScore;
                studentExam.ObtainedScore = obtainedScore;
                studentExam.IsPassed = isPassed;
                studentExam.FinishedAt = DateTime.UtcNow;
                studentExam.UpdatedBy = dto.UpdatedBy;
                studentExam.UpdatedAt = DateTime.UtcNow;

                var updatedExam = await _studentExamRepository.UpdateAsync(studentExam);

                var resultDto = MapToDto(updatedExam);
                resultDto.Percentage = Math.Round(percentage, 2);

                var message = isPassed
                    ? $"Exam submitted successfully. You passed! Score: {obtainedScore}/{totalScore} ({Math.Round(percentage, 2)}%)"
                    : $"Exam submitted successfully. You did not pass. Score: {obtainedScore}/{totalScore} ({Math.Round(percentage, 2)}%)";

                return ApiResponse<StudentExamDto>.SuccessResponse(resultDto, message);
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentExamDto>.ErrorResponse($"Error submitting exam: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _studentExamRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Student exam not found");

                return ApiResponse<bool>.SuccessResponse(true, "Student exam deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting student exam: {ex.Message}");
            }
        }

        private StudentExamDto MapToDto(StudentExam exam)
        {
            return new StudentExamDto
            {
                Oid = exam.Oid,
                StudentOid = exam.StudentOid,
                StudentName = exam.Student?.NameEn ?? exam.Student?.Username,
                CoursesMasterExamOid = exam.CoursesMasterExamOid,
                ExamName = exam.MasterExam?.CourseName,
                AttemptNo = exam.AttemptNo,
                TotalScore = exam.TotalScore,
                ObtainedScore = exam.ObtainedScore,
                PassPercent = exam.PassPercent,
                IsPassed = exam.IsPassed,
                ExamStatusLookupId = exam.ExamStatusLookupId,
                ExamStatusName = exam.ExamStatusLookup?.LookupNameEn,
                ExamModeLookupId = exam.ExamModeLookupId,
                ExamModeName = exam.ExamModeLookup?.LookupNameEn,
                StartedAt = exam.StartedAt,
                FinishedAt = exam.FinishedAt,
                CreatedAt = exam.CreatedAt,
                CreatedBy = exam.CreatedBy,
                UpdatedAt = exam.UpdatedAt,
                UpdatedBy = exam.UpdatedBy
            };
        }

        private StudentExamDto MapToDtoWithQuestions(StudentExam exam)
        {
            return new StudentExamDto
            {
                Oid = exam.Oid,
                StudentOid = exam.StudentOid,
                StudentName = exam.Student?.NameEn ?? exam.Student?.Username,
                CoursesMasterExamOid = exam.CoursesMasterExamOid,
                ExamName = exam.MasterExam?.CourseName,
                AttemptNo = exam.AttemptNo,
                TotalScore = exam.TotalScore,
                ObtainedScore = exam.ObtainedScore,
                PassPercent = exam.PassPercent,
                IsPassed = exam.IsPassed,
                ExamStatusLookupId = exam.ExamStatusLookupId,
                ExamStatusName = exam.ExamStatusLookup?.LookupNameEn,
                ExamModeLookupId = exam.ExamModeLookupId,
                ExamModeName = exam.ExamModeLookup?.LookupNameEn,
                StartedAt = exam.StartedAt,
                FinishedAt = exam.FinishedAt,
                CreatedAt = exam.CreatedAt,
                CreatedBy = exam.CreatedBy,
                UpdatedAt = exam.UpdatedAt,
                UpdatedBy = exam.UpdatedBy,
                ExamQuestions = exam.ExamQuestions.Select(q => new StudentExamQuestionDto
                {
                    Oid = q.Oid,
                    StudentExamOid = q.StudentExamOid,
                    QuestionOid = q.QuestionOid,
                    QuestionText = q.Question?.QuestionText,
                    QuestionText_Ar = q.Question?.QuestionText_Ar ?? string.Empty,
                    QuestionExplination = q.Question?.QuestionExplination ?? string.Empty,
                    ExamName = q.Question?.MasterExam?.CourseName,
                    QuestionTypeLookupId = q.Question?.QuestionTypeLookupId,
                    QuestionTypeName = q.Question?.QuestionTypeLookup?.LookupNameEn,
                    OrderNo = q.Question?.OrderNo,
                    IsActive = q.Question?.IsActive ?? false,
                    CorrectAnswer = q.Question?.CorrectAnswer ?? false,
                    Question = q.Question?.Question ?? false,
                    CorrectChoiceOid = q.Question?.CorrectChoiceOid,
                    IsCorrect = q.IsCorrect,
                    QuestionScore = q.QuestionScore,
                    ObtainedScore = q.ObtainedScore,
                    QuestionStatusLookupId = q.QuestionStatusLookupId,
                    QuestionStatusName = q.QuestionStatus?.LookupNameEn,
                    Answers = q.Answers?.Where(a => !a.IsDeleted).Select(a => new StudentExamQuestionAnswerDto
                    {
                        Oid = a.Oid,
                        SelectedAnswerOid = a.SelectedAnswerOid,
                        SelectedAnswerText = a.SelectedAnswer?.AnswerText,
                        AnswerSelectedAnswerOid = a.AnswerSelectedAnswerOid
                    }).ToList() ?? new List<StudentExamQuestionAnswerDto>(),
                    CreatedAt = q.CreatedAt,
                    CreatedBy = q.CreatedBy,
                    UpdatedAt = q.UpdatedAt,
                    UpdatedBy = q.UpdatedBy
                }).ToList(),
                StatusSummary = CalcStatusSummary(exam.ExamQuestions)
            };
        }

        private static List<QuestionStatusSummaryDto> CalcStatusSummary(IEnumerable<StudentExamQuestion> questions)
        {
            var list = questions.ToList();
            int total = list.Count;
            if (total == 0) return new();

            return list
                .GroupBy(q => new { q.QuestionStatusLookupId, StatusName = q.QuestionStatus?.LookupNameEn })
                .Select(g => new QuestionStatusSummaryDto
                {
                    QuestionStatusLookupId = g.Key.QuestionStatusLookupId,
                    StatusName = g.Key.StatusName,
                    Count = g.Count(),
                    Percentage = Math.Round((decimal)g.Count() / total * 100, 2)
                })
                .OrderBy(s => s.StatusName)
                .ToList();
        }

        public async Task<ApiResponse<StudentExamSummaryDto>> GetStudentExamSummaryAsync(Guid studentId, Guid examId)
        {
            try
            {
                // Get the last exam for this student and master exam
                var studentExams = await _studentExamRepository.GetByStudentIdAsync(studentId);
                var lastExam = studentExams
                    .Where(e => e.CoursesMasterExamOid == examId)
                    .FirstOrDefault();
                if (lastExam == null)
                    return ApiResponse<StudentExamSummaryDto>.ErrorResponse("No exams found for this student and exam");

                // Get questions with status for the last exam
                var examQuestions = await _studentExamQuestionRepository.GetByStudentExamIdAsync(lastExam.Oid);

                int totalQuestions = examQuestions.Count;
                int totalScore = examQuestions.Sum(q => q.QuestionScore ?? 0);
                int obtainedScore = examQuestions.Sum(q => q.ObtainedScore ?? 0);
                decimal percentage = totalScore > 0
                    ? Math.Round((decimal)obtainedScore / totalScore * 100, 2)
                    : 0;

                var summary = new StudentExamSummaryDto
                {
                    StudentExamOid = lastExam.Oid,
                    StudentOid = lastExam.StudentOid,
                    StudentName = lastExam.Student?.NameEn ?? lastExam.Student?.Username,
                    ExamName = lastExam.MasterExam?.CourseName,
                    AttemptNo = lastExam.AttemptNo,
                    TotalScore = totalScore,
                    ObtainedScore = obtainedScore,
                    Percentage = percentage,
                    IsPassed = lastExam.IsPassed,
                    ExamModeLookupId = lastExam.ExamModeLookupId,
                    ExamModeName = lastExam.ExamModeLookup?.LookupNameEn,
                    StartedAt = lastExam.StartedAt,
                    FinishedAt = lastExam.FinishedAt,
                    TotalQuestions = totalQuestions,
                    StatusSummary = CalcStatusSummary(examQuestions)
                };

                return ApiResponse<StudentExamSummaryDto>.SuccessResponse(summary);
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentExamSummaryDto>.ErrorResponse($"Error retrieving exam summary: {ex.Message}");
            }
        }
    }
}
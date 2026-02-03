using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class StudentExamQuestionService : IStudentExamQuestionService
    {
        private readonly IStudentExamQuestionRepository _studentExamQuestionRepository;
        private readonly IStudentExamRepository _studentExamRepository;
        private readonly ICourseQuestionRepository _courseQuestionRepository;
        private readonly ICourseAnswerRepository _courseAnswerRepository;

        public StudentExamQuestionService(
            IStudentExamQuestionRepository studentExamQuestionRepository,
            IStudentExamRepository studentExamRepository,
            ICourseQuestionRepository courseQuestionRepository,
            ICourseAnswerRepository courseAnswerRepository)
        {
            _studentExamQuestionRepository = studentExamQuestionRepository;
            _studentExamRepository = studentExamRepository;
            _courseQuestionRepository = courseQuestionRepository;
            _courseAnswerRepository = courseAnswerRepository;
        }

        public async Task<PagedResponse<StudentExamQuestionDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _studentExamQuestionRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<StudentExamQuestionDto>
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
                return new PagedResponse<StudentExamQuestionDto>
                {
                    Success = false,
                    Message = $"Error retrieving student exam questions: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<StudentExamQuestionDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var examQuestion = await _studentExamQuestionRepository.GetByIdAsync(id);
                if (examQuestion == null)
                    return ApiResponse<StudentExamQuestionDto>.ErrorResponse("Student exam question not found");

                return ApiResponse<StudentExamQuestionDto>.SuccessResponse(MapToDto(examQuestion));
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentExamQuestionDto>.ErrorResponse($"Error retrieving student exam question: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<StudentExamQuestionDto>>> GetByStudentExamIdAsync(Guid studentExamId)
        {
            try
            {
                var examQuestions = await _studentExamQuestionRepository.GetByStudentExamIdAsync(studentExamId);
                var dtos = examQuestions.Select(MapToDto).ToList();

                return ApiResponse<List<StudentExamQuestionDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<StudentExamQuestionDto>>.ErrorResponse($"Error retrieving student exam questions: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentExamQuestionDto>> CreateAsync(CreateStudentExamQuestionDto dto)
        {
            try
            {
                // Validate Student Exam exists
                var examExists = await _studentExamRepository.ExistsAsync(e => e.Oid == dto.StudentExamOid && !e.IsDeleted);
                if (!examExists)
                    return ApiResponse<StudentExamQuestionDto>.ErrorResponse("Invalid Student Exam. Please select a valid exam.");

                // Validate Question exists
                var question = await _courseQuestionRepository.GetByIdAsync(dto.QuestionOid);
                if (question == null)
                    return ApiResponse<StudentExamQuestionDto>.ErrorResponse("Invalid Question. Please select a valid question.");

                var examQuestion = new StudentExamQuestion
                {
                    StudentExamOid = dto.StudentExamOid,
                    QuestionOid = dto.QuestionOid,
                    SelectedAnswerOid = dto.SelectedAnswerOid,
                    QuestionScore = question.QuestionScore,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var createdExamQuestion = await _studentExamQuestionRepository.AddAsync(examQuestion);
                return ApiResponse<StudentExamQuestionDto>.SuccessResponse(MapToDto(createdExamQuestion), "Student exam question created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentExamQuestionDto>.ErrorResponse($"Error creating student exam question: {ex.Message}");
            }
        }

        public async Task<ApiResponse<StudentExamQuestionDto>> UpdateAsync(UpdateStudentExamQuestionDto dto)
        {
            try
            {
                var examQuestion = await _studentExamQuestionRepository.GetByIdAsync(dto.Oid);
                if (examQuestion == null)
                    return ApiResponse<StudentExamQuestionDto>.ErrorResponse("Student exam question not found");

                // Validate answer if provided
                if (dto.SelectedAnswerOid.HasValue)
                {
                    var answerExists = await _courseAnswerRepository.ExistsAsync(
                        a => a.Oid == dto.SelectedAnswerOid.Value && !a.IsDeleted);
                    if (!answerExists)
                        return ApiResponse<StudentExamQuestionDto>.ErrorResponse("Invalid Answer. Please select a valid answer.");
                }

                examQuestion.SelectedAnswerOid = dto.SelectedAnswerOid;
                examQuestion.IsCorrect = dto.IsCorrect;
                examQuestion.QuestionScore = dto.QuestionScore;
                examQuestion.ObtainedScore = dto.ObtainedScore;
                examQuestion.UpdatedBy = dto.UpdatedBy;
                examQuestion.UpdatedAt = DateTime.UtcNow;

                var updatedExamQuestion = await _studentExamQuestionRepository.UpdateAsync(examQuestion);
                return ApiResponse<StudentExamQuestionDto>.SuccessResponse(MapToDto(updatedExamQuestion), "Student exam question updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<StudentExamQuestionDto>.ErrorResponse($"Error updating student exam question: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _studentExamQuestionRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Student exam question not found");

                return ApiResponse<bool>.SuccessResponse(true, "Student exam question deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting student exam question: {ex.Message}");
            }
        }

        private StudentExamQuestionDto MapToDto(StudentExamQuestion examQuestion)
        {
            return new StudentExamQuestionDto
            {
                Oid = examQuestion.Oid,
                StudentExamOid = examQuestion.StudentExamOid,
                QuestionOid = examQuestion.QuestionOid,
                QuestionText = examQuestion.Question?.QuestionText,
                SelectedAnswerOid = examQuestion.SelectedAnswerOid,
                SelectedAnswerText = examQuestion.SelectedAnswer?.AnswerText,
                IsCorrect = examQuestion.IsCorrect,
                QuestionScore = examQuestion.QuestionScore,
                ObtainedScore = examQuestion.ObtainedScore,
                CreatedAt = examQuestion.CreatedAt,
                CreatedBy = examQuestion.CreatedBy,
                UpdatedAt = examQuestion.UpdatedAt,
                UpdatedBy = examQuestion.UpdatedBy
            };
        }
    }
}
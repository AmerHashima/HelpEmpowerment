using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseAnswerService : ICourseAnswerService
    {
        private readonly ICourseAnswerRepository _answerRepository;
        private readonly ICourseQuestionRepository _questionRepository;

        public CourseAnswerService(
            ICourseAnswerRepository answerRepository,
            ICourseQuestionRepository questionRepository)
        {
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
        }

        public async Task<PagedResponse<CourseAnswerDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _answerRepository.GetPagedAsync(request);
                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseAnswerDto>
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
                return new PagedResponse<CourseAnswerDto>
                {
                    Success = false,
                    Message = $"Error retrieving answers: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseAnswerDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var answer = await _answerRepository.GetByIdAsync(id);
                if (answer == null)
                    return ApiResponse<CourseAnswerDto>.ErrorResponse("Answer not found");

                return ApiResponse<CourseAnswerDto>.SuccessResponse(MapToDto(answer));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseAnswerDto>.ErrorResponse($"Error retrieving answer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseAnswerDto>>> GetByQuestionIdAsync(Guid questionId)
        {
            try
            {
                var answers = await _answerRepository.GetByQuestionIdAsync(questionId);
                var dtos = answers.Select(MapToDto).ToList();

                return ApiResponse<List<CourseAnswerDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseAnswerDto>>.ErrorResponse($"Error retrieving answers: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseAnswerDto>> GetCorrectAnswerByQuestionIdAsync(Guid questionId)
        {
            try
            {
                var answer = await _answerRepository.GetCorrectAnswerByQuestionIdAsync(questionId);
                if (answer == null)
                    return ApiResponse<CourseAnswerDto>.ErrorResponse("Correct answer not found for this question");

                return ApiResponse<CourseAnswerDto>.SuccessResponse(MapToDto(answer));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseAnswerDto>.ErrorResponse($"Error retrieving correct answer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseAnswerDto>> CreateAsync(CreateCourseAnswerDto dto)
        {
            try
            {
                // Validate question exists
                var questionExists = await _questionRepository.ExistsAsync(q => q.Oid == dto.QuestionId && !q.IsDeleted);
                if (!questionExists)
                    return ApiResponse<CourseAnswerDto>.ErrorResponse("Invalid Question. Please select a valid question.");

                // If this answer is marked as correct, unmark other answers for the same question
                if (dto.IsCorrect)
                {
                    var existingAnswers = await _answerRepository.GetByQuestionIdAsync(dto.QuestionId);
                    foreach (var existingAnswer in existingAnswers.Where(a => a.IsCorrect))
                    {
                        existingAnswer.IsCorrect = false;
                        existingAnswer.UpdatedAt = DateTime.UtcNow;
                        await _answerRepository.UpdateAsync(existingAnswer);
                    }
                }

                var answer = new CourseAnswer
                {
                    QuestionId = dto.QuestionId,
                    AnswerText = dto.AnswerText,
                    IsCorrect = dto.IsCorrect,
                    OrderNo = dto.OrderNo,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _answerRepository.AddAsync(answer);
                return ApiResponse<CourseAnswerDto>.SuccessResponse(MapToDto(created), "Answer created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseAnswerDto>.ErrorResponse($"Error creating answer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseAnswerDto>> UpdateAsync(UpdateCourseAnswerDto dto)
        {
            try
            {
                var answer = await _answerRepository.GetByIdAsync(dto.Oid);
                if (answer == null)
                    return ApiResponse<CourseAnswerDto>.ErrorResponse("Answer not found");

                // Validate question exists
                var questionExists = await _questionRepository.ExistsAsync(q => q.Oid == dto.QuestionId && !q.IsDeleted);
                if (!questionExists)
                    return ApiResponse<CourseAnswerDto>.ErrorResponse("Invalid Question. Please select a valid question.");

                // If this answer is being marked as correct, unmark other answers for the same question
                //if (dto.IsCorrect && !answer.IsCorrect)
                //{
                //    var existingAnswers = await _answerRepository.GetByQuestionIdAsync(dto.QuestionId);
                //    foreach (var existingAnswer in existingAnswers.Where(a => a.IsCorrect && a.Oid != dto.Oid))
                //    {
                //        existingAnswer.IsCorrect = false;
                //        existingAnswer.UpdatedAt = DateTime.UtcNow;
                //        await _answerRepository.UpdateAsync(existingAnswer);
                //    }
                //}

                answer.QuestionId = dto.QuestionId;
                answer.CorrectAnswerOid = dto.CorrectAnswerOid;
                answer.AnswerText = dto.AnswerText;
                answer.IsCorrect = dto.IsCorrect;
                answer.OrderNo = dto.OrderNo;
                answer.UpdatedBy = dto.UpdatedBy;
                answer.UpdatedAt = DateTime.UtcNow;

                var updated = await _answerRepository.UpdateAsync(answer);
                return ApiResponse<CourseAnswerDto>.SuccessResponse(MapToDto(updated), "Answer updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseAnswerDto>.ErrorResponse($"Error updating answer: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _answerRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Answer not found");

                return ApiResponse<bool>.SuccessResponse(true, "Answer deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting answer: {ex.Message}");
            }
        }

        private static CourseAnswerDto MapToDto(CourseAnswer answer)
        {
            return new CourseAnswerDto
            {
                Oid = answer.Oid,
                QuestionId = answer.QuestionId,
                AnswerText = answer.AnswerText,
                IsCorrect = answer.IsCorrect,
                OrderNo = answer.OrderNo,
                CorrectAnswerOid = answer.CorrectAnswerOid,
                CreatedAt = answer.CreatedAt,
                CreatedBy = answer.CreatedBy,
                UpdatedAt = answer.UpdatedAt,
                UpdatedBy = answer.UpdatedBy
            };
        }
    }
}
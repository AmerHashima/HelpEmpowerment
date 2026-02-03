using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class CourseQuestionService : ICourseQuestionService
    {
        private readonly ICourseQuestionRepository _questionRepository;
        private readonly ICourseAnswerRepository _answerRepository;
        private readonly IAppLookupDetailRepository _lookupDetailRepository;
        private readonly ICoursesMasterExamRepository _examRepository;

        public CourseQuestionService(
            ICourseQuestionRepository questionRepository, 
            ICourseAnswerRepository answerRepository,
            IAppLookupDetailRepository lookupDetailRepository,
            ICoursesMasterExamRepository examRepository)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _lookupDetailRepository = lookupDetailRepository;
            _examRepository = examRepository;
        }

        public async Task<PagedResponse<CourseQuestionDto>> GetPagedAsync(DataRequest request)
        {
            try
            {
                var pagedResult = await _questionRepository.GetPagedAsync(request);

                var dtos = pagedResult.Items.Select(MapToDto).ToList();

                return new PagedResponse<CourseQuestionDto>
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
                return new PagedResponse<CourseQuestionDto>
                {
                    Success = false,
                    Message = $"Error retrieving questions: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<CourseQuestionDto>> GetByIdAsync(Guid id)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    return ApiResponse<CourseQuestionDto>.ErrorResponse("Question not found");

                return ApiResponse<CourseQuestionDto>.SuccessResponse(MapToDto(question));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseQuestionDto>.ErrorResponse($"Error retrieving question: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseQuestionDto>>> GetByExamIdAsync(Guid examId)
        {
            try
            {
                var questions = await _questionRepository.GetByExamIdAsync(examId);
                var dtos = questions.Select(MapToDto).ToList();

                return ApiResponse<List<CourseQuestionDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseQuestionDto>>.ErrorResponse($"Error retrieving questions: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseQuestionDto>> GetWithAnswersAsync(Guid id)
        {
            try
            {
                var question = await _questionRepository.GetWithAnswersAsync(id);
                if (question == null)
                    return ApiResponse<CourseQuestionDto>.ErrorResponse("Question not found");

                return ApiResponse<CourseQuestionDto>.SuccessResponse(MapToDto(question));
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseQuestionDto>.ErrorResponse($"Error retrieving question: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CourseQuestionDto>>> GetWithAnswersByExamIdAsync(Guid examId)
        {
            try
            {
                var questions = await _questionRepository.GetWithAnswersByExamIdAsync(examId);
                var dtos = questions.Select(MapToDto).ToList();

                return ApiResponse<List<CourseQuestionDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseQuestionDto>>.ErrorResponse($"Error retrieving questions: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseQuestionDto>> CreateAsync(CreateCourseQuestionDto dto)
        {
            try
            {
                // Validate exam exists
                var exam = await _examRepository.GetByIdAsync(dto.CoursesMasterExamOid);
                if (exam == null)
                    return ApiResponse<CourseQuestionDto>.ErrorResponse("Exam not found");

                // Validate Question Type Lookup if provided
                if (dto.QuestionTypeLookupId.HasValue)
                {
                    var questionTypeExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.QuestionTypeLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!questionTypeExists)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("Invalid Question Type. Please select a valid question type.");
                }

                // Validate Correct Choice if provided
                if (dto.CorrectChoiceOid.HasValue)
                {
                    var correctChoiceExists = await _questionRepository.ExistsAsync(
                        q => q.Oid == dto.CorrectChoiceOid.Value && !q.IsDeleted);
                    if (!correctChoiceExists)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("Invalid Correct Choice. The referenced question does not exist.");
                }

                var question = new CourseQuestion
                {
                    CoursesMasterExamOid = dto.CoursesMasterExamOid,
                    QuestionText = dto.QuestionText,
                    QuestionTypeLookupId = dto.QuestionTypeLookupId,
                    QuestionScore = dto.QuestionScore,
                    OrderNo = dto.OrderNo,
                    IsActive = dto.IsActive,
                    CorrectAnswer = dto.CorrectAnswer,
                    Question = dto.Question,
                    CorrectChoiceOid = dto.CorrectChoiceOid,
                    CreatedBy = dto.CreatedBy
                };

                var created = await _questionRepository.AddAsync(question);

                // Add answers if provided
                if (dto.Answers != null && dto.Answers.Any())
                {
                    foreach (var answerDto in dto.Answers)
                    {
                        var answer = new CourseAnswer
                        {
                            QuestionId = created.Oid,
                            CorrectAnswerOid = answerDto.CorrectAnswerOid,
                            Question_Ask = answerDto.Question_Ask,
                            AnswerText = answerDto.AnswerText,
                            IsCorrect = answerDto.IsCorrect,
                            OrderNo = answerDto.OrderNo,
                            CreatedBy = answerDto.CreatedBy
                        };
                        await _answerRepository.AddAsync(answer);
                    }
                }

                var result = await _questionRepository.GetWithAnswersAsync(created.Oid);
                return ApiResponse<CourseQuestionDto>.SuccessResponse(MapToDto(result!), "Question created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseQuestionDto>.ErrorResponse($"Error creating question: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseQuestionDto>> UpdateAsync(UpdateCourseQuestionDto dto)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(dto.Oid);
                if (question == null)
                    return ApiResponse<CourseQuestionDto>.ErrorResponse("Question not found");

                // Validate Question Type Lookup if provided
                if (dto.QuestionTypeLookupId.HasValue)
                {
                    var questionTypeExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.QuestionTypeLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!questionTypeExists)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("Invalid Question Type. Please select a valid question type.");
                }

                // Validate Correct Choice if provided
                if (dto.CorrectChoiceOid.HasValue)
                {
                    // Prevent self-reference
                    if (dto.CorrectChoiceOid.Value == dto.Oid)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("A question cannot reference itself as the correct choice.");

                    var correctChoiceExists = await _questionRepository.ExistsAsync(
                        q => q.Oid == dto.CorrectChoiceOid.Value && !q.IsDeleted);
                    if (!correctChoiceExists)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("Invalid Correct Choice. The referenced question does not exist.");
                }

                question.CoursesMasterExamOid = dto.CoursesMasterExamOid;
                question.QuestionText = dto.QuestionText;
                question.QuestionTypeLookupId = dto.QuestionTypeLookupId;
                question.QuestionScore = dto.QuestionScore;
                question.OrderNo = dto.OrderNo;
                question.IsActive = dto.IsActive;
                question.CorrectAnswer = dto.CorrectAnswer;
                question.Question = dto.Question;
                question.CorrectChoiceOid = dto.CorrectChoiceOid;
                question.UpdatedBy = dto.UpdatedBy;

                var updated = await _questionRepository.UpdateAsync(question);
                return ApiResponse<CourseQuestionDto>.SuccessResponse(MapToDto(updated), "Question updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseQuestionDto>.ErrorResponse($"Error updating question: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _questionRepository.SoftDeleteAsync(id);
                if (!result)
                    return ApiResponse<bool>.ErrorResponse("Question not found");

                return ApiResponse<bool>.SuccessResponse(true, "Question deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting question: {ex.Message}");
            }
        }

        private static CourseQuestionDto MapToDto(CourseQuestion question)
        {
            return new CourseQuestionDto
            {
                Oid = question.Oid,
                CoursesMasterExamOid = question.CoursesMasterExamOid,
                ExamName = question.MasterExam?.CourseName,
                QuestionText = question.QuestionText,
                QuestionTypeLookupId = question.QuestionTypeLookupId,
                QuestionTypeName = question.QuestionTypeLookup?.LookupNameEn,
                QuestionScore = question.QuestionScore,
                OrderNo = question.OrderNo,
                IsActive = question.IsActive,
                CorrectAnswer = question.CorrectAnswer,
                Question = question.Question,
                CorrectChoiceOid = question.CorrectChoiceOid,
                Answers = question.Answers?.Select(a => new CourseAnswerDto
                {
                    Oid = a.Oid,
                    QuestionId = a.QuestionId,
                    AnswerText = a.AnswerText,
                    CorrectAnswerOid = a.CorrectAnswerOid,
                    Question_Ask = a.Question_Ask,
                    IsCorrect = a.IsCorrect,
                    OrderNo = a.OrderNo,
                    CreatedAt = a.CreatedAt,
                    CreatedBy = a.CreatedBy,
                    UpdatedAt = a.UpdatedAt,
                    UpdatedBy = a.UpdatedBy
                }).ToList() ?? new(),
                CreatedAt = question.CreatedAt,
                CreatedBy = question.CreatedBy,
                UpdatedAt = question.UpdatedAt,
                UpdatedBy = question.UpdatedBy
            };
        }
    }
}
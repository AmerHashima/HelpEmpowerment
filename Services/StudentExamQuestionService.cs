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

                // Group by question to combine multiple answer rows
                var groupedQuestions = examQuestions
                    .GroupBy(eq => eq.QuestionOid)
                    .Select(g =>
                    {
                        var first = g.First();
                        var dto = MapToDto(first);
                        // Collect all selected answer IDs for this question
                        dto.SelectedAnswerOids = g
                            .Where(eq => eq.SelectedAnswerOid.HasValue)
                            .Select(eq => eq.SelectedAnswerOid!.Value)
                            .ToList();
                        return dto;
                    })
                    .ToList();

                return ApiResponse<List<StudentExamQuestionDto>>.SuccessResponse(groupedQuestions);
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

                if (dto.SelectedAnswerOids == null || !dto.SelectedAnswerOids.Any())
                    return ApiResponse<StudentExamQuestionDto>.ErrorResponse("Please select at least one answer.");

                // Get all answers for the question
                var allAnswers = await _courseAnswerRepository.GetByQuestionIdAsync(dto.QuestionOid);

                // Validate all selected answers exist
                var invalidAnswers = dto.SelectedAnswerOids.Where(id => !allAnswers.Any(a => a.Oid == id)).ToList();
                if (invalidAnswers.Any())
                    return ApiResponse<StudentExamQuestionDto>.ErrorResponse("Invalid Answer(s) selected. Please select valid answers.");

                // Get correct answers
                var correctAnswerOids = allAnswers.Where(a => a.IsCorrect).Select(a => a.Oid).ToList();

                // Check if selected answers match correct answers exactly
                var selectedSet = dto.SelectedAnswerOids.OrderBy(x => x).ToList();
                var correctSet = correctAnswerOids.OrderBy(x => x).ToList();
                bool isCorrect = selectedSet.SequenceEqual(correctSet);
                int obtainedScore = isCorrect ? question.QuestionScore : 0;

                // Delete existing answers for this question
                var existingAnswers = await _studentExamQuestionRepository.GetByStudentExamIdAsync(dto.StudentExamOid);
                var existingForQuestion = existingAnswers.Where(eq => eq.QuestionOid == dto.QuestionOid).ToList();
                foreach (var existing in existingForQuestion)
                {
                    await _studentExamQuestionRepository.SoftDeleteAsync(existing.Oid);
                }

                // Create multiple rows - one for each selected answer
                StudentExamQuestion? firstCreatedQuestion = null;
                foreach (var answerId in dto.SelectedAnswerOids)
                {
                    var examQuestion = new StudentExamQuestion
                    {
                        StudentExamOid = dto.StudentExamOid,
                        QuestionOid = dto.QuestionOid,
                        SelectedAnswerOid = answerId,
                        IsCorrect = isCorrect,
                        QuestionScore = question.QuestionScore,
                        ObtainedScore = obtainedScore,
                        CreatedBy = dto.CreatedBy,
                        CreatedAt = DateTime.UtcNow
                    };

                    var created = await _studentExamQuestionRepository.AddAsync(examQuestion);
                    if (firstCreatedQuestion == null)
                        firstCreatedQuestion = created;
                }

                return ApiResponse<StudentExamQuestionDto>.SuccessResponse(
                    MapToDto(firstCreatedQuestion!), 
                    "Student exam question created successfully");
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

                // Validate answers if provided
                if (dto.SelectedAnswerOids != null && dto.SelectedAnswerOids.Any())
                {
                    // Get all answers for the question
                    var allAnswers = await _courseAnswerRepository.GetByQuestionIdAsync(examQuestion.QuestionOid);

                    // Validate all selected answers exist
                    var invalidAnswers = dto.SelectedAnswerOids.Where(id => !allAnswers.Any(a => a.Oid == id)).ToList();
                    if (invalidAnswers.Any())
                        return ApiResponse<StudentExamQuestionDto>.ErrorResponse("Invalid Answer(s). Please select valid answers.");

                    // Get correct answers for the question
                    var correctAnswerOids = allAnswers.Where(a => a.IsCorrect).Select(a => a.Oid).ToList();

                    // Check if selected answers match correct answers exactly
                    var selectedSet = dto.SelectedAnswerOids.OrderBy(x => x).ToList();
                    var correctSet = correctAnswerOids.OrderBy(x => x).ToList();
                    var isCorrect = selectedSet.SequenceEqual(correctSet);

                    // Calculate score
                    var obtainedScore = isCorrect ? examQuestion.QuestionScore ?? 0 : 0;

                    // Delete all existing answers for this question
                    var existingAnswers = await _studentExamQuestionRepository.GetByStudentExamIdAsync(examQuestion.StudentExamOid);
                    var existingForQuestion = existingAnswers.Where(eq => eq.QuestionOid == examQuestion.QuestionOid).ToList();
                    foreach (var existing in existingForQuestion)
                    {
                        await _studentExamQuestionRepository.SoftDeleteAsync(existing.Oid);
                    }

                    // Create new rows for each selected answer
                    StudentExamQuestion? firstCreatedQuestion = null;
                    foreach (var answerId in dto.SelectedAnswerOids)
                    {
                        var newExamQuestion = new StudentExamQuestion
                        {
                            StudentExamOid = examQuestion.StudentExamOid,
                            QuestionOid = examQuestion.QuestionOid,
                            SelectedAnswerOid = answerId,
                            IsCorrect = isCorrect,
                            QuestionScore = examQuestion.QuestionScore,
                            ObtainedScore = obtainedScore,
                            CreatedBy = examQuestion.CreatedBy,
                            CreatedAt = examQuestion.CreatedAt,
                            UpdatedBy = dto.UpdatedBy,
                            UpdatedAt = DateTime.UtcNow
                        };

                        var created = await _studentExamQuestionRepository.AddAsync(newExamQuestion);
                        if (firstCreatedQuestion == null)
                            firstCreatedQuestion = created;
                    }

                    return ApiResponse<StudentExamQuestionDto>.SuccessResponse(
                        MapToDto(firstCreatedQuestion!), 
                        "Student exam question updated successfully");
                }
                else if (dto.IsCorrect.HasValue || dto.ObtainedScore.HasValue || dto.QuestionScore.HasValue)
                {
                    // Manual override of values
                    examQuestion.IsCorrect = dto.IsCorrect ?? examQuestion.IsCorrect;
                    examQuestion.QuestionScore = dto.QuestionScore ?? examQuestion.QuestionScore;
                    examQuestion.ObtainedScore = dto.ObtainedScore ?? examQuestion.ObtainedScore;
                    examQuestion.UpdatedBy = dto.UpdatedBy;
                    examQuestion.UpdatedAt = DateTime.UtcNow;

                    var updatedExamQuestion = await _studentExamQuestionRepository.UpdateAsync(examQuestion);
                    return ApiResponse<StudentExamQuestionDto>.SuccessResponse(MapToDto(updatedExamQuestion), "Student exam question updated successfully");
                }

                return ApiResponse<StudentExamQuestionDto>.ErrorResponse("No valid update data provided");
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
                SelectedAnswerOids = new List<Guid>(), // Will be populated by caller if needed
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

        public async Task<ApiResponse<MultipleQuestionsSubmissionResult>> SubmitMultipleQuestionsAsync(SubmitMultipleQuestionsDto dto)
        {
            try
            {
                // Validate Student Exam exists
                var examExists = await _studentExamRepository.ExistsAsync(e => e.Oid == dto.StudentExamOid && !e.IsDeleted);
                if (!examExists)
                    return ApiResponse<MultipleQuestionsSubmissionResult>.ErrorResponse("Invalid Student Exam. Please select a valid exam.");

                var result = new MultipleQuestionsSubmissionResult
                {
                    Success = true,
                    TotalQuestions = dto.Questions.Count,
                    Questions = new List<QuestionResultDto>(),
                    Errors = new List<string>()
                };

                int correctAnswers = 0;
                int totalScore = 0;
                int obtainedScore = 0;

                // Get all existing answers for this exam
                var existingAnswers = await _studentExamQuestionRepository.GetByStudentExamIdAsync(dto.StudentExamOid);

                foreach (var questionSubmission in dto.Questions)
                {
                    try
                    {
                        // Validate Question exists
                        var question = await _courseQuestionRepository.GetByIdAsync(questionSubmission.QuestionOid);
                        if (question == null)
                        {
                            result.Errors.Add($"Question {questionSubmission.QuestionOid} not found");
                            continue;
                        }

                        // Get all answers for the question
                        var allAnswers = await _courseAnswerRepository.GetByQuestionIdAsync(questionSubmission.QuestionOid);
                        var correctAnswerOids = allAnswers.Where(a => a.IsCorrect).Select(a => a.Oid).ToList();

                        // Check if selected answers are correct
                        bool isCorrect = false;
                        int questionObtainedScore = 0;

                        if (questionSubmission.SelectedAnswerOids != null && questionSubmission.SelectedAnswerOids.Any())
                        {
                            // Validate all selected answers exist
                            var invalidAnswers = questionSubmission.SelectedAnswerOids.Where(id => !allAnswers.Any(a => a.Oid == id)).ToList();
                            if (invalidAnswers.Any())
                            {
                                result.Errors.Add($"Invalid answer(s) for question {questionSubmission.QuestionOid}");
                                continue;
                            }

                            // Check if selected answers match correct answers exactly
                            var selectedSet = questionSubmission.SelectedAnswerOids.OrderBy(x => x).ToList();
                            var correctSet = correctAnswerOids.OrderBy(x => x).ToList();
                            isCorrect = selectedSet.SequenceEqual(correctSet);

                            // Calculate score
                            questionObtainedScore = isCorrect ? question.QuestionScore : 0;
                        }

                        // Delete existing answers for this question
                        var existingForQuestion = existingAnswers.Where(eq => eq.QuestionOid == questionSubmission.QuestionOid).ToList();
                        foreach (var existing in existingForQuestion)
                        {
                            await _studentExamQuestionRepository.SoftDeleteAsync(existing.Oid);
                        }

                        // Create multiple rows - one for each selected answer
                        if (questionSubmission.SelectedAnswerOids != null && questionSubmission.SelectedAnswerOids.Any())
                        {
                            foreach (var answerId in questionSubmission.SelectedAnswerOids)
                            {
                                var examQuestion = new StudentExamQuestion
                                {
                                    StudentExamOid = dto.StudentExamOid,
                                    QuestionOid = questionSubmission.QuestionOid,
                                    SelectedAnswerOid = answerId,
                                    IsCorrect = isCorrect,
                                    QuestionScore = question.QuestionScore,
                                    ObtainedScore = questionObtainedScore,
                                    CreatedBy = dto.CreatedBy,
                                    CreatedAt = DateTime.UtcNow
                                };

                                await _studentExamQuestionRepository.AddAsync(examQuestion);
                            }
                        }

                        // Update statistics
                        //if (isCorrect)
                        //    correctAnswers++;

                        //totalScore += question.QuestionScore;
                        //obtainedScore += questionObtainedScore;

                        //result.Questions.Add(new QuestionResultDto
                        //{
                        //    QuestionOid = questionSubmission.QuestionOid,
                        //    QuestionText = question.QuestionText,
                        //    SelectedAnswerOids = questionSubmission.SelectedAnswerOids ?? new List<Guid>(),
                        //    IsCorrect = isCorrect,
                        //    QuestionScore = question.QuestionScore,
                        //    ObtainedScore = questionObtainedScore
                        //});
                        //if (isCorrect)
                        //    correctAnswers++;

                        //totalScore += question.QuestionScore;
                        //obtainedScore += questionObtainedScore;

                        //result.Questions.Add(new QuestionResultDto
                        //{
                        //    QuestionOid = questionSubmission.QuestionOid,
                        //    QuestionText = question.QuestionText,
                        //    SelectedAnswerOids = questionSubmission.SelectedAnswerOids ?? new List<Guid>(),
                        //    IsCorrect = isCorrect,
                        //    QuestionScore = question.QuestionScore,
                        //    ObtainedScore = questionObtainedScore
                        //});
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Error processing question {questionSubmission.QuestionOid}: {ex.Message}");
                    }
                }

                //result.CorrectAnswers = correctAnswers;
                //result.TotalScore = totalScore;
                //result.ObtainedScore = obtainedScore;
                //result.Message = $"Submitted {result.Questions.Count} of {dto.Questions.Count} questions successfully. " +
                //                $"Score: {obtainedScore}/{totalScore} ({correctAnswers}/{dto.Questions.Count} correct)";
                result.Message = "submited";
                if (result.Errors.Any())
                {
                    result.Success = false;
                }

                return ApiResponse<MultipleQuestionsSubmissionResult>.SuccessResponse(result, result.Message);
            }
            catch (Exception ex)
            {
                return ApiResponse<MultipleQuestionsSubmissionResult>.ErrorResponse($"Error submitting questions: {ex.Message}");
            }
        }
    }
}
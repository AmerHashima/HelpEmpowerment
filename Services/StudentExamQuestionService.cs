using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;

namespace HelpEmpowermentApi.Services
{
    public class StudentExamQuestionService : IStudentExamQuestionService
    {
        // Question Status Lookup IDs
        private static readonly Guid QuestionStatusCorrect = Guid.Parse("44444444-4444-4444-4444-444444444401");
        private static readonly Guid QuestionStatusIncorrect = Guid.Parse("44444444-4444-4444-4444-444444444402");
        private static readonly Guid QuestionStatusNotAnswered = Guid.Parse("44444444-4444-4444-4444-444444444403");

        private readonly IStudentExamQuestionRepository _studentExamQuestionRepository;
        private readonly IStudentExamQuestionAnswerRepository _studentExamQuestionAnswerRepository;
        private readonly IStudentExamRepository _studentExamRepository;
        private readonly ICourseQuestionRepository _courseQuestionRepository;
        private readonly ICourseAnswerRepository _courseAnswerRepository;

        public StudentExamQuestionService(
            IStudentExamQuestionRepository studentExamQuestionRepository,
            IStudentExamQuestionAnswerRepository studentExamQuestionAnswerRepository,
            IStudentExamRepository studentExamRepository,
            ICourseQuestionRepository courseQuestionRepository,
            ICourseAnswerRepository courseAnswerRepository)
        {
            _studentExamQuestionRepository = studentExamQuestionRepository;
            _studentExamQuestionAnswerRepository = studentExamQuestionAnswerRepository;
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

                // Find or create the question row
                var existingQuestions = await _studentExamQuestionRepository.GetByStudentExamIdAsync(dto.StudentExamOid);
                var examQuestion = existingQuestions.FirstOrDefault(eq => eq.QuestionOid == dto.QuestionOid);

                if (examQuestion != null)
                {
                    // Update existing question row
                    examQuestion.IsCorrect = isCorrect;
                    examQuestion.QuestionStatusLookupId = isCorrect ? QuestionStatusCorrect : QuestionStatusIncorrect;
                    examQuestion.QuestionScore = question.QuestionScore;
                    examQuestion.ObtainedScore = obtainedScore;
                    examQuestion.UpdatedBy = dto.CreatedBy;
                    examQuestion.UpdatedAt = DateTime.UtcNow;
                    await _studentExamQuestionRepository.UpdateAsync(examQuestion);

                    // Update answers
                    await UpdateAnswersForQuestion(examQuestion.Oid, dto.SelectedAnswerOids, dto.CreatedBy);
                }
                else
                {
                    // Create new question row
                    examQuestion = new StudentExamQuestion
                    {
                        StudentExamOid = dto.StudentExamOid,
                        QuestionOid = dto.QuestionOid,
                        IsCorrect = isCorrect,
                        QuestionStatusLookupId = isCorrect ? QuestionStatusCorrect : QuestionStatusIncorrect,
                        QuestionScore = question.QuestionScore,
                        ObtainedScore = obtainedScore,
                        CreatedBy = dto.CreatedBy,
                        CreatedAt = DateTime.UtcNow
                    };
                    examQuestion = await _studentExamQuestionRepository.AddAsync(examQuestion);

                    // Create answer rows
                    foreach (var answerId in dto.SelectedAnswerOids)
                    {
                        var answerRow = new StudentExamQuestionAnswer
                        {
                            StudentExamQuestionOid = examQuestion.Oid,
                            SelectedAnswerOid = answerId,
                            CreatedBy = dto.CreatedBy,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _studentExamQuestionAnswerRepository.AddAsync(answerRow);
                    }
                }

                // Recalculate total score for the student exam
                await CalcAndUpdateStudentExamScoreAsync(dto.StudentExamOid);

                return ApiResponse<StudentExamQuestionDto>.SuccessResponse(
                    MapToDto(examQuestion),
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

                if (dto.SelectedAnswerOids != null && dto.SelectedAnswerOids.Any())
                {
                    var allAnswers = await _courseAnswerRepository.GetByQuestionIdAsync(examQuestion.QuestionOid);

                    var invalidAnswers = dto.SelectedAnswerOids.Where(id => !allAnswers.Any(a => a.Oid == id)).ToList();
                    if (invalidAnswers.Any())
                        return ApiResponse<StudentExamQuestionDto>.ErrorResponse("Invalid Answer(s). Please select valid answers.");

                    var correctAnswerOids = allAnswers.Where(a => a.IsCorrect).Select(a => a.Oid).ToList();
                    var selectedSet = dto.SelectedAnswerOids.OrderBy(x => x).ToList();
                    var correctSet = correctAnswerOids.OrderBy(x => x).ToList();
                    var isCorrect = selectedSet.SequenceEqual(correctSet);
                    var obtainedScore = isCorrect ? examQuestion.QuestionScore ?? 0 : 0;

                    examQuestion.IsCorrect = isCorrect;
                    examQuestion.QuestionStatusLookupId = isCorrect ? QuestionStatusCorrect : QuestionStatusIncorrect;
                    examQuestion.ObtainedScore = obtainedScore;
                    examQuestion.UpdatedBy = dto.UpdatedBy;
                    examQuestion.UpdatedAt = DateTime.UtcNow;
                    await _studentExamQuestionRepository.UpdateAsync(examQuestion);

                    // Update answers
                    await UpdateAnswersForQuestion(examQuestion.Oid, dto.SelectedAnswerOids, dto.UpdatedBy);

                    // Recalculate total score for the student exam
                    await CalcAndUpdateStudentExamScoreAsync(examQuestion.StudentExamOid);

                    return ApiResponse<StudentExamQuestionDto>.SuccessResponse(
                        MapToDto(examQuestion),
                        "Student exam question updated successfully");
                }
                else if (dto.IsCorrect.HasValue || dto.ObtainedScore.HasValue || dto.QuestionScore.HasValue)
                {
                    examQuestion.IsCorrect = dto.IsCorrect ?? examQuestion.IsCorrect;
                    examQuestion.QuestionScore = dto.QuestionScore ?? examQuestion.QuestionScore;
                    examQuestion.ObtainedScore = dto.ObtainedScore ?? examQuestion.ObtainedScore;
                    examQuestion.UpdatedBy = dto.UpdatedBy;
                    examQuestion.UpdatedAt = DateTime.UtcNow;

                    var updatedExamQuestion = await _studentExamQuestionRepository.UpdateAsync(examQuestion);

                    // Recalculate total score for the student exam
                    await CalcAndUpdateStudentExamScoreAsync(examQuestion.StudentExamOid);

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
                QuestionText_Ar = examQuestion.Question?.QuestionText_Ar ?? string.Empty,
                QuestionExplination = examQuestion.Question?.QuestionExplination ?? string.Empty,
                ExamName = examQuestion.Question?.MasterExam?.CourseName,
                QuestionTypeLookupId = examQuestion.Question?.QuestionTypeLookupId,
                QuestionTypeName = examQuestion.Question?.QuestionTypeLookup?.LookupNameEn,
                OrderNo = examQuestion.Question?.OrderNo,
                IsActive = examQuestion.Question?.IsActive ?? false,
                CorrectAnswer = examQuestion.Question?.CorrectAnswer ?? false,
                Question = examQuestion.Question?.Question ?? false,
                CorrectChoiceOid = examQuestion.Question?.CorrectChoiceOid,
                IsCorrect = examQuestion.IsCorrect,
                QuestionStatusLookupId = examQuestion.QuestionStatusLookupId,
                QuestionStatusName = examQuestion.QuestionStatus?.LookupNameEn,
                QuestionScore = examQuestion.QuestionScore,
                ObtainedScore = examQuestion.ObtainedScore,
                Answers = examQuestion.Answers?
                    .Where(a => !a.IsDeleted)
                    .Select(a => new StudentExamQuestionAnswerDto
                    {
                        Oid = a.Oid,
                        SelectedAnswerOid = a.SelectedAnswerOid,
                        SelectedAnswerText = a.SelectedAnswer?.AnswerText,
                        AnswerSelectedAnswerOid = a.AnswerSelectedAnswerOid
                    }).ToList() ?? new List<StudentExamQuestionAnswerDto>(),
                CreatedAt = examQuestion.CreatedAt,
                CreatedBy = examQuestion.CreatedBy,
                UpdatedAt = examQuestion.UpdatedAt,
                UpdatedBy = examQuestion.UpdatedBy
            };
        }

        private async Task UpdateAnswersForQuestion(Guid studentExamQuestionOid, List<Guid> selectedAnswerOids, Guid? userId)
        {
            var existingAnswers = await _studentExamQuestionAnswerRepository.GetByStudentExamQuestionIdAsync(studentExamQuestionOid);
            var processedIds = new HashSet<Guid>();

            foreach (var answerId in selectedAnswerOids)
            {
                var existing = existingAnswers.FirstOrDefault(a => a.SelectedAnswerOid == answerId);
                if (existing == null)
                    existing = existingAnswers.FirstOrDefault(a => !processedIds.Contains(a.Oid));

                if (existing != null)
                {
                    processedIds.Add(existing.Oid);
                    existing.SelectedAnswerOid = answerId;
                    existing.UpdatedBy = userId;
                    existing.UpdatedAt = DateTime.UtcNow;
                    await _studentExamQuestionAnswerRepository.UpdateAsync(existing);
                }
                else
                {
                    var answerRow = new StudentExamQuestionAnswer
                    {
                        StudentExamQuestionOid = studentExamQuestionOid,
                        SelectedAnswerOid = answerId,
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _studentExamQuestionAnswerRepository.AddAsync(answerRow);
                }
            }
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

                        // Find or create question row
                        var existingQuestion = existingAnswers.FirstOrDefault(eq => eq.QuestionOid == questionSubmission.QuestionOid);

                        if (existingQuestion != null)
                        {
                            existingQuestion.IsCorrect = isCorrect;
                            existingQuestion.QuestionStatusLookupId = (questionSubmission.SelectedAnswerOids != null && questionSubmission.SelectedAnswerOids.Any())
                                ? (isCorrect ? QuestionStatusCorrect : QuestionStatusIncorrect)
                                : QuestionStatusNotAnswered;
                            existingQuestion.QuestionScore = question.QuestionScore;
                            existingQuestion.ObtainedScore = questionObtainedScore;
                            existingQuestion.UpdatedBy = dto.CreatedBy;
                            existingQuestion.UpdatedAt = DateTime.UtcNow;
                            await _studentExamQuestionRepository.UpdateAsync(existingQuestion);

                            if (questionSubmission.SelectedAnswerOids != null && questionSubmission.SelectedAnswerOids.Any())
                            {
                                await UpdateAnswersForQuestion(existingQuestion.Oid, questionSubmission.SelectedAnswerOids, dto.CreatedBy);
                            }
                        }
                        else
                        {
                            var examQuestion = new StudentExamQuestion
                            {
                                StudentExamOid = dto.StudentExamOid,
                                QuestionOid = questionSubmission.QuestionOid,
                                IsCorrect = isCorrect,
                                QuestionStatusLookupId = (questionSubmission.SelectedAnswerOids != null && questionSubmission.SelectedAnswerOids.Any())
                                    ? (isCorrect ? QuestionStatusCorrect : QuestionStatusIncorrect)
                                    : QuestionStatusNotAnswered,
                                QuestionScore = question.QuestionScore,
                                ObtainedScore = questionObtainedScore,
                                CreatedBy = dto.CreatedBy,
                                CreatedAt = DateTime.UtcNow
                            };
                            examQuestion = await _studentExamQuestionRepository.AddAsync(examQuestion);

                            if (questionSubmission.SelectedAnswerOids != null && questionSubmission.SelectedAnswerOids.Any())
                            {
                                foreach (var answerId in questionSubmission.SelectedAnswerOids)
                                {
                                    var answerRow = new StudentExamQuestionAnswer
                                    {
                                        StudentExamQuestionOid = examQuestion.Oid,
                                        SelectedAnswerOid = answerId,
                                        CreatedBy = dto.CreatedBy,
                                        CreatedAt = DateTime.UtcNow
                                    };
                                    await _studentExamQuestionAnswerRepository.AddAsync(answerRow);
                                }
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

                // Recalculate total score for the student exam
                await CalcAndUpdateStudentExamScoreAsync(dto.StudentExamOid);

                result.Message = "submitted";
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

        public async Task<ApiResponse<AnswerValidationResult>> ValidateAnswersAsync(ValidateAnswersDto dto)
        {
            try
            {
                // Validate Student Exam exists
                var examExists = await _studentExamRepository.ExistsAsync(e => e.Oid == dto.StudentExamOid && !e.IsDeleted);
                if (!examExists)
                    return ApiResponse<AnswerValidationResult>.ErrorResponse("Invalid Student Exam. Please select a valid exam.");

                // Validate Question exists
                var question = await _courseQuestionRepository.GetByIdAsync(dto.QuestionOid);
                if (question == null)
                    return ApiResponse<AnswerValidationResult>.ErrorResponse("Question not found");

                // Get all answers for the question
                var allAnswers = await _courseAnswerRepository.GetByQuestionIdAsync(dto.QuestionOid);
                if (!allAnswers.Any())
                    return ApiResponse<AnswerValidationResult>.ErrorResponse("No answers found for this question");

                // Validate all submitted answers exist
                var submittedAnswerOids = dto.Answers.Select(a => a.SelectedAnswerOid).ToList();
                var invalidAnswers = submittedAnswerOids.Where(id => !allAnswers.Any(a => a.Oid == id)).ToList();
                if (invalidAnswers.Any())
                    return ApiResponse<AnswerValidationResult>.ErrorResponse("Invalid answer(s) selected");

                // Validate each answer submission and determine correctness
                bool isCorrect = true; // Start with true, will set to false if any validation fails
                var validationErrors = new List<string>();

                foreach (var answerSubmission in dto.Answers)
                {
                    // Find the answer in the database
                    var dbAnswer = allAnswers.FirstOrDefault(a => a.Oid == answerSubmission.SelectedAnswerOid);
                    if (dbAnswer == null)
                    {
                        isCorrect = false;
                        validationErrors.Add($"Answer {answerSubmission.SelectedAnswerOid} not found");
                        continue;
                    }

                    // Check if the AnswerSelectedAnswerOid matches the database CorrectAnswerOid
                    // Answer is correct when: SelectedAnswerOid = answerOid AND AnswerSelectedAnswerOid = CorrectAnswerOid
                    if (answerSubmission.AnswerSelectedAnswerOid.HasValue)
                    {
                        if (dbAnswer.CorrectAnswerOid != answerSubmission.AnswerSelectedAnswerOid.Value)
                        {
                            isCorrect = false;
                            validationErrors.Add($"Answer {answerSubmission.SelectedAnswerOid}: CorrectAnswerOid mismatch. Expected {dbAnswer.CorrectAnswerOid}, got {answerSubmission.AnswerSelectedAnswerOid}");
                        }
                    }
                    else
                    {
                        // If no AnswerSelectedAnswerOid provided but the answer has one, it's invalid
                        if (dbAnswer.CorrectAnswerOid.HasValue)
                        {
                            isCorrect = false;
                            validationErrors.Add($"Answer {answerSubmission.SelectedAnswerOid}: Missing AnswerSelectedAnswerOid. Expected {dbAnswer.CorrectAnswerOid}");
                        }
                        // If both are null, it's valid for this answer
                    }
                }

                // Get correct answers using IsCorrect flag (for reference in response)
                var correctAnswerOids = allAnswers.Where(a => a.IsCorrect).Select(a => a.Oid).ToList();

                // Calculate score
                int obtainedScore = isCorrect ? question.QuestionScore : 0;

                // Find or create the question row
                var existingQuestions = await _studentExamQuestionRepository.GetByStudentExamIdAsync(dto.StudentExamOid);
                var examQuestion = existingQuestions.FirstOrDefault(eq => eq.QuestionOid == dto.QuestionOid);

                if (examQuestion != null)
                {
                    examQuestion.IsCorrect = isCorrect;
                    examQuestion.QuestionStatusLookupId = isCorrect ? QuestionStatusCorrect : QuestionStatusIncorrect;
                    examQuestion.QuestionScore = question.QuestionScore;
                    examQuestion.ObtainedScore = obtainedScore;
                    examQuestion.UpdatedBy = dto.CreatedBy;
                    examQuestion.UpdatedAt = DateTime.UtcNow;
                    await _studentExamQuestionRepository.UpdateAsync(examQuestion);
                }
                else
                {
                    examQuestion = new StudentExamQuestion
                    {
                        StudentExamOid = dto.StudentExamOid,
                        QuestionOid = dto.QuestionOid,
                        IsCorrect = isCorrect,
                        QuestionStatusLookupId = isCorrect ? QuestionStatusCorrect : QuestionStatusIncorrect,
                        QuestionScore = question.QuestionScore,
                        ObtainedScore = obtainedScore,
                        CreatedBy = dto.CreatedBy,
                        CreatedAt = DateTime.UtcNow
                    };
                    examQuestion = await _studentExamQuestionRepository.AddAsync(examQuestion);
                }

                // Update or create answer rows
                var existingAnswerRows = await _studentExamQuestionAnswerRepository.GetByStudentExamQuestionIdAsync(examQuestion.Oid);
                var processedAnswerIds = new HashSet<Guid>();

                foreach (var answerId in submittedAnswerOids)
                {
                    var answerSubmission = dto.Answers.FirstOrDefault(a => a.SelectedAnswerOid == answerId);

                    var existingAnswer = existingAnswerRows.FirstOrDefault(a => a.SelectedAnswerOid == answerId);
                    if (existingAnswer == null)
                        existingAnswer = existingAnswerRows.FirstOrDefault(a => !processedAnswerIds.Contains(a.Oid));

                    if (existingAnswer != null)
                    {
                        processedAnswerIds.Add(existingAnswer.Oid);
                        existingAnswer.SelectedAnswerOid = answerId;
                        existingAnswer.AnswerSelectedAnswerOid = answerSubmission?.AnswerSelectedAnswerOid;
                        existingAnswer.UpdatedBy = dto.CreatedBy;
                        existingAnswer.UpdatedAt = DateTime.UtcNow;
                        await _studentExamQuestionAnswerRepository.UpdateAsync(existingAnswer);
                    }
                    else
                    {
                        var answerRow = new StudentExamQuestionAnswer
                        {
                            StudentExamQuestionOid = examQuestion.Oid,
                            SelectedAnswerOid = answerId,
                            AnswerSelectedAnswerOid = answerSubmission?.AnswerSelectedAnswerOid,
                            CreatedBy = dto.CreatedBy,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _studentExamQuestionAnswerRepository.AddAsync(answerRow);
                    }
                }

                // Recalculate total score for the student exam
                await CalcAndUpdateStudentExamScoreAsync(dto.StudentExamOid);

                // Build answer details
                var answerDetails = allAnswers.Select(a => new AnswerValidationDetail
                {
                    AnswerOid = a.Oid,
                    AnswerText = a.AnswerText,
                    IsSelected = submittedAnswerOids.Contains(a.Oid),
                    IsCorrectAnswer = a.IsCorrect,
                    CorrectAnswerOid = a.CorrectAnswerOid
                }).ToList();

                var message = isCorrect 
                    ? "Correct answer!" 
                    : validationErrors.Any() 
                        ? $"Incorrect answer: {string.Join("; ", validationErrors)}" 
                        : "Incorrect answer";

                var result = new AnswerValidationResult
                {
                    Success = isCorrect,
                    Message = message,
                    QuestionOid = dto.QuestionOid,
                    QuestionText = question.QuestionText,
                    SelectedAnswerOids = submittedAnswerOids,
                    CorrectAnswerOids = correctAnswerOids,
                    IsCorrect = isCorrect,
                    QuestionScore = question.QuestionScore,
                    ObtainedScore = obtainedScore,
                    AnswerDetails = answerDetails
                };

                return ApiResponse<AnswerValidationResult>.SuccessResponse(result, message);
            }
            catch (Exception ex)
            {
                return ApiResponse<AnswerValidationResult>.ErrorResponse($"Error validating answers: {ex.Message}");
            }
        }
                    private async Task CalcAndUpdateStudentExamScoreAsync(Guid studentExamOid)
                    {
                        var allQuestions = await _studentExamQuestionRepository.GetByStudentExamIdAsync(studentExamOid);

                        int totalScore = allQuestions.Sum(q => q.QuestionScore ?? 0);
                        int obtainedScore = allQuestions.Sum(q => q.ObtainedScore ?? 0);

                        // Use the already-tracked StudentExam from navigation to avoid tracking conflict
                        var studentExam = allQuestions.FirstOrDefault()?.StudentExam
                                          ?? await _studentExamRepository.GetByIdAsync(studentExamOid);

                        if (studentExam != null)
                        {
                            studentExam.TotalScore = totalScore;
                            studentExam.ObtainedScore = obtainedScore;

                            if (studentExam.PassPercent.HasValue && totalScore > 0)
                            {
                                decimal percentage = (decimal)obtainedScore / totalScore * 100;
                                studentExam.IsPassed = percentage >= studentExam.PassPercent.Value;
                            }

                            studentExam.UpdatedAt = DateTime.UtcNow;
                            await _studentExamRepository.UpdateAsync(studentExam);
                        }
                    }
                }
            }
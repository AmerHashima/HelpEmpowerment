using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IRepositories;
using HelpEmpowermentApi.IServices;
using HelpEmpowermentApi.Models;
using HelpEmpowermentApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HelpEmpowermentApi.Services
{
    public class CourseQuestionService : ICourseQuestionService
    {
        private static readonly Guid MultiImageQuestionTypeId = Guid.Parse("33333333-3333-3333-3333-333333333307");
        private readonly ICourseQuestionRepository _questionRepository;
        private readonly ICourseAnswerRepository _answerRepository;
        private readonly IAppLookupDetailRepository _lookupDetailRepository;
        private readonly ICoursesMasterExamRepository _examRepository;
        private readonly IRepository<CourseQuestionImage> _questionImageRepository;
        private readonly IRepository<CourseQuestionExplanationImage> _explanationImageRepository;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;

        public CourseQuestionService(
            ICourseQuestionRepository questionRepository, 
            ICourseAnswerRepository answerRepository,
            IAppLookupDetailRepository lookupDetailRepository,
            ICoursesMasterExamRepository examRepository,
            IRepository<CourseQuestionImage> questionImageRepository,
            IRepository<CourseQuestionExplanationImage> explanationImageRepository,
            ApplicationDbContext db,
            IConfiguration configuration)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _lookupDetailRepository = lookupDetailRepository;
            _examRepository = examRepository;
            _questionImageRepository = questionImageRepository;
            _explanationImageRepository = explanationImageRepository;
            _db = db;
            _configuration = configuration;
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
                var exam = await _examRepository.GetByIdAsync(dto.CoursesMasterExamOid);
                if (exam == null)
                    return ApiResponse<CourseQuestionDto>.ErrorResponse("Exam not found");

                if (dto.QuestionTypeLookupId.HasValue)
                {
                    var questionTypeExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.QuestionTypeLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!questionTypeExists)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("Invalid Question Type. Please select a valid question type.");
                }

                if (dto.CorrectChoiceOid.HasValue)
                {
                    var correctChoiceExists = await _questionRepository.ExistsAsync(
                        q => q.Oid == dto.CorrectChoiceOid.Value && !q.IsDeleted);
                    if (!correctChoiceExists)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("Invalid Correct Choice. The referenced question does not exist.");
                }

                if (dto.QuestionTypeLookupId == MultiImageQuestionTypeId)
                    return await CreateMultiQuestionAsync(dto);

                var question = new CourseQuestion
                {
                    CoursesMasterExamOid = dto.CoursesMasterExamOid,
                    QuestionText = dto.QuestionText,
                    QuestionText_Ar = dto.QuestionText_Ar,
                    QuestionTypeLookupId = dto.QuestionTypeLookupId,
                    QuestionScore = dto.QuestionScore,
                    OrderNo = dto.OrderNo,
                    IsActive = dto.IsActive,
                    QuestionExplination = dto.QuestionExplination,
                    CorrectAnswer = dto.CorrectAnswer,
                    Question = dto.Question,
                    CorrectChoiceOid = dto.CorrectChoiceOid,
                    CreatedBy = dto.CreatedBy
                };

                var created = await _questionRepository.AddAsync(question);

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
                            AnswerText_Ar = answerDto.AnswerText_Ar,
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

                if (dto.QuestionTypeLookupId.HasValue)
                {
                    var questionTypeExists = await _lookupDetailRepository.ExistsAsync(
                        d => d.Oid == dto.QuestionTypeLookupId.Value && !d.IsDeleted && d.IsActive);
                    if (!questionTypeExists)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("Invalid Question Type. Please select a valid question type.");
                }

                if (dto.CorrectChoiceOid.HasValue)
                {
                    if (dto.CorrectChoiceOid.Value == dto.Oid)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("A question cannot reference itself as the correct choice.");

                    var correctChoiceExists = await _questionRepository.ExistsAsync(
                        q => q.Oid == dto.CorrectChoiceOid.Value && !q.IsDeleted);
                    if (!correctChoiceExists)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("Invalid Correct Choice. The referenced question does not exist.");
                }

                if (dto.QuestionTypeLookupId == MultiImageQuestionTypeId)
                    return await UpdateMultiQuestionAsync(question, dto);

                // Update question properties
                question.CoursesMasterExamOid = dto.CoursesMasterExamOid;
                question.QuestionText = dto.QuestionText;
                question.QuestionText_Ar = dto.QuestionText_Ar;
                question.QuestionTypeLookupId = dto.QuestionTypeLookupId;
                question.QuestionScore = dto.QuestionScore;
                question.OrderNo = dto.OrderNo;
                question.IsActive = dto.IsActive;
                question.QuestionExplination = dto.QuestionExplination;
                question.CorrectAnswer = dto.CorrectAnswer;
                question.Question = dto.Question;
                question.CorrectChoiceOid = dto.CorrectChoiceOid;
                question.UpdatedBy = dto.UpdatedBy;
                question.UpdatedAt = DateTime.UtcNow;

                await _questionRepository.UpdateAsync(question);

                // ✅ UPDATE ANSWERS if provided
                if (dto.Answers != null && dto.Answers.Any())
                {
                    // Process deletes
                    var existingAnswerIds = (await _answerRepository.GetByQuestionIdAsync(dto.Oid))
                        .Select(a => a.Oid).ToHashSet();
                    var dtoAnswerIds = dto.Answers.Where(a => a.Oid != Guid.Empty).Select(a => a.Oid).ToHashSet();
                    var answersToDelete = existingAnswerIds.Except(dtoAnswerIds);

                    foreach (var answerIdToDelete in answersToDelete)
                    {
                        await _answerRepository.SoftDeleteAsync(answerIdToDelete);
                    }

                    // Process creates and updates
                    foreach (var answerDto in dto.Answers)
                    {
                        if (answerDto.Oid == Guid.Empty)
                        {
                            // Create new answer
                            var newAnswer = new CourseAnswer
                            {
                                QuestionId = dto.Oid,
                                AnswerText = answerDto.AnswerText,
                                AnswerText_Ar = answerDto.AnswerText_Ar,
                                Question_Ask = answerDto.Question_Ask,
                                CorrectAnswerOid = answerDto.CorrectAnswerOid,
                                IsCorrect = answerDto.IsCorrect,
                                OrderNo = answerDto.OrderNo,
                                CreatedBy = dto.UpdatedBy,
                                CreatedAt = DateTime.UtcNow
                            };
                            await _answerRepository.AddAsync(newAnswer);
                        }
                        else
                        {
                            // ✅ FIX: Fetch individual entity to avoid tracking conflicts
                            var existingAnswer = await _answerRepository.GetByIdAsync(answerDto.Oid);
                            if (existingAnswer != null)
                            {
                                existingAnswer.AnswerText = answerDto.AnswerText;
                                existingAnswer.AnswerText_Ar = answerDto.AnswerText_Ar;
                                existingAnswer.Question_Ask = answerDto.Question_Ask;
                                existingAnswer.CorrectAnswerOid = answerDto.CorrectAnswerOid;
                                existingAnswer.IsCorrect = answerDto.IsCorrect;
                                existingAnswer.OrderNo = answerDto.OrderNo;
                                existingAnswer.UpdatedBy = dto.UpdatedBy;
                                existingAnswer.UpdatedAt = DateTime.UtcNow;
                                
                                await _answerRepository.UpdateAsync(existingAnswer);
                            }
                        }
                    }
                }

                // Reload question with updated answers
                var result = await _questionRepository.GetWithAnswersAsync(dto.Oid);
                return ApiResponse<CourseQuestionDto>.SuccessResponse(MapToDto(result!), "Question and answers updated successfully");
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

        private static readonly HashSet<string> _allowedImageExtensions = new() { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private string ImageStoragePath => _configuration["FileStorage:QuestionImagesPath"] ?? "/var/www/images/questions";

        public async Task<ApiResponse<CourseQuestionDto>> UploadImageAsync(Guid id, IFormFile image)
        {
            // Preserve the original single-image endpoint's replace behavior.
            await DeleteImageAsync(id);
            return await UploadImagesAsync(id, new[] { image });
        }

        public async Task<ApiResponse<CourseQuestionDto>> UploadImagesAsync(Guid id, IReadOnlyCollection<IFormFile> images)
        {
            try
            {
                var question = await _questionRepository.GetWithAnswersAsync(id);
                if (question == null)
                    return ApiResponse<CourseQuestionDto>.ErrorResponse("Question not found");

                if (images.Count == 0 || images.Any(image => image == null || image.Length == 0))
                    return ApiResponse<CourseQuestionDto>.ErrorResponse("At least one non-empty image is required");

                var invalidImage = images.FirstOrDefault(image =>
                    !_allowedImageExtensions.Contains(Path.GetExtension(image.FileName).ToLowerInvariant()));
                if (invalidImage != null)
                    return ApiResponse<CourseQuestionDto>.ErrorResponse($"Invalid file type for '{invalidImage.FileName}'. Allowed: {string.Join(", ", _allowedImageExtensions)}");

                var basePath = ImageStoragePath;
                Directory.CreateDirectory(basePath);

                var existingImages = (await _questionImageRepository.FindAsync(image => image.CourseQuestionOid == id))
                    .OrderBy(image => image.OrderNo)
                    .ToList();
                var nextOrder = existingImages.Count == 0 ? 1 : existingImages.Max(image => image.OrderNo) + 1;

                foreach (var image in images)
                {
                    var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
                    var imageId = Guid.NewGuid();
                    var fileName = $"{id}_{imageId}{ext}";
                    var filePath = Path.Combine(basePath, fileName);

                    await using (var stream = new FileStream(filePath, FileMode.CreateNew))
                        await image.CopyToAsync(stream);

                    await _questionImageRepository.AddAsync(new CourseQuestionImage
                    {
                        Oid = imageId,
                        CourseQuestionOid = id,
                        FileName = fileName,
                        OrderNo = nextOrder++
                    });
                }

                // Keep the old single-image field populated for older clients.
                if (string.IsNullOrWhiteSpace(question.QuestionImage))
                {
                    question.QuestionImage = (await _questionImageRepository.FindAsync(image => image.CourseQuestionOid == id))
                        .OrderBy(image => image.OrderNo)
                        .Select(image => image.FileName)
                        .FirstOrDefault() ?? string.Empty;
                    await _questionRepository.UpdateAsync(question);
                }

                var result = await _questionRepository.GetWithAnswersAsync(id);
                return ApiResponse<CourseQuestionDto>.SuccessResponse(MapToDto(result!), "Images uploaded successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseQuestionDto>.ErrorResponse($"Error uploading image: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> GetImagePathAsync(Guid id)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    return ApiResponse<string>.ErrorResponse("Question not found");

                var firstImage = (await _questionImageRepository.FindAsync(image => image.CourseQuestionOid == id))
                    .OrderBy(image => image.OrderNo)
                    .FirstOrDefault();
                var fileName = firstImage?.FileName ?? question.QuestionImage;

                if (string.IsNullOrEmpty(fileName))
                    return ApiResponse<string>.ErrorResponse("No image uploaded for this question");

                return ApiResponse<string>.SuccessResponse(fileName);
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> GetImagePathAsync(Guid id, Guid imageId)
        {
            var image = await _questionImageRepository.GetByIdAsync(imageId);
            if (image == null || image.CourseQuestionOid != id)
                return ApiResponse<string>.ErrorResponse("Image not found");

            return ApiResponse<string>.SuccessResponse(image.FileName);
        }

        public async Task<ApiResponse<bool>> DeleteImageAsync(Guid id)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    return ApiResponse<bool>.ErrorResponse("Question not found");

                var images = (await _questionImageRepository.FindAsync(image => image.CourseQuestionOid == id))
                    .OrderBy(image => image.OrderNo)
                    .ToList();

                if (images.Count == 0 && string.IsNullOrEmpty(question.QuestionImage))
                    return ApiResponse<bool>.ErrorResponse("No image to delete for this question");

                foreach (var image in images)
                {
                    DeletePhysicalImage(image.FileName);
                    await _questionImageRepository.SoftDeleteAsync(image.Oid);
                }

                if (images.Count == 0)
                    DeletePhysicalImage(question.QuestionImage);

                question.QuestionImage = string.Empty;
                question.UpdatedAt = DateTime.UtcNow;
                await _questionRepository.UpdateAsync(question);

                return ApiResponse<bool>.SuccessResponse(true, "Image deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting image: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteImageAsync(Guid id, Guid imageId)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(id);
                if (question == null)
                    return ApiResponse<bool>.ErrorResponse("Question not found");

                var image = await _questionImageRepository.GetByIdAsync(imageId);
                if (image == null || image.CourseQuestionOid != id)
                    return ApiResponse<bool>.ErrorResponse("Image not found");

                DeletePhysicalImage(image.FileName);
                await _questionImageRepository.SoftDeleteAsync(imageId);

                var nextImage = (await _questionImageRepository.FindAsync(item => item.CourseQuestionOid == id))
                    .OrderBy(item => item.OrderNo)
                    .FirstOrDefault();
                question.QuestionImage = nextImage?.FileName ?? string.Empty;
                await _questionRepository.UpdateAsync(question);

                return ApiResponse<bool>.SuccessResponse(true, "Image deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting image: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CourseQuestionDto>> UploadExplanationImagesAsync(
            Guid id, IReadOnlyCollection<IFormFile> images)
        {
            try
            {
                var question = await _questionRepository.GetWithAnswersAsync(id);
                if (question == null)
                    return ApiResponse<CourseQuestionDto>.ErrorResponse("Question not found");

                if (images.Count == 0 || images.Any(image => image == null || image.Length == 0))
                    return ApiResponse<CourseQuestionDto>.ErrorResponse("At least one non-empty image is required");

                var invalidImage = images.FirstOrDefault(image =>
                    !_allowedImageExtensions.Contains(Path.GetExtension(image.FileName).ToLowerInvariant()));
                if (invalidImage != null)
                    return ApiResponse<CourseQuestionDto>.ErrorResponse(
                        $"Invalid file type for '{invalidImage.FileName}'. Allowed: {string.Join(", ", _allowedImageExtensions)}");

                Directory.CreateDirectory(ImageStoragePath);
                var existingImages = (await _explanationImageRepository.FindAsync(
                        image => image.CourseQuestionOid == id))
                    .OrderBy(image => image.OrderNo)
                    .ToList();
                var nextOrder = existingImages.Count == 0 ? 1 : existingImages.Max(image => image.OrderNo) + 1;

                foreach (var image in images)
                {
                    var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
                    var imageId = Guid.NewGuid();
                    var fileName = $"{id}_explanation_{imageId}{ext}";
                    var filePath = Path.Combine(ImageStoragePath, fileName);

                    await using (var stream = new FileStream(filePath, FileMode.CreateNew))
                        await image.CopyToAsync(stream);

                    await _explanationImageRepository.AddAsync(new CourseQuestionExplanationImage
                    {
                        Oid = imageId,
                        CourseQuestionOid = id,
                        FileName = fileName,
                        OrderNo = nextOrder++
                    });
                }

                var result = await _questionRepository.GetWithAnswersAsync(id);
                return ApiResponse<CourseQuestionDto>.SuccessResponse(
                    MapToDto(result!), "Explanation images uploaded successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseQuestionDto>.ErrorResponse($"Error uploading explanation image: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> GetExplanationImagePathAsync(Guid id, Guid imageId)
        {
            var image = await _explanationImageRepository.GetByIdAsync(imageId);
            if (image == null || image.CourseQuestionOid != id)
                return ApiResponse<string>.ErrorResponse("Explanation image not found");

            return ApiResponse<string>.SuccessResponse(image.FileName);
        }

        public async Task<ApiResponse<bool>> DeleteExplanationImageAsync(Guid id, Guid imageId)
        {
            try
            {
                if (await _questionRepository.GetByIdAsync(id) == null)
                    return ApiResponse<bool>.ErrorResponse("Question not found");

                var image = await _explanationImageRepository.GetByIdAsync(imageId);
                if (image == null || image.CourseQuestionOid != id)
                    return ApiResponse<bool>.ErrorResponse("Explanation image not found");

                DeletePhysicalImage(image.FileName);
                await _explanationImageRepository.SoftDeleteAsync(imageId);
                return ApiResponse<bool>.SuccessResponse(true, "Explanation image deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting explanation image: {ex.Message}");
            }
        }

        private static string? ValidateSubQuestions(IReadOnlyCollection<UpsertCourseQuestionSubQuestionDto>? subQuestions)
        {
            if (subQuestions == null || subQuestions.Count == 0)
                return "Multiple Sub-Questions requires at least one sub-question.";

            foreach (var subQuestion in subQuestions)
            {
                if (string.IsNullOrWhiteSpace(subQuestion.QuestionText) &&
                    string.IsNullOrWhiteSpace(subQuestion.QuestionTextAr))
                    return "Every sub-question requires English or Arabic text.";
                if (subQuestion.Choices == null || subQuestion.Choices.Count < 2)
                    return "Every sub-question requires at least two choices.";
                if (subQuestion.Choices.Count(choice => choice.IsCorrect) != 1)
                    return "Every sub-question must have exactly one correct choice.";
                if (subQuestion.Choices.Any(choice =>
                    string.IsNullOrWhiteSpace(choice.ChoiceText) && string.IsNullOrWhiteSpace(choice.ChoiceTextAr)))
                    return "Every choice requires English or Arabic text.";
            }

            return null;
        }

        private async Task<ApiResponse<CourseQuestionDto>> CreateMultiQuestionAsync(CreateCourseQuestionDto dto)
        {
            var validationError = ValidateSubQuestions(dto.SubQuestions);
            if (validationError != null)
                return ApiResponse<CourseQuestionDto>.ErrorResponse(validationError);
            if (dto.QuestionScore <= 0)
                return ApiResponse<CourseQuestionDto>.ErrorResponse("Question score must be greater than zero.");

            try
            {
                var question = new CourseQuestion
                {
                    CoursesMasterExamOid = dto.CoursesMasterExamOid,
                    QuestionText = dto.QuestionText,
                    QuestionText_Ar = dto.QuestionText_Ar,
                    QuestionTypeLookupId = MultiImageQuestionTypeId,
                    QuestionScore = dto.QuestionScore,
                    OrderNo = dto.OrderNo,
                    IsActive = dto.IsActive,
                    QuestionExplination = dto.QuestionExplination,
                    CreatedBy = dto.CreatedBy
                };
                _db.CourseQuestions.Add(question);

                var subQuestionOrder = 1;
                foreach (var subQuestionDto in dto.SubQuestions)
                {
                    var subQuestion = new CourseQuestionSubQuestion
                    {
                        CourseQuestion = question,
                        QuestionText = subQuestionDto.QuestionText.Trim(),
                        QuestionTextAr = subQuestionDto.QuestionTextAr.Trim(),
                        OrderNo = subQuestionOrder++,
                        CreatedBy = dto.CreatedBy
                    };
                    var choiceOrder = 1;
                    foreach (var choiceDto in subQuestionDto.Choices)
                    {
                        subQuestion.Choices.Add(new CourseQuestionSubQuestionChoice
                        {
                            ChoiceText = choiceDto.ChoiceText.Trim(),
                            ChoiceTextAr = choiceDto.ChoiceTextAr.Trim(),
                            IsCorrect = choiceDto.IsCorrect,
                            OrderNo = choiceOrder++,
                            CreatedBy = dto.CreatedBy
                        });
                    }
                    question.SubQuestions.Add(subQuestion);
                }

                await _db.SaveChangesAsync();
                var result = await _questionRepository.GetWithAnswersAsync(question.Oid);
                return ApiResponse<CourseQuestionDto>.SuccessResponse(MapToDto(result!), "Multiple sub-question created successfully");
            }
            catch { throw; }
        }

        private async Task<ApiResponse<CourseQuestionDto>> UpdateMultiQuestionAsync(
            CourseQuestion question, UpdateCourseQuestionDto dto)
        {
            var validationError = ValidateSubQuestions(dto.SubQuestions);
            if (validationError != null)
                return ApiResponse<CourseQuestionDto>.ErrorResponse(validationError);
            if (dto.QuestionScore <= 0)
                return ApiResponse<CourseQuestionDto>.ErrorResponse("Question score must be greater than zero.");
            var submittedSubQuestions = dto.SubQuestions!;

            try
            {
                var trackedQuestion = await _db.CourseQuestions
                    .Include(x => x.SubQuestions)
                        .ThenInclude(x => x.Choices)
                    .FirstAsync(x => x.Oid == question.Oid && !x.IsDeleted);

                trackedQuestion.CoursesMasterExamOid = dto.CoursesMasterExamOid;
                trackedQuestion.QuestionText = dto.QuestionText;
                trackedQuestion.QuestionText_Ar = dto.QuestionText_Ar;
                trackedQuestion.QuestionTypeLookupId = MultiImageQuestionTypeId;
                trackedQuestion.QuestionScore = dto.QuestionScore;
                trackedQuestion.OrderNo = dto.OrderNo;
                trackedQuestion.IsActive = dto.IsActive;
                trackedQuestion.QuestionExplination = dto.QuestionExplination;
                trackedQuestion.UpdatedBy = dto.UpdatedBy;
                trackedQuestion.UpdatedAt = DateTime.UtcNow;

                var incomingSubIds = submittedSubQuestions
                    .Where(x => x.Oid.HasValue && x.Oid != Guid.Empty)
                    .Select(x => x.Oid!.Value)
                    .ToHashSet();
                foreach (var existingSub in trackedQuestion.SubQuestions.Where(x => !x.IsDeleted && !incomingSubIds.Contains(x.Oid)))
                {
                    existingSub.IsDeleted = true;
                    existingSub.DeletedAt = DateTime.UtcNow;
                    foreach (var choice in existingSub.Choices.Where(x => !x.IsDeleted))
                    {
                        choice.IsDeleted = true;
                        choice.DeletedAt = DateTime.UtcNow;
                    }
                }

                for (var subIndex = 0; subIndex < submittedSubQuestions.Count; subIndex++)
                {
                    var subDto = submittedSubQuestions[subIndex];
                    var subQuestion = subDto.Oid.HasValue && subDto.Oid != Guid.Empty
                        ? trackedQuestion.SubQuestions.FirstOrDefault(x => x.Oid == subDto.Oid.Value && !x.IsDeleted)
                        : null;
                    if (subDto.Oid.HasValue && subDto.Oid != Guid.Empty && subQuestion == null)
                        return ApiResponse<CourseQuestionDto>.ErrorResponse("A sub-question does not belong to this question.");
                    if (subQuestion == null)
                    {
                        subQuestion = new CourseQuestionSubQuestion
                        {
                            CourseQuestionOid = trackedQuestion.Oid,
                            CreatedBy = dto.UpdatedBy
                        };
                        trackedQuestion.SubQuestions.Add(subQuestion);
                    }
                    subQuestion.QuestionText = subDto.QuestionText.Trim();
                    subQuestion.QuestionTextAr = subDto.QuestionTextAr.Trim();
                    subQuestion.OrderNo = subIndex + 1;
                    subQuestion.UpdatedBy = dto.UpdatedBy;
                    subQuestion.UpdatedAt = DateTime.UtcNow;

                    var incomingChoiceIds = subDto.Choices
                        .Where(x => x.Oid.HasValue && x.Oid != Guid.Empty)
                        .Select(x => x.Oid!.Value)
                        .ToHashSet();
                    foreach (var existingChoice in subQuestion.Choices.Where(x => !x.IsDeleted && !incomingChoiceIds.Contains(x.Oid)))
                    {
                        existingChoice.IsDeleted = true;
                        existingChoice.DeletedAt = DateTime.UtcNow;
                    }

                    for (var choiceIndex = 0; choiceIndex < subDto.Choices.Count; choiceIndex++)
                    {
                        var choiceDto = subDto.Choices[choiceIndex];
                        var choice = choiceDto.Oid.HasValue && choiceDto.Oid != Guid.Empty
                            ? subQuestion.Choices.FirstOrDefault(x => x.Oid == choiceDto.Oid.Value && !x.IsDeleted)
                            : null;
                        if (choiceDto.Oid.HasValue && choiceDto.Oid != Guid.Empty && choice == null)
                            return ApiResponse<CourseQuestionDto>.ErrorResponse("A choice does not belong to its sub-question.");
                        if (choice == null)
                        {
                            choice = new CourseQuestionSubQuestionChoice
                            {
                                SubQuestion = subQuestion,
                                CreatedBy = dto.UpdatedBy
                            };
                            subQuestion.Choices.Add(choice);
                        }
                        choice.ChoiceText = choiceDto.ChoiceText.Trim();
                        choice.ChoiceTextAr = choiceDto.ChoiceTextAr.Trim();
                        choice.IsCorrect = choiceDto.IsCorrect;
                        choice.OrderNo = choiceIndex + 1;
                        choice.UpdatedBy = dto.UpdatedBy;
                        choice.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _db.SaveChangesAsync();
                var result = await _questionRepository.GetWithAnswersAsync(trackedQuestion.Oid);
                return ApiResponse<CourseQuestionDto>.SuccessResponse(MapToDto(result!), "Multiple sub-question updated successfully");
            }
            catch { throw; }
        }

        private void DeletePhysicalImage(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var filePath = Path.Combine(ImageStoragePath, fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private static CourseQuestionDto MapToDto(CourseQuestion question)
        {
            return new CourseQuestionDto
            {
                Oid = question.Oid,
                CoursesMasterExamOid = question.CoursesMasterExamOid,
                ExamName = question.MasterExam?.CourseName,
                QuestionText = question.QuestionText,
                QuestionText_Ar = question.QuestionText_Ar,
                QuestionTypeLookupId = question.QuestionTypeLookupId,
                QuestionTypeName = question.QuestionTypeLookup?.LookupNameEn,
                QuestionImage = question.QuestionImage,
                QuestionImages = question.Images?
                    .Where(image => !image.IsDeleted)
                    .OrderBy(image => image.OrderNo)
                    .Select(image => new CourseQuestionImageDto
                    {
                        Oid = image.Oid,
                        FileName = image.FileName,
                        OrderNo = image.OrderNo
                    }).ToList() ?? new(),
                ExplanationImages = question.ExplanationImages?
                    .Where(image => !image.IsDeleted)
                    .OrderBy(image => image.OrderNo)
                    .Select(image => new CourseQuestionImageDto
                    {
                        Oid = image.Oid,
                        FileName = image.FileName,
                        OrderNo = image.OrderNo
                    }).ToList() ?? new(),
                SubQuestions = question.SubQuestions?
                    .Where(sub => !sub.IsDeleted)
                    .OrderBy(sub => sub.OrderNo)
                    .Select(sub => new CourseQuestionSubQuestionDto
                    {
                        Oid = sub.Oid,
                        QuestionText = sub.QuestionText,
                        QuestionTextAr = sub.QuestionTextAr,
                        OrderNo = sub.OrderNo,
                        Choices = sub.Choices.Where(choice => !choice.IsDeleted)
                            .OrderBy(choice => choice.OrderNo)
                            .Select(choice => new CourseQuestionSubQuestionChoiceDto
                            {
                                Oid = choice.Oid,
                                ChoiceText = choice.ChoiceText,
                                ChoiceTextAr = choice.ChoiceTextAr,
                                IsCorrect = choice.IsCorrect,
                                OrderNo = choice.OrderNo
                            }).ToList()
                    }).ToList() ?? new(),
                QuestionScore = question.QuestionScore,
                OrderNo = question.OrderNo,
                IsActive = question.IsActive,
                QuestionExplination = question.QuestionExplination,
                CorrectAnswer = question.CorrectAnswer,
                Question = question.Question,
                CorrectChoiceOid = question.CorrectChoiceOid,
                Answers = question.Answers?.Select(a => new CourseAnswerDto
                {
                    Oid = a.Oid,
                    QuestionId = a.QuestionId,
                    AnswerText = a.AnswerText,
                    AnswerText_Ar = a.AnswerText_Ar,
                    Question_Ask = a.Question_Ask,
                    CorrectAnswerOid = a.CorrectAnswerOid,
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

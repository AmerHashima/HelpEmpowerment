namespace HelpEmpowermentApi.DTOs
{
    public class StudentExamQuestionDto
    {
        public Guid Oid { get; set; }
        public Guid StudentExamOid { get; set; }
        public Guid QuestionOid { get; set; }
        public string? QuestionText { get; set; }
        public bool? IsCorrect { get; set; }
        public int? QuestionScore { get; set; }
        public int? ObtainedScore { get; set; }
        public Guid? QuestionStatusLookupId { get; set; }
        public string? QuestionStatusName { get; set; }
        public string? ExamName { get; set; }
        public Guid? CoursesMasterExamOid { get; set; }
        public string QuestionText_Ar { get; set; } = string.Empty;

        public Guid? QuestionTypeLookupId { get; set; }
        public string QuestionExplination { get; set; } = string.Empty;

        public string? QuestionTypeName { get; set; }
        public int? OrderNo { get; set; }
        public bool IsActive { get; set; }
        public bool CorrectAnswer { get; set; }
        public bool Question { get; set; }
        public Guid? CorrectChoiceOid { get; set; }
        public List<StudentExamQuestionAnswerDto> Answers { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class StudentExamQuestionAnswerDto
    {
        public Guid Oid { get; set; }
        public Guid SelectedAnswerOid { get; set; }
        public string? SelectedAnswerText { get; set; }
        public Guid? AnswerSelectedAnswerOid { get; set; }
    }

    public class CreateStudentExamQuestionDto
    {
        public Guid StudentExamOid { get; set; }
        public Guid? QuestionStatusLookupId { get; set; }

        public Guid QuestionOid { get; set; }
        public List<Guid> SelectedAnswerOids { get; set; } = new();
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateStudentExamQuestionDto
    {
        public Guid Oid { get; set; }
        public Guid? QuestionStatusLookupId { get; set; }

        public List<Guid> SelectedAnswerOids { get; set; } = new();
        public bool? IsCorrect { get; set; }
        public int? QuestionScore { get; set; }
        public int? ObtainedScore { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class QuestionAnswerSubmission
    {
        public Guid QuestionOid { get; set; }
        public List<Guid> SelectedAnswerOids { get; set; } = new();
    }

    public class SubmitMultipleQuestionsDto
    {
        public Guid StudentExamOid { get; set; }
        public List<QuestionAnswerSubmission> Questions { get; set; } = new();
        public Guid? CreatedBy { get; set; }
    }

    public class MultipleQuestionsSubmissionResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalScore { get; set; }
        public int ObtainedScore { get; set; }
        public List<QuestionResultDto> Questions { get; set; } = new();
        public List<string>? Errors { get; set; }
    }

    public class QuestionResultDto
    {
        public Guid QuestionOid { get; set; }
        public string? QuestionText { get; set; }
        public List<Guid> SelectedAnswerOids { get; set; } = new();
        public bool IsCorrect { get; set; }
        public int QuestionScore { get; set; }
        public int ObtainedScore { get; set; }
    }

    public class ValidateAnswersDto
    {
        public Guid StudentExamOid { get; set; }
        public Guid QuestionOid { get; set; }
        public List<AnswerSubmission> Answers { get; set; } = new();
        public Guid? CreatedBy { get; set; }
    }

    public class AnswerSubmission
    {
        public Guid SelectedAnswerOid { get; set; }
        public Guid? AnswerSelectedAnswerOid { get; set; }
    }

    public class AnswerValidationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Guid QuestionOid { get; set; }
        public string? QuestionText { get; set; }
        public List<Guid> SelectedAnswerOids { get; set; } = new();
        public List<Guid> CorrectAnswerOids { get; set; } = new();
        public bool IsCorrect { get; set; }
        public int QuestionScore { get; set; }
        public int ObtainedScore { get; set; }
        public List<AnswerValidationDetail> AnswerDetails { get; set; } = new();
    }

    public class AnswerValidationDetail
    {
        public Guid AnswerOid { get; set; }
        public string? AnswerText { get; set; }
        public bool IsSelected { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public Guid? CorrectAnswerOid { get; set; }
    }
}
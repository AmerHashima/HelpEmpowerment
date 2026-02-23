namespace HelpEmpowermentApi.DTOs
{
    public class StudentExamQuestionDto
    {
        public Guid Oid { get; set; }
        public Guid StudentExamOid { get; set; }
        public Guid QuestionOid { get; set; }
        public string? QuestionText { get; set; }
        public Guid? SelectedAnswerOid { get; set; }
        public List<Guid> SelectedAnswerOids { get; set; } = new();
        public string? SelectedAnswerText { get; set; }
        public bool? IsCorrect { get; set; }
        public int? QuestionScore { get; set; }
        public int? ObtainedScore { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateStudentExamQuestionDto
    {
        public Guid StudentExamOid { get; set; }
        public Guid QuestionOid { get; set; }
        public List<Guid> SelectedAnswerOids { get; set; } = new();
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateStudentExamQuestionDto
    {
        public Guid Oid { get; set; }
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
}
namespace HelpEmpowermentApi.DTOs
{
    public class StudentExamDto
    {
        public Guid Oid { get; set; }
        public Guid StudentOid { get; set; }
        public string? StudentName { get; set; }
        public Guid CoursesMasterExamOid { get; set; }
        public string? ExamName { get; set; }
        public int AttemptNo { get; set; }
        public int? TotalScore { get; set; }
        public int? ObtainedScore { get; set; }
        public int? PassPercent { get; set; }
        public decimal? Percentage { get; set; }
        public bool? IsPassed { get; set; }
        public Guid? ExamStatusLookupId { get; set; }
        public string? ExamStatusName { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public List<StudentExamQuestionDto> ExamQuestions { get; set; } = new();
        public List<QuestionStatusSummaryDto> StatusSummary { get; set; } = new();
    }

    public class CreateStudentExamDto
    {
        public Guid StudentOid { get; set; }
        public Guid CoursesMasterExamOid { get; set; }
        public int AttemptNo { get; set; } = 1;
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateStudentExamDto
    {
        public Guid Oid { get; set; }
        public int? TotalScore { get; set; }
        public int? ObtainedScore { get; set; }
        public int? PassPercent { get; set; }
        public bool? IsPassed { get; set; }
        public Guid? ExamStatusLookupId { get; set; }
        public DateTime? FinishedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class SubmitStudentExamDto
    {
        public Guid StudentExamOid { get; set; }
      // public List<SubmitAnswerDto> Answers { get; set; } = new();
        public Guid? UpdatedBy { get; set; }
    }

    public class SubmitAnswerDto
    {
        public Guid QuestionOid { get; set; }
        public Guid? SelectedAnswerOid { get; set; }
    }

    public class StudentExamSummaryRequestDto
    {
        public Guid StudentId { get; set; }
        public Guid ExamId { get; set; }
    }

    public class StudentExamSummaryDto
    {
        public Guid StudentExamOid { get; set; }
        public Guid StudentOid { get; set; }
        public string? StudentName { get; set; }
        public string? ExamName { get; set; }
        public int AttemptNo { get; set; }
        public int? TotalScore { get; set; }
        public int? ObtainedScore { get; set; }
        public decimal? Percentage { get; set; }
        public bool? IsPassed { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public int TotalQuestions { get; set; }
        public List<QuestionStatusSummaryDto> StatusSummary { get; set; } = new();
    }

    public class QuestionStatusSummaryDto
    {
        public Guid? QuestionStatusLookupId { get; set; }
        public string? StatusName { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
namespace HelpEmpowermentApi.DTOs
{
    public class StudentExamQuestionDto
    {
        public Guid Oid { get; set; }
        public Guid StudentExamOid { get; set; }
        public Guid QuestionOid { get; set; }
        public string? QuestionText { get; set; }
        public Guid? SelectedAnswerOid { get; set; }
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
        public Guid? SelectedAnswerOid { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateStudentExamQuestionDto
    {
        public Guid Oid { get; set; }
        public Guid? SelectedAnswerOid { get; set; }
        public bool? IsCorrect { get; set; }
        public int? QuestionScore { get; set; }
        public int? ObtainedScore { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
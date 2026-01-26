namespace StandardArticture.DTOs
{
    public class CourseAnswerDto
    {
        public Guid Oid { get; set; }
        public Guid QuestionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int? OrderNo { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateCourseAnswerDto
    {
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
        public int? OrderNo { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateCourseAnswerDto
    {
        public Guid Oid { get; set; }
        public Guid QuestionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int? OrderNo { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
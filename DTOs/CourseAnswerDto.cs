namespace HelpEmpowermentApi.DTOs
{
    public class CourseAnswerDto
    {
        public Guid Oid { get; set; }
        public Guid QuestionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool Question_Ask { get; set; } = false;
        public string AnswerText_Ar { get; set; } = string.Empty;

        public Guid? CorrectAnswerOid { get; set; }
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
        public bool Question_Ask { get; set; } = false;
        public Guid QuestionId { get; set; }
        public string AnswerText_Ar { get; set; } = string.Empty;

        public Guid? CorrectAnswerOid { get; set; }
        public bool IsCorrect { get; set; } = false;
        public int? OrderNo { get; set; }
        public Guid? CreatedBy { get; set; }
    }

    public class UpdateCourseAnswerDto
    {
        public Guid Oid { get; set; }
        public Guid QuestionId { get; set; }
        public bool Question_Ask { get; set; } = false;
        public string AnswerText_Ar { get; set; } = string.Empty;

        public Guid? CorrectAnswerOid { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int? OrderNo { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
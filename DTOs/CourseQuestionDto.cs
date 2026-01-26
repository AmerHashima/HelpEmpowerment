namespace StandardArticture.DTOs
{
    public class CourseQuestionDto
    {
        public Guid Oid { get; set; }
        public Guid CoursesMasterExamOid { get; set; }
        public string? ExamName { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public Guid? QuestionTypeLookupId { get; set; }
        public string? QuestionTypeName { get; set; }
        public int QuestionScore { get; set; }
        public int? OrderNo { get; set; }
        public bool IsActive { get; set; }
        public bool CorrectAnswer { get; set; }
        public bool Question { get; set; }
        public Guid? CorrectChoiceOid { get; set; }
        public List<CourseAnswerDto> Answers { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class CreateCourseQuestionDto
    {
        public Guid CoursesMasterExamOid { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public Guid? QuestionTypeLookupId { get; set; }
        public int QuestionScore { get; set; } = 1;
        public int? OrderNo { get; set; }
        public bool IsActive { get; set; } = true;
        public bool CorrectAnswer { get; set; } = false;
        public bool Question { get; set; } = false;
        public Guid? CorrectChoiceOid { get; set; }
        public Guid? CreatedBy { get; set; }
        public List<CreateCourseAnswerDto> Answers { get; set; } = new();
    }

    public class UpdateCourseQuestionDto
    {
        public Guid Oid { get; set; }
        public Guid CoursesMasterExamOid { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public Guid? QuestionTypeLookupId { get; set; }
        public int QuestionScore { get; set; }
        public int? OrderNo { get; set; }
        public bool IsActive { get; set; }
        public bool CorrectAnswer { get; set; }
        public bool Question { get; set; }
        public Guid? CorrectChoiceOid { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
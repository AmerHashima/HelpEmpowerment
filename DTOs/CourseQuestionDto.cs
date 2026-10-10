namespace HelpEmpowermentApi.DTOs
{
    public class CourseQuestionDto
    {
        public Guid Oid { get; set; }
        public Guid CoursesMasterExamOid { get; set; }
        public string? ExamName { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionText_Ar { get; set; } = string.Empty;

        public Guid? QuestionTypeLookupId { get; set; }
        public string QuestionExplination { get; set; } = string.Empty;
        public string QuestionImage { get; set; } = string.Empty;
        public List<CourseQuestionImageDto> QuestionImages { get; set; } = new();
        public List<CourseQuestionImageDto> ExplanationImages { get; set; } = new();
        public List<CourseQuestionSubQuestionDto> SubQuestions { get; set; } = new();

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

    public class CourseQuestionImageDto
    {
        public Guid Oid { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int OrderNo { get; set; }
    }

    public class CreateCourseQuestionDto
    {
        public Guid CoursesMasterExamOid { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionText_Ar { get; set; } = string.Empty;

        public string QuestionExplination { get; set; } = string.Empty;
        public string QuestionImage { get; set; } = string.Empty;

        public Guid? QuestionTypeLookupId { get; set; }
        public int QuestionScore { get; set; } = 1;
        public int? OrderNo { get; set; }
        public bool IsActive { get; set; } = true;

        public bool CorrectAnswer { get; set; } = false;
        public bool Question { get; set; } = false;
        public Guid? CorrectChoiceOid { get; set; }
        public Guid? CreatedBy { get; set; }
        public List<CreateCourseAnswerDto> Answers { get; set; } = new();
        public List<UpsertCourseQuestionSubQuestionDto> SubQuestions { get; set; } = new();
    }

    public class UpdateCourseQuestionDto
    {
        public Guid Oid { get; set; }
        public Guid CoursesMasterExamOid { get; set; }
        public string QuestionText_Ar { get; set; } = string.Empty;
        public string QuestionImage { get; set; } = string.Empty;

        public string QuestionExplination { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public Guid? QuestionTypeLookupId { get; set; }
        public int QuestionScore { get; set; }
        public int? OrderNo { get; set; }
        public bool IsActive { get; set; }
        public bool CorrectAnswer { get; set; } = false;
        public bool Question { get; set; } = false;
        public Guid? CorrectChoiceOid { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        // ✅ ADD: Support for updating answers
        public List<UpdateCourseAnswerDto>? Answers { get; set; }
        public List<UpsertCourseQuestionSubQuestionDto>? SubQuestions { get; set; }
    }

    public class CourseQuestionSubQuestionDto
    {
        public Guid Oid { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionTextAr { get; set; } = string.Empty;
        public int OrderNo { get; set; }
        public List<CourseQuestionSubQuestionChoiceDto> Choices { get; set; } = new();
    }

    public class CourseQuestionSubQuestionChoiceDto
    {
        public Guid Oid { get; set; }
        public string ChoiceText { get; set; } = string.Empty;
        public string ChoiceTextAr { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OrderNo { get; set; }
    }

    public class UpsertCourseQuestionSubQuestionDto
    {
        public Guid? Oid { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionTextAr { get; set; } = string.Empty;
        public int OrderNo { get; set; }
        public List<UpsertCourseQuestionSubQuestionChoiceDto> Choices { get; set; } = new();
    }

    public class UpsertCourseQuestionSubQuestionChoiceDto
    {
        public Guid? Oid { get; set; }
        public string ChoiceText { get; set; } = string.Empty;
        public string ChoiceTextAr { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int OrderNo { get; set; }
    }
}

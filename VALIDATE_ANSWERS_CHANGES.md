# Summary of Changes - ValidateAnswers Endpoint

## What Changed

The `validate-answers` endpoint has been updated to **save validation results to the database**.

## Previous Behavior
- ❌ Only validated answers without saving
- ❌ No database records created
- ✅ Returned detailed validation results

## Current Behavior
- ✅ Validates answers
- ✅ **Creates rows in `StudentExamQuestion` table**
- ✅ Saves `IsCorrect` status and scores
- ✅ Returns detailed validation results

## Updated Request Structure

```json
{
  "studentExamOid": "guid-of-student-exam",  // ✨ NEW - Required
  "questionOid": "guid-of-question",
  "answers": [
    {
      "selectedAnswerOid": "guid-of-answer",
      "answerSelectedAnswerOid": "guid-or-null"
    }
  ],
  "createdBy": "guid-of-user"  // ✨ NEW - Optional
}
```

## Database Impact

When you call this endpoint, it will:

1. **Delete** existing `StudentExamQuestion` rows for this question in the exam
2. **Create** new rows (one per selected answer) with:
   ```
   StudentExamOid = provided studentExamOid
   QuestionOid = provided questionOid
   SelectedAnswerOid = each selected answer
   IsCorrect = calculated based on validation
   QuestionScore = from CourseQuestion
   ObtainedScore = full score if correct, 0 if wrong
   CreatedBy = provided createdBy
   CreatedAt = current timestamp
   ```

## Example Flow

### Request
```json
POST /api/StudentExamQuestions/validate-answers
{
  "studentExamOid": "exam-123",
  "questionOid": "question-456",
  "answers": [
    {
      "selectedAnswerOid": "answer-a",
      "answerSelectedAnswerOid": null
    },
    {
      "selectedAnswerOid": "answer-b",
      "answerSelectedAnswerOid": "answer-a"
    }
  ],
  "createdBy": "user-789"
}
```

### What Happens in Database

**Before:**
```
StudentExamQuestion table:
(empty or has old answers for this question)
```

**After (if 2 answers selected):**
```
StudentExamQuestion table:
| Oid | StudentExamOid | QuestionOid | SelectedAnswerOid | IsCorrect | QuestionScore | ObtainedScore | CreatedBy |
|-----|----------------|-------------|-------------------|-----------|---------------|---------------|-----------|
| 001 | exam-123       | question-456| answer-a          | true      | 10            | 10            | user-789  |
| 002 | exam-123       | question-456| answer-b          | true      | 10            | 10            | user-789  |
```

Note: All rows for the same question share the same `IsCorrect` and `ObtainedScore` values.

## Use Cases

### ✅ Best For:
- Single question submission with immediate feedback
- Validating `CorrectAnswerOid` property matching
- Getting detailed validation results while saving
- Questions requiring strict validation rules

### ❌ Not Ideal For:
- Bulk submission of many questions (use `submit-multiple` instead)
- Practice mode without saving (this now saves to DB)

## Comparison with Other Endpoints

| Endpoint | Saves to DB | Returns Details | CorrectAnswerOid Check | Multiple Questions |
|----------|-------------|-----------------|------------------------|-------------------|
| `validate-answers` | ✅ Yes | ✅ Full details | ✅ Yes | ❌ One at a time |
| `submit-multiple` | ✅ Yes | ❌ Simple message | ❌ No | ✅ Batch |
| `POST /` (Create) | ✅ Yes | ❌ Simple message | ❌ No | ❌ One at a time |

## Migration Notes

If you were previously using this endpoint for **practice mode without saving**, you should:
1. Create a new endpoint for validation-only (no save)
2. Or use this endpoint but be aware it now creates database records
3. Ensure you always provide a valid `studentExamOid`

## Updated DTOs

### ValidateAnswersDto
```csharp
public class ValidateAnswersDto
{
    public Guid StudentExamOid { get; set; }        // NEW
    public Guid QuestionOid { get; set; }
    public List<AnswerSubmission> Answers { get; set; } = new();
    public Guid? CreatedBy { get; set; }            // NEW
}
```

### AnswerSubmission
```csharp
public class AnswerSubmission
{
    public Guid SelectedAnswerOid { get; set; }
    public Guid? AnswerSelectedAnswerOid { get; set; }
}
```

## Files Modified
- ✅ `DTOs/StudentExamQuestionDto.cs` - Added `StudentExamOid` and `CreatedBy` to `ValidateAnswersDto`
- ✅ `Services/StudentExamQuestionService.cs` - Added database save logic to `ValidateAnswersAsync`
- ✅ `ANSWER_VALIDATION_ENDPOINT.md` - Updated documentation

## Testing Checklist

- [ ] Verify `studentExamOid` is required
- [ ] Verify existing answers are deleted before creating new ones
- [ ] Verify correct answers create rows with `IsCorrect=true` and full score
- [ ] Verify incorrect answers create rows with `IsCorrect=false` and `ObtainedScore=0`
- [ ] Verify multiple selected answers create multiple rows
- [ ] Verify `CreatedBy` is saved correctly
- [ ] Verify response still returns detailed validation results
- [ ] Verify invalid `studentExamOid` returns appropriate error

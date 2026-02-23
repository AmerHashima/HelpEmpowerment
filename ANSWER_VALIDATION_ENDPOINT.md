# Answer Validation Endpoint

## Overview
This endpoint validates selected answers for a question and **saves the results to the database**. It performs:
1. Validates each selected answer's `SelectedAnswerOid` matches an answer in the database
2. Validates each answer's `AnswerSelectedAnswerOid` matches the `CorrectAnswerOid` stored in the database
3. Checks if the set of selected answers matches the set of correct answers (using `IsCorrect` flag)
4. **Creates rows in `StudentExamQuestion` table** with the validation results

## Endpoint

**POST** `/api/StudentExamQuestions/validate-answers`

## Request Body

```json
{
  "studentExamOid": "guid-of-student-exam",
  "questionOid": "guid-of-question",
  "answers": [
    {
      "selectedAnswerOid": "guid-of-answer-1",
      "answerSelectedAnswerOid": "guid-of-correct-answer"
    },
    {
      "selectedAnswerOid": "guid-of-answer-2",
      "answerSelectedAnswerOid": null
    }
  ],
  "createdBy": "guid-of-user"
}
```

### Parameters
- **studentExamOid** (Guid, required): The ID of the student exam to save results to
- **questionOid** (Guid, required): The ID of the question to validate
- **answers** (List<AnswerSubmission>, required): List of answer submissions, each containing:
  - **selectedAnswerOid** (Guid, required): The ID of the answer selected by the user
  - **answerSelectedAnswerOid** (Guid?, optional): The `CorrectAnswerOid` value for this answer (must match the database value)
- **createdBy** (Guid?, optional): The ID of the user submitting the answers

## What Happens

### Step 1: Validation
The endpoint validates the student exam, question, and answers exist.

### Step 2: CorrectAnswerOid Matching
For each answer submission, it checks if the `answerSelectedAnswerOid` matches the database's `CorrectAnswerOid`.

### Step 3: IsCorrect Flag Matching
It compares the selected answers with the correct answers (using `IsCorrect` flag).

### Step 4: **Save to Database** ✨
After validation:
- Deletes any existing answers for this question in the exam
- Creates **multiple rows** in `StudentExamQuestion` table (one per selected answer)
- Each row contains:
  - `StudentExamOid`
  - `QuestionOid`
  - `SelectedAnswerOid`
  - `IsCorrect` (true/false based on validation)
  - `QuestionScore` (max possible score)
  - `ObtainedScore` (actual score earned)
  - `CreatedBy`
  - `CreatedAt`

## Response

### Success Response - Correct Answer (200 OK)

```json
{
  "success": true,
  "message": "Correct answer!",
  "data": {
    "success": true,
    "message": "Correct answer!",
    "questionOid": "guid-of-question",
    "questionText": "What is 2 + 2?",
    "selectedAnswerOids": [
      "answer-guid-1"
    ],
    "correctAnswerOids": [
      "answer-guid-1"
    ],
    "isCorrect": true,
    "questionScore": 5,
    "obtainedScore": 5,
    "answerDetails": [
      {
        "answerOid": "answer-guid-1",
        "answerText": "4",
        "isSelected": true,
        "isCorrectAnswer": true,
        "correctAnswerOid": null
      },
      {
        "answerOid": "answer-guid-2",
        "answerText": "3",
        "isSelected": false,
        "isCorrectAnswer": false,
        "correctAnswerOid": "answer-guid-1"
      }
    ]
  }
}
```

### Error Response - CorrectAnswerOid Mismatch (200 OK)

```json
{
  "success": true,
  "message": "Incorrect answer: Answer guid-1: CorrectAnswerOid mismatch. Expected guid-x, got guid-y",
  "data": {
    "success": false,
    "message": "Incorrect answer: Answer guid-1: CorrectAnswerOid mismatch. Expected guid-x, got guid-y",
    "questionOid": "guid-of-question",
    "questionText": "What is 2 + 2?",
    "selectedAnswerOids": ["answer-guid-1"],
    "correctAnswerOids": ["answer-guid-1"],
    "isCorrect": false,
    "questionScore": 5,
    "obtainedScore": 0,
    "answerDetails": [...]
  }
}
```

### Error Response - Wrong Answers Selected (200 OK)

```json
{
  "success": true,
  "message": "Incorrect answer",
  "data": {
    "success": true,
    "message": "Incorrect answer",
    "questionOid": "guid-of-question",
    "questionText": "What is 2 + 2?",
    "selectedAnswerOids": ["answer-guid-2"],
    "correctAnswerOids": ["answer-guid-1"],
    "isCorrect": false,
    "questionScore": 5,
    "obtainedScore": 0,
    "answerDetails": [...]
  }
}
```

### Error Response (400 Bad Request)

```json
{
  "success": false,
  "message": "Question not found",
  "data": null
}
```

## Validation Logic

### Step 1: Primary Validation (IsCorrect Flag)
The endpoint first checks the `IsCorrect` boolean flag on each `CourseAnswer`:
```csharp
var correctAnswersFromFlag = allAnswers.Where(a => a.IsCorrect).Select(a => a.Oid).ToList();
```

### Step 2: Fallback Validation (CorrectAnswerOid Property)
If the `IsCorrect` flag method doesn't find correct answers, it checks the `CorrectAnswerOid` property:
```csharp
// For each selected answer, check if its CorrectAnswerOid points to another selected answer
foreach (var selectedAnswerOid in dto.SelectedAnswerOids)
{
    var selectedAnswer = allAnswers.FirstOrDefault(a => a.Oid == selectedAnswerOid);
    if (selectedAnswer?.CorrectAnswerOid.HasValue == true)
    {
        if (dto.SelectedAnswerOids.Contains(selectedAnswer.CorrectAnswerOid.Value))
        {
            correctAnswersFromOid.Add(selectedAnswer.CorrectAnswerOid.Value);
        }
    }
}
```

### Step 3: Comparison
The selected answers are compared with the correct answers:
- Selected answers must **exactly match** correct answers (no more, no less)
- Both selected and correct answer lists are sorted before comparison
- If they match perfectly → `isCorrect = true`
- If they don't match → `isCorrect = false`

### Step 4: Scoring
```csharp
int obtainedScore = isCorrect ? question.QuestionScore : 0;
```
- If correct: Full score
- If incorrect: 0 score

## Response Fields Explained

### AnswerValidationResult
- **success**: Indicates if the API call succeeded
- **message**: "Correct answer!" or "Incorrect answer"
- **questionOid**: The question being validated
- **questionText**: The question text
- **selectedAnswerOids**: The answers selected by the user
- **correctAnswerOids**: The actual correct answers
- **isCorrect**: Boolean indicating if the answer is correct
- **questionScore**: Maximum possible score for this question
- **obtainedScore**: Score obtained (full score or 0)
- **answerDetails**: Detailed information about each answer option

### AnswerValidationDetail
- **answerOid**: Unique ID of the answer
- **answerText**: The text of the answer option
- **isSelected**: Whether this answer was selected by the user
- **isCorrectAnswer**: Whether this is a correct answer (from `IsCorrect` flag)
- **correctAnswerOid**: Points to the correct answer OID (if applicable)

## Use Cases

### 1. Quiz/Exam Validation Without Saving
Validate answers without storing them in the database. Useful for:
- Practice quizzes
- Preview/review mode
- Immediate feedback without committing results

### 2. Multi-Select Questions
Supports questions with multiple correct answers:
```json
{
  "questionOid": "guid",
  "selectedAnswerOids": ["answer-1", "answer-2", "answer-3"]
}
```

### 3. Learning Mode
Get detailed feedback showing:
- Which answers were selected
- Which answers are correct
- How the CorrectAnswerOid relationships work

## Example Usage

### JavaScript/Fetch
```javascript
const validateAnswers = async (questionOid, answers) => {
  const response = await fetch('/api/StudentExamQuestions/validate-answers', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      questionOid,
      answers: answers.map(a => ({
        selectedAnswerOid: a.answerOid,
        answerSelectedAnswerOid: a.correctAnswerOid
      }))
    })
  });

  const result = await response.json();

  if (result.success && result.data.isCorrect) {
    console.log(`Correct! You scored ${result.data.obtainedScore}/${result.data.questionScore}`);
  } else {
    console.log(`Incorrect. ${result.message}`);
  }

  return result;
};

// Usage
const answers = [
  { answerOid: 'guid-1', correctAnswerOid: 'guid-x' },
  { answerOid: 'guid-2', correctAnswerOid: null }
];
await validateAnswers('question-guid', answers);
```

### C# HttpClient
```csharp
var dto = new ValidateAnswersDto
{
    QuestionOid = questionId,
    Answers = new List<AnswerSubmission>
    {
        new AnswerSubmission
        {
            SelectedAnswerOid = answer1Id,
            AnswerSelectedAnswerOid = correctAnswer1Id
        },
        new AnswerSubmission
        {
            SelectedAnswerOid = answer2Id,
            AnswerSelectedAnswerOid = null
        }
    }
};

var response = await httpClient.PostAsJsonAsync(
    "/api/StudentExamQuestions/validate-answers", 
    dto
);

var result = await response.Content.ReadFromJsonAsync<ApiResponse<AnswerValidationResult>>();

if (result.Success && result.Data.IsCorrect)
{
    Console.WriteLine($"Correct! Score: {result.Data.ObtainedScore}/{result.Data.QuestionScore}");
}
else
{
    Console.WriteLine($"Incorrect: {result.Message}");
}
```

## Real-World Example

Given a question "What is 2 + 2?" with answers:
- Answer A (guid-a): "4" - IsCorrect=true, CorrectAnswerOid=null
- Answer B (guid-b): "3" - IsCorrect=false, CorrectAnswerOid=guid-a
- Answer C (guid-c): "5" - IsCorrect=false, CorrectAnswerOid=guid-a

### Request
```json
{
  "questionOid": "question-guid",
  "answers": [
    {
      "selectedAnswerOid": "guid-a",
      "answerSelectedAnswerOid": null
    }
  ]
}
```

### Validation Process
1. ✅ Check "guid-a" exists in database → Found
2. ✅ Check AnswerSelectedAnswerOid (null) matches database CorrectAnswerOid (null) → Match
3. ✅ Check if selected answers [guid-a] match correct answers [guid-a] → Match
4. ✅ **Result: CORRECT** - Score: 5/5

## Differences from Submit Endpoints

| Feature | validate-answers | submit-multiple | Create |
|---------|------------------|-----------------|--------|
| **Saves to DB** | ✅ Yes | ✅ Yes | ✅ Yes |
| **Creates Records** | ✅ Yes (multiple rows) | ✅ Yes (multiple rows) | ✅ Yes (multiple rows) |
| **Returns Details** | ✅ Full validation details | ❌ Simple message | ❌ Simple message |
| **CorrectAnswerOid Check** | ✅ Yes (validates match) | ❌ No | ❌ No |
| **Use Case** | Single question submission with validation | Batch submission | Single question submission |
| **Detailed Errors** | ✅ Yes | ✅ Yes | ✅ Yes |
| **Multiple Questions** | ❌ One at a time | ✅ Multiple questions | ❌ One at a time |
| **Replaces Existing** | ✅ Yes (deletes & recreates) | ✅ Yes (deletes & recreates) | ✅ Yes (deletes & recreates) |

## Key Features

1. **✅ Saves to Database**: Creates `StudentExamQuestion` rows with validation results
2. **✅ Three-Level Validation**: 
   - Answer existence
   - CorrectAnswerOid matching
   - IsCorrect flag matching
3. **✅ Detailed Response**: Returns all answer options with their validation status
4. **✅ Automatic Scoring**: Calculates and stores score based on correctness
5. **✅ Multiple Answers Support**: Handles questions with multiple correct answers
6. **✅ Replaces Existing Answers**: Deletes old answers for the question before creating new ones

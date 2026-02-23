# Multi-Answer Implementation for Student Exam Questions

## Overview
This implementation allows students to select multiple answers for a single question and stores each selected answer as a separate row in the `student_exam_questions` table.

## Changes Made

### 1. **Model Changes** (`Models/StudentExamQuestion.cs`)
- Removed the `SelectedAnswerOids` JSON column (commented out code)
- Each answer is now stored as a separate row with a single `SelectedAnswerOid`
- Multiple rows share the same `StudentExamOid`, `QuestionOid`, `IsCorrect`, `QuestionScore`, and `ObtainedScore`

### 2. **DTO Updates** (`DTOs/StudentExamQuestionDto.cs`)
Added new DTOs:
- **`QuestionSubmissionDto`**: Represents a question with multiple selected answers
- **`SubmitMultipleQuestionsDto`**: Contains multiple questions for bulk submission
- **`MultipleQuestionsSubmissionResult`**: Response with scoring and results
- **`QuestionResultDto`**: Individual question result with selected answers and scores

### 3. **Service Updates** (`Services/StudentExamQuestionService.cs`)

#### **CreateAsync Method**
- Now creates multiple `StudentExamQuestion` rows, one for each selected answer
- Validates all selected answers against the question's available answers
- Checks if the selected answers exactly match the correct answers
- Calculates score based on whether ALL selected answers are correct
- Deletes existing answers for the question before creating new ones

#### **UpdateAsync Method**
- Deletes all existing answer rows for the question
- Creates new rows for each newly selected answer
- Maintains the same scoring logic as CreateAsync

#### **GetByStudentExamIdAsync Method**
- Groups multiple answer rows by `QuestionOid`
- Aggregates all `SelectedAnswerOid` values into a `SelectedAnswerOids` list
- Returns one DTO per question with all selected answers

#### **SubmitMultipleQuestionsAsync Method** (NEW)
- Processes multiple questions in a single API call
- For each question:
  - Validates the question and answers
  - Deletes existing answer rows
  - Creates new rows for each selected answer
  - Checks correctness and calculates scores
- Returns aggregated results with:
  - Total questions submitted
  - Number of correct answers
  - Total possible score
  - Obtained score
  - Individual question results
  - Any errors encountered

### 4. **Controller Updates** (`Controllers/StudentExamQuestionsController.cs`)
Added new endpoint:
- **POST `/api/StudentExamQuestions/submit-multiple`**: Submits multiple questions with their answers at once

## API Usage

### Submit Multiple Questions
```http
POST /api/StudentExamQuestions/submit-multiple
Content-Type: application/json

{
  "studentExamOid": "guid-of-exam",
  "createdBy": "guid-of-user",
  "questions": [
    {
      "questionOid": "guid-of-question-1",
      "selectedAnswerOids": ["answer-guid-1", "answer-guid-2"]
    },
    {
      "questionOid": "guid-of-question-2",
      "selectedAnswerOids": ["answer-guid-3"]
    }
  ]
}
```

### Response
```json
{
  "success": true,
  "message": "Submitted 2 of 2 questions successfully. Score: 5/10 (1/2 correct)",
  "data": {
    "success": true,
    "message": "Submitted 2 of 2 questions successfully. Score: 5/10 (1/2 correct)",
    "totalQuestions": 2,
    "correctAnswers": 1,
    "totalScore": 10,
    "obtainedScore": 5,
    "questions": [
      {
        "questionOid": "guid-of-question-1",
        "questionText": "What is 2+2?",
        "selectedAnswerOids": ["answer-guid-1", "answer-guid-2"],
        "isCorrect": true,
        "questionScore": 5,
        "obtainedScore": 5
      },
      {
        "questionOid": "guid-of-question-2",
        "questionText": "What is the capital of France?",
        "selectedAnswerOids": ["answer-guid-3"],
        "isCorrect": false,
        "questionScore": 5,
        "obtainedScore": 0
      }
    ],
    "errors": []
  }
}
```

## Database Structure

### Before (Old Approach)
```
student_exam_questions
- Oid
- StudentExamOid
- QuestionOid
- SelectedAnswerOid (single answer)
- SelectedAnswerOids (JSON string) ← REMOVED
- IsCorrect
- QuestionScore
- ObtainedScore
```

### After (New Approach)
```
student_exam_questions (multiple rows per question)
Row 1: StudentExamOid | QuestionOid | SelectedAnswerOid (Answer A) | IsCorrect | Score
Row 2: StudentExamOid | QuestionOid | SelectedAnswerOid (Answer B) | IsCorrect | Score
Row 3: StudentExamOid | QuestionOid | SelectedAnswerOid (Answer C) | IsCorrect | Score
```

## Scoring Logic
- A question is marked **correct** only if:
  1. ALL correct answers are selected
  2. NO incorrect answers are selected
- If correct: `ObtainedScore = QuestionScore`
- If incorrect: `ObtainedScore = 0`
- The same `IsCorrect`, `QuestionScore`, and `ObtainedScore` values are stored in all rows for the same question

## Benefits
1. **Normalized Data**: Each answer is a separate row (more relational)
2. **Easy Querying**: Can query individual answer selections
3. **Audit Trail**: Each answer selection can be tracked independently
4. **No JSON Parsing**: Avoids JSON serialization/deserialization overhead
5. **Database Constraints**: Can add foreign key constraints on `SelectedAnswerOid`

## Migration Notes
If you have existing data with the `SelectedAnswerOids` JSON column, you'll need to:
1. Create a data migration to split JSON arrays into multiple rows
2. Add a database migration to drop the `SelectedAnswerOids` column (if it exists)

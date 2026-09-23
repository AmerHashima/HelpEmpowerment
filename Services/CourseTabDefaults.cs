using System.Text.Json;

namespace HelpEmpowermentApi.Services;

public static class CourseTabDefaults
{
    public static readonly string[] TabKeys =
        ["exam-simulator", "recorded-course", "live-course", "webinar", "quiz-game", "faq", "reviews"];

    public static string Create(string courseCode, string tabKey)
    {
        var code = courseCode.Equals("capm", StringComparison.OrdinalIgnoreCase) ? "CAPM" : "PMP";
        var image = tabKey switch
        {
            "quiz-game" => "assets/images/quizGame/quizGame.jpeg",
            "reviews" => "assets/images/reviewers/review.jpeg",
            "recorded-course" => "assets/images/recordedCourse.jpeg",
            "live-course" => "assets/images/liveCourse/liveCourse.jpeg",
            "webinar" => "assets/images/webinar/webinar.jpeg",
            _ => "assets/images/certification.jpg"
        };
        var title = tabKey switch
        {
            "exam-simulator" => $"Master the {code} Exam",
            "recorded-course" => $"Master {code} With Our Recorded Course",
            "live-course" => $"Master {code} With Live Expert Training",
            "webinar" => $"Free {code} Webinar",
            "quiz-game" => $"Test Your {code} Knowledge",
            "faq" => $"{code} Frequently Asked Questions",
            _ => $"What Our {code} Learners Say"
        };
        var titleAr = tabKey switch
        {
            "exam-simulator" => $"أتقن اختبار {code}",
            "recorded-course" => $"أتقن {code} مع الكورس المسجل",
            "live-course" => $"أتقن {code} مع التدريب المباشر",
            "webinar" => $"ندوة {code} مجانية",
            "quiz-game" => $"اختبر معلوماتك في {code}",
            "faq" => $"الأسئلة الشائعة عن {code}",
            _ => $"آراء متدربي {code}"
        };
        return JsonSerializer.Serialize(new
        {
            banner = new
            {
                // Empty copy preserves the existing translated UI until an editor publishes exact content.
                en = new { titlePart1 = "", titlePart2 = "", description = "" },
                ar = new { titlePart1 = "", titlePart2 = "", description = "" },
                mediaUrl = "",
                mediaType = "image"
            },
            sections = Array.Empty<object>()
        });
    }
}

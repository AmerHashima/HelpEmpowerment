using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Translation.V2; // استخدام مكتبة Google الرسمية
using System.Threading.Tasks;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslateController : ControllerBase
    {
        private readonly TranslationClient _client;

        public TranslateController()
        {
            // إنشاء TranslationClient
            _client = TranslationClient.Create();
        }

        [HttpPost("en-to-ar")]
        public IActionResult TranslateEnToAr([FromBody] TranslateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text is required" });

            try
            {
                var response = _client.TranslateText(request.Text, "ar", "en");

                return Ok(new TranslateResponse
                {
                    translatedText = response.TranslatedText,
                    sourceLanguage = response.DetectedSourceLanguage,
                    targetLanguage = "ar",
                    originalText = request.Text
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = $"Translation failed: {ex.Message}" });
            }
        }

        [HttpPost("ar-to-en")]
        public IActionResult TranslateArToEn([FromBody] TranslateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text is required" });

            try
            {
                var response = _client.TranslateText(request.Text, "en", "ar");

                return Ok(new TranslateResponse
                {
                    translatedText = response.TranslatedText,
                    sourceLanguage = response.DetectedSourceLanguage,
                    targetLanguage = "en",
                    originalText = request.Text
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = $"Translation failed: {ex.Message}" });
            }
        }

        [HttpPost("auto")]
        public IActionResult TranslateAuto([FromBody] TranslateAutoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text is required" });

            if (string.IsNullOrWhiteSpace(request.Target))
                return BadRequest(new { error = "Target language is required" });

            try
            {
                var response = _client.TranslateText(request.Text, request.Target);

                return Ok(new TranslateResponse
                {
                    translatedText = response.TranslatedText,
                    sourceLanguage = response.DetectedSourceLanguage,
                    targetLanguage = request.Target,
                    originalText = request.Text
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = $"Translation failed: {ex.Message}" });
            }
        }

        [HttpPost("translate")]
        public IActionResult Translate([FromBody] TranslateFullRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text is required" });

            if (string.IsNullOrWhiteSpace(request.Target))
                return BadRequest(new { error = "Target language is required" });

            try
            {
                var source = string.IsNullOrWhiteSpace(request.Source) ? null : request.Source;
                var response = _client.TranslateText(request.Text, request.Target, source);

                return Ok(new TranslateResponse
                {
                    translatedText = response.TranslatedText,
                    sourceLanguage = response.DetectedSourceLanguage,
                    targetLanguage = request.Target,
                    originalText = request.Text
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = $"Translation failed: {ex.Message}" });
            }
        }

        [HttpGet("languages")]
        public IActionResult GetSupportedLanguages()
        {
            // قائمة مختصرة، يمكن التوسع حسب الحاجة
            var languages = new List<LanguageInfo>
            {
                new() { code = "en", name = "English" },
                new() { code = "ar", name = "Arabic" },
                new() { code = "es", name = "Spanish" },
                new() { code = "fr", name = "French" },
                new() { code = "de", name = "German" },
                new() { code = "zh", name = "Chinese (Simplified)" }
            };

            return Ok(languages);
        }
    }

    // طلبات واستجابات
    public class TranslateRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    public class TranslateAutoRequest
    {
        public string Text { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
    }

    public class TranslateFullRequest
    {
        public string Text { get; set; } = string.Empty;
        public string? Source { get; set; }
        public string Target { get; set; } = string.Empty;
    }

    public class TranslateResponse
    {
        public string translatedText { get; set; } = string.Empty;
        public string originalText { get; set; } = string.Empty;
        public string sourceLanguage { get; set; } = string.Empty;
        public string targetLanguage { get; set; } = string.Empty;
    }

    public class LanguageInfo
    {
        public string code { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
    }
}

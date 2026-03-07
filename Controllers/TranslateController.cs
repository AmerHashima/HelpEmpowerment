using Microsoft.AspNetCore.Mvc;
using GTranslate.Translators;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslateController : ControllerBase
    {
        private readonly AggregateTranslator _translator;

        public TranslateController()
        {
            _translator = new AggregateTranslator();
        }

        [HttpPost("en-to-ar")]
        public async Task<IActionResult> TranslateEnToAr([FromBody] TranslateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text is required" });

            try
            {
                var result = await _translator.TranslateAsync(request.Text, "ar", "en");

                return Ok(new TranslateResponse
                {
                    translatedText = result.Translation,
                    sourceLanguage = "en",
                    targetLanguage = "ar",
                    originalText = request.Text
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Translation failed: {ex.Message}" });
            }
        }

        [HttpPost("ar-to-en")]
        public async Task<IActionResult> TranslateArToEn([FromBody] TranslateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text is required" });

            try
            {
                var result = await _translator.TranslateAsync(request.Text, "en", "ar");

                return Ok(new TranslateResponse
                {
                    translatedText = result.Translation,
                    sourceLanguage = "ar",
                    targetLanguage = "en",
                    originalText = request.Text
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Translation failed: {ex.Message}" });
            }
        }

        [HttpPost("auto")]
        public async Task<IActionResult> TranslateAuto([FromBody] TranslateAutoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text is required" });

            if (string.IsNullOrWhiteSpace(request.Target))
                return BadRequest(new { error = "Target language is required" });

            try
            {
                var result = await _translator.TranslateAsync(request.Text, request.Target);

                return Ok(new TranslateResponse
                {
                    translatedText = result.Translation,
                    sourceLanguage = result.SourceLanguage.ISO6391,
                    targetLanguage = request.Target,
                    originalText = request.Text
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Translation failed: {ex.Message}" });
            }
        }

        [HttpPost("translate")]
        public async Task<IActionResult> Translate([FromBody] TranslateFullRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text is required" });

            if (string.IsNullOrWhiteSpace(request.Target))
                return BadRequest(new { error = "Target language is required" });

            try
            {
                var source = string.IsNullOrWhiteSpace(request.Source) ? null : request.Source;

                var result = source != null
                    ? await _translator.TranslateAsync(request.Text, request.Target, source)
                    : await _translator.TranslateAsync(request.Text, request.Target);

                return Ok(new TranslateResponse
                {
                    translatedText = result.Translation,
                    sourceLanguage = result.SourceLanguage.ISO6391,
                    targetLanguage = request.Target,
                    originalText = request.Text
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Translation failed: {ex.Message}" });
            }
        }

        [HttpGet("languages")]
        public IActionResult GetSupportedLanguages()
        {
            var languages = new List<LanguageInfo>
            {
                new() { code = "en", name = "English" },
                new() { code = "ar", name = "Arabic" },
                new() { code = "es", name = "Spanish" },
                new() { code = "fr", name = "French" },
                new() { code = "de", name = "German" },
                new() { code = "zh", name = "Chinese (Simplified)" },
                new() { code = "tr", name = "Turkish" },
                new() { code = "ja", name = "Japanese" },
                new() { code = "ko", name = "Korean" },
                new() { code = "pt", name = "Portuguese" },
                new() { code = "ru", name = "Russian" },
                new() { code = "hi", name = "Hindi" },
                new() { code = "it", name = "Italian" }
            };

            return Ok(languages);
        }
    }

    // Request/Response DTOs
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

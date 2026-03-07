using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslateController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public TranslateController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("en-to-ar")]
        public async Task<IActionResult> TranslateEnToAr([FromBody] TranslateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return BadRequest(new { error = "Text is required" });

            try
            {
                var translatedText = await TranslateWithMyMemory(request.Text, "en", "ar");

                return Ok(new TranslateResponse
                {
                    translatedText = translatedText,
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
                var translatedText = await TranslateWithMyMemory(request.Text, "ar", "en");

                return Ok(new TranslateResponse
                {
                    translatedText = translatedText,
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
                var translatedText = await TranslateWithMyMemory(request.Text, null, request.Target);

                return Ok(new TranslateResponse
                {
                    translatedText = translatedText,
                    sourceLanguage = "auto",
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
                var translatedText = await TranslateWithMyMemory(request.Text, source, request.Target);

                return Ok(new TranslateResponse
                {
                    translatedText = translatedText,
                    sourceLanguage = source ?? "auto",
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
                new() { code = "zh", name = "Chinese (Simplified)" }
            };

            return Ok(languages);
        }

        private async Task<string> TranslateWithMyMemory(string text, string? sourceLanguage, string targetLanguage)
        {
            // Try MyMemory first
            try
            {
                var langPair = string.IsNullOrWhiteSpace(sourceLanguage)
                    ? $"en|{targetLanguage}"
                    : $"{sourceLanguage}|{targetLanguage}";

                var encodedText = Uri.EscapeDataString(text);
                var url = $"https://api.mymemory.translated.net/get?q={encodedText}&langpair={langPair}";

                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    // Fallback to LibreTranslate
                    return await TranslateWithLibreTranslate(text, sourceLanguage, targetLanguage);
                }

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<MyMemoryResponse>(json);

                if (result?.ResponseData?.TranslatedText == null)
                    throw new Exception("Translation returned no result");

                return result.ResponseData.TranslatedText;
            }
            catch (HttpRequestException)
            {
                // Fallback to LibreTranslate on any HTTP error
                return await TranslateWithLibreTranslate(text, sourceLanguage, targetLanguage);
            }
        }

        private async Task<string> TranslateWithLibreTranslate(string text, string? sourceLanguage, string targetLanguage)
        {
            var requestBody = new
            {
                q = text,
                source = string.IsNullOrWhiteSpace(sourceLanguage) ? "auto" : sourceLanguage,
                target = targetLanguage,
                format = "text"
            };

            // Try multiple LibreTranslate instances
            var libreTranslateUrls = new[]
            {
                "https://libretranslate.de/translate",
                "https://translate.argosopentech.com/translate",
                "https://translate.terraprint.co/translate"
            };

            foreach (var url in libreTranslateUrls)
            {
                try
                {
                    var jsonContent = new StringContent(
                        JsonSerializer.Serialize(requestBody),
                        System.Text.Encoding.UTF8,
                        "application/json");

                    var response = await _httpClient.PostAsync(url, jsonContent);

                    if (!response.IsSuccessStatusCode)
                        continue;

                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LibreTranslateResponse>(json);

                    if (!string.IsNullOrEmpty(result?.TranslatedText))
                        return result.TranslatedText;
                }
                catch
                {
                    // Try next instance
                    continue;
                }
            }

            throw new Exception("All translation services are currently unavailable. Please try again later.");
        }
    }

    // MyMemory API response models
    public class MyMemoryResponse
    {
        [JsonPropertyName("responseData")]
        public MyMemoryResponseData? ResponseData { get; set; }

        [JsonPropertyName("responseStatus")]
        public int ResponseStatus { get; set; }
    }

    public class MyMemoryResponseData
    {
        [JsonPropertyName("translatedText")]
        public string? TranslatedText { get; set; }

        [JsonPropertyName("match")]
        public double Match { get; set; }
    }

    public class LibreTranslateResponse
    {
        [JsonPropertyName("translatedText")]
        public string? TranslatedText { get; set; }
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

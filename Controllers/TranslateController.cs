using Microsoft.AspNetCore.Mvc;
using HelpEmpowermentApi.Common;
using HelpEmpowermentApi.DTOs;
using HelpEmpowermentApi.IServices;

namespace HelpEmpowermentApi.Controllers
{
    [ApiController]
    [Route("api/translate")]
    public class TranslateController : ControllerBase
    {
        private readonly HttpClient _http;

        public TranslateController(HttpClient http)
        {
            _http = http;
        }

        [HttpPost("en-to-ar")]
        public async Task<IActionResult> Translate([FromBody] TranslateRequest request)
        {
            var response = await _http.PostAsJsonAsync(
                "https://libretranslate.de/translate",
                new
                {
                    q = request.Text,
                    source = "en",
                    target = "ar",
                    format = "text"
                });

            var result = await response.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }
    }

    public class TranslateRequest
    {
        public string Text { get; set; }
    }
}
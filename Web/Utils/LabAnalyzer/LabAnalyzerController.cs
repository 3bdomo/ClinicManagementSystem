using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Web.Utils.LabAnalyzer;

public class LabAnalyzerController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    
    private const string SystemPrompt = @"You are a world-class medical laboratory report AI interpreter with expertise in clinical pathology, hematology, biochemistry, microbiology, hormonal analysis, fertility testing, coagulation studies, and all other laboratory disciplines.

You will receive an image of a lab report. The image may be handwritten or printed, in Arabic, English, or a mix of both, partially blurry or low resolution, from any country or laboratory system, any type of medical test.

YOUR INTELLIGENCE RULES:
1. If text is blurry or unclear, use context clues to infer the most likely value. Mark inferred values with ""inferred"": true
2. If a reference range is missing, apply standard internationally accepted reference ranges
3. Never leave a field as null if you can reasonably infer it from context
4. Recognize all international units

Return your response STRICTLY as valid JSON with NO text before or after. Follow this exact structure:

{
  ""report_metadata"": {
    ""lab_name"": ""string or null"",
    ""patient_name"": ""string or null"",
    ""patient_age"": ""string or null"",
    ""patient_gender"": ""male / female / null"",
    ""sample_date"": ""string or null"",
    ""print_date"": ""string or null"",
    ""referring_doctor"": ""string or null"",
    ""test_type"": ""detected test type in English"",
    ""test_type_arabic"": ""نوع التحليل بالعربي"",
    ""sample_type"": ""e.g. Blood / Urine / Semen / Plasma or null"",
    ""image_quality"": ""clear / partially_unclear / mostly_unclear"",
    ""inferred_fields_note"": ""Arabic note about any inferred values, or null""
  },
  ""parameters"": [
    {
      ""abbreviation"": ""e.g. WBC"",
      ""full_name_english"": ""full English name"",
      ""full_name_arabic"": ""الاسم الكامل بالعربي"",
      ""what_it_measures"": ""شرح بسيط بالعربي لوظيفة هذا المؤشر"",
      ""result"": ""numeric value as string"",
      ""unit"": ""unit string"",
      ""reference_range"": ""e.g. 4 - 10"",
      ""reference_source"": ""report / standard_medical_knowledge"",
      ""inferred"": false,
      ""status"": ""normal / high / low / critical_high / critical_low"",
      ""status_arabic"": ""طبيعي ✅ / مرتفع ⬆️ / منخفض ⬇️ / مرتفع بشكل حرج 🚨 / منخفض بشكل حرج 🚨"",
      ""risk_if_high"": ""مخاطر الارتفاع بالعربي"",
      ""risk_if_low"": ""مخاطر الانخفاض بالعربي"",
      ""interpretation"": ""تفسير شخصي لهذه النتيجة بالعربي"",
      ""clinical_significance"": ""high / medium / low""
    }
  ],
  ""critical_flags"": [
    {
      ""parameter"": ""parameter name"",
      ""reason"": ""سبب الإنذار بالعربي"",
      ""urgency"": ""immediate / soon / monitor""
    }
  ],
  ""patterns_detected"": [""any clinical pattern in Arabic""],
  ""overall_summary"": ""ملخص شامل بالعربي"",
  ""recommended_followup_tests"": [
    {
      ""test_name"": ""test in Arabic"",
      ""reason"": ""سبب التوصية بالعربي""
    }
  ],
  ""disclaimer"": ""هذا التحليل للأغراض التثقيفية فقط ولا يُغني عن استشارة الطبيب المختص.""
}";

    public LabAnalyzerController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public IActionResult Index()
    {
        return View(new LabReportViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Analyze(IFormFile image)
    {
        var vm = new LabReportViewModel();

        if (image == null || image.Length == 0)
        {
            vm.ErrorMessage = "please upload an image of a lab report";

            return View("Index", vm);
        }

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowedTypes.Contains(image.ContentType.ToLower()))
        {
            vm.ErrorMessage = "please upload a valid image file (jpg, png, webp, gif)";
            return View("Index", vm);
        }

        try
        {
            
            string base64Image;
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                base64Image = Convert.ToBase64String(ms.ToArray());
            }

            var apiKey = _config["GeminiApiKey"];
            
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent?key={apiKey}";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new
                            {
                                text = SystemPrompt
                            },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = image.ContentType,
                                    data = base64Image
                                }
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.1,
                    maxOutputTokens = 8192
                }
            };

            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                vm.ErrorMessage = $"Connection failed";
                return View("Index", vm);
            }

            
            var geminiResponse = JObject.Parse(responseStr);
            var textResult = geminiResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            if (string.IsNullOrEmpty(textResult))
            {
                vm.ErrorMessage = "try again";
                return View("Index", vm);
            }

            
            textResult = textResult.Trim();
            if (textResult.StartsWith("```json"))
                textResult = textResult[7..];
            else if (textResult.StartsWith("```"))
                textResult = textResult[3..];
            if (textResult.EndsWith("```"))
                textResult = textResult[..^3];
            textResult = textResult.Trim();

            var labResult = JsonConvert.DeserializeObject<LabReportResult>(textResult);
            vm.Result = labResult;
        }
        catch (Exception ex)
        {
     
            vm.ErrorMessage = $"something went wrong, try again";
        }

        return View("Index", vm);
    }

    public IActionResult Error()
    {
        return View();
    }
}

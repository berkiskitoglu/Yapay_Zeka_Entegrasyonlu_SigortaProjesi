using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;

namespace InsureYouAI.Controllers
{
    public class PolicyAnalysisWithAIController : Controller
    {
        private readonly IConfiguration _configuration;

        public PolicyAnalysisWithAIController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult PdfAnalyze()
        {
            ViewBag.ControllerName = "OpenAI";
            ViewBag.PageName = "OpenAI ile Pdf Analizi";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PdfAnalyze(IFormFile pdfFile)
        {
            ViewBag.ControllerName = "OpenAI";
            ViewBag.PageName = "OpenAI ile Pdf Analizi";

            if (pdfFile == null || pdfFile.Length == 0)
            {
                ViewBag.Error = "Lütfen bir PDF poliçe dosyası yükleyiniz.";
                return View();
            }

            // 1) PDF METNİ ÇIKAR
            string extractedText = await ExtractTextFromPdf(pdfFile);

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                ViewBag.Error = "PDF içerisinden metin çıkarılamadı.";
                return View();
            }

            // 2) OPENAI'YA ANALİZ ETTİR
            string analysis = await AnalyzePolicyWithOpenAI(extractedText);

            ViewBag.OriginalText = extractedText;
            ViewBag.AnalysisResult = analysis;

            return View();
        }

        private async Task<string> ExtractTextFromPdf(IFormFile pdfFile)
        {
            using var ms = new MemoryStream();
            await pdfFile.CopyToAsync(ms);
            ms.Position = 0;

            var sb = new StringBuilder();

            using (var document = PdfDocument.Open(ms))
            {
                foreach (var page in document.GetPages())
                {
                    sb.AppendLine(page.Text);
                    sb.AppendLine("\n");
                }
            }
            return sb.ToString();
        }

        private async Task<string> AnalyzePolicyWithOpenAI(string policyText)
        {
            var apiUrl = "https://api.openai.com/v1/chat/completions";

            var prompt = $@"
Aşağıdaki metin bir sigorta poliçesine aittir.

Görevlerin:
1) Poliçeyi 10 maddede özetle.
2) Neleri kapsar? (Madde madde yaz)
3) Neleri kapsamaz? (Madde madde yaz)
4) Müşteri için kritik uyarıları **kalın** yap.
5) Yanıtı markdown formatında üret.

--- POLİÇE METNİ ---
{policyText}
--- SON ---
";

            var body = new
            {
                model = "gpt-4o",
                max_tokens = 4096,
                temperature = 0.2,
                messages = new[]
                {
                    new { role = "system", content = "Sen bir sigorta uzmanısın. Poliçeleri analiz ederek müşterilere anlaşılır özetler sunuyorsun." },
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(body);

            using var http = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Headers.Add("Authorization", $"Bearer {_configuration["OpenAIApiKey"]}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await http.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"AI Analizi sırasında hata oluştu: {response.StatusCode}\n{responseText}";
            }

            using var doc = JsonDocument.Parse(responseText);
            var root = doc.RootElement;

            var content = root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content ?? "Yanıt alınamadı.";
        }
    }
}
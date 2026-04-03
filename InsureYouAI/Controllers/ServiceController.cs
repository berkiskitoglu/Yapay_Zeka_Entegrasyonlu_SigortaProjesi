using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly InsureContext _context;
        private readonly IConfiguration _configuration;

        public ServiceController(InsureContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult ServiceList()
        {
            ViewBag.ControllerName = "Hizmelet";
            ViewBag.PageName = "Mevcut Sigorta Hizmetleri Listesi";
            var serviceList = _context.Services.ToList();
            return View(serviceList);
        }
        [HttpGet]
        public IActionResult CreateService()
        {
            ViewBag.ControllerName = "Hizmetler";
            ViewBag.PageName = "Yeni Hizmet Yazısı Girişi";
            return View();
        }
        [HttpPost]
        public IActionResult CreateService(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }
        [HttpGet]
        public IActionResult UpdateService(int id)
        {
            ViewBag.ControllerName = "Hizmetler";
            ViewBag.PageName = "Hizmet Yazısı Güncelleme Sayfası";
            var value = _context.Services.Find(id);
            return View(value);
        }
        [HttpPost]
        public IActionResult UpdateService(Service service)
        {
            _context.Services.Update(service);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }
        [HttpGet]
        public IActionResult DeleteService(int id)
        {
            var value = _context.Services.Find(id);
            _context.Services.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ServiceList");
        }

        [HttpGet]
        public async Task<IActionResult> CreateServiceItemWithGrok()
        {
            var apiKey = _configuration["GrokApiKey"];
            var url = "https://api.groq.com/openai/v1/chat/completions";

            var requestData = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Sen bir sigorta uzmanısın. Türkçe, profesyonel ve etkileyici içerikler yazıyorsun." },
                    new { role = "user", content = "Kurumsal bir sigorta firması için sunulan 5 farklı hizmeti madde halinde yaz. Her madde kısa, etkileyici ve profesyonel olsun. Sadece maddeleri yaz, başka açıklama ekleme." }              
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var response = await httpClient.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseJson);
            var services = jsonDoc.RootElement
                                   .GetProperty("choices")[0]
                                   .GetProperty("message")
                                   .GetProperty("content")
                                   .GetString();
            ViewBag.value = services?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                           .Where(x => x.Trim().Length > 0)
                           .ToList(); 
            return View();

        }
    }
}

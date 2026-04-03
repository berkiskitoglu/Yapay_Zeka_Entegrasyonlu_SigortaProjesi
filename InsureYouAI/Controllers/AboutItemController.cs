using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class AboutItemController : Controller
    {
        private readonly InsureContext _context;
        private readonly IConfiguration _configuration;

        public AboutItemController(InsureContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult AboutItemList()
        {
            ViewBag.ControllerName = "Hakkımızda Ögeleri";
            ViewBag.PageName = "Mevcut Hakkımızda Ögeleri";
            var aboutItemList = _context.AboutItems.ToList();
            return View(aboutItemList);
        }

        [HttpGet]
        public IActionResult CreateAboutItem()
        {
            ViewBag.ControllerName = "Hakkımızda Ögeleri";
            ViewBag.PageName = "Yeni Hakkımızda Öge Girişi";
            return View();
        }

        [HttpPost]
        public IActionResult CreateAboutItem(AboutItem aboutItem)
        {
            _context.AboutItems.Add(aboutItem);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }

        [HttpGet]
        public IActionResult UpdateAboutItem(int id)
        {
            ViewBag.ControllerName = "Hakkımızda";
            ViewBag.PageName = "Hakkımızda Ögeleri Güncelleme Sayfası";
            var value = _context.AboutItems.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateAboutItem(AboutItem aboutItem)
        {
            _context.AboutItems.Update(aboutItem);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }

        [HttpGet]
        public IActionResult DeleteAboutItem(int id)
        {
            var value = _context.AboutItems.Find(id);
            _context.AboutItems.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("AboutItemList");
        }

        [HttpGet]
        public IActionResult CreateAboutItemWithGrok()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAboutItemWithGrok(AboutItem aboutItem)
        {
            var apiKey = _configuration["GrokApiKey"];
            var url = "https://api.groq.com/openai/v1/chat/completions";
            var requestData = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "system", content = "Sen bir sigorta uzmanısın. Türkçe, profesyonel ve etkileyici içerikler yazıyorsun." },
                    new { role = "user", content = "Kurumsal bir sigorta firması için Hakkımızda sayfasında kullanılacak 1 adet minumum 50 karakter maksimum 85 karakter içeren , etkileyici ve profesyonel madde oluştur. Sadece maddeyi yaz, başka açıklama ekleme." }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await httpClient.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseJson);
            var aboutItemText = jsonDoc.RootElement
                                       .GetProperty("choices")[0]
                                       .GetProperty("message")
                                       .GetProperty("content")
                                       .GetString();

            _context.AboutItems.Add(new AboutItem
            {
                Detail = aboutItemText!.Trim()
            });
            _context.SaveChanges();

            return RedirectToAction("AboutItemList");
        }
    }
}
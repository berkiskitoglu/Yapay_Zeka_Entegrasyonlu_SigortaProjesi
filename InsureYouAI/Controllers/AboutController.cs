using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class AboutController : Controller
    {
        private readonly InsureContext _context;
        private readonly IConfiguration _configuration;

        public AboutController(InsureContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult AboutList()
        {
            var aboutList = _context.Abouts.ToList();
            return View(aboutList);
        }

        [HttpGet]
        public IActionResult CreateAbout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAbout(About about)
        {
            _context.Abouts.Add(about);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }

        [HttpGet]
        public IActionResult UpdateAbout(int id)
        {
            var value = _context.Abouts.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateAbout(About about)
        {
            _context.Abouts.Update(about);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }

        [HttpGet]
        public IActionResult DeleteAbout(int id)
        {
            var value = _context.Abouts.Find(id);
            _context.Abouts.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("AboutList");
        }

        [HttpGet]
        public IActionResult CreateAboutWithGrok()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAboutWithGrok(About about)
        {
            var apiKey = _configuration["GrokApiKey"];
            var url = "https://api.groq.com/openai/v1/chat/completions";
            var requestData = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
            new { role = "system", content = "Sen bir sigorta uzmanısın. Türkçe, profesyonel ve etkileyici içerikler yazıyorsun." },
            new { role = "user", content = "Kurumsal bir sigorta firması için etkileyici, güven verici ve profesyonel bir Hakkımızda yazısı oluştur. Bu yazı en az 1500 karakterden oluşsun." }
        }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await httpClient.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseJson);
            var aboutText = jsonDoc.RootElement
                                   .GetProperty("choices")[0]
                                   .GetProperty("message")
                                   .GetProperty("content")
                                   .GetString();

            _context.Abouts.Add(new About
            {
                Title = "Hakkımızda",
                Description = aboutText,
                ImageUrl = "default.jpg"
            });
            _context.SaveChanges();

            return RedirectToAction("AboutList");
        }
    }
}
using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class TestimonialController : Controller
    {
        private readonly InsureContext _context;
        private readonly IConfiguration _configuration;

        public TestimonialController(InsureContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult TestimonialList()
        {
            ViewBag.ControllerName = "Referanslar";
            ViewBag.PageName = "Referanslar Tarafından Oluşuturulan Yazılar";
            var testimonialList = _context.Testimonials.ToList();
            return View(testimonialList);
        }

        [HttpGet]
        public IActionResult CreateTestimonial()
        {
            ViewBag.ControllerName = "Referanslar";
            ViewBag.PageName = "Yeni Referans Yazısı";
            return View();
        }

        [HttpPost]
        public IActionResult CreateTestimonial(Testimonial testimonial)
        {
            _context.Testimonials.Add(testimonial);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }

        [HttpGet]
        public IActionResult UpdateTestimonial(int id)
        {
            ViewBag.ControllerName = "Referanslar";
            ViewBag.PageName = "Referans Yazısı Güncelleme Sayfası";
            var value = _context.Testimonials.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateTestimonial(Testimonial testimonial)
        {
            _context.Testimonials.Update(testimonial);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }

        [HttpGet]
        public IActionResult DeleteTestimonial(int id)
        {
            var value = _context.Testimonials.Find(id);
            _context.Testimonials.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("TestimonialList");
        }

        [HttpGet]
        public IActionResult CreateTestimonialWithGrok()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTestimonialWithGrok(Testimonial testimonial)
        {
            var apiKey = _configuration["GrokApiKey"];
            var url = "https://api.groq.com/openai/v1/chat/completions";
            var requestData = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
            new { role = "system", content = "Sen bir sigorta uzmanısın. Türkçe, profesyonel ve etkileyici içerikler yazıyorsun." },
            new { role = "user", content = "Kurumsal bir sigorta firması için 1 adet müşteri yorumu (testimonial) oluştur. Her yorum için: müşteri adı soyadı, unvanı ve kısa bir yorum olsun. Format şu şekilde olsun: Ad Soyad | Unvan | Yorum. Sadece bu formatı kullan, başka açıklama ekleme. Tüm içerik %100 Türkçe olsun, kesinlikle İngilizce kelime kullanma." }
        }
            };
            

            var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await httpClient.PostAsync(url, content);
            var responseJson = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseJson);
            var testimonialText = jsonDoc.RootElement
                                       .GetProperty("choices")[0]
                                       .GetProperty("message")
                                       .GetProperty("content")
                                       .GetString();

            var lines = testimonialText?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                       .Where(x => x.Trim().Length > 0 && x.Contains('|'))
                                       .ToList();

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length >= 3)
                {
                    _context.Testimonials.Add(new Testimonial
                    {
                        NameSurname = parts[0].Trim(),
                        Title = parts[1].Trim(),
                        CommentDetail = parts[2].Trim(),
                        ImageUrl = "default.jpg"
                    });
                }
            }
            _context.SaveChanges();

            return RedirectToAction("TestimonialList");
        }
    }
}
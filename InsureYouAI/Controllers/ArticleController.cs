using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class ArticleController : Controller
    {
        private readonly InsureContext _context;
        private readonly IConfiguration _configuration;

        public ArticleController(InsureContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult ArticleList()
        {
            ViewBag.ControllerName = "Makaleler";
            ViewBag.PageName = "Makale Listesi";
            var values = _context.Articles.Include(x => x.AppUser).Include(y => y.Category).ToList();
            return View(values);
        }

        [HttpGet]
        public IActionResult CreateArticle()
        {
            ViewBag.ControllerName = "Makaleler";
            ViewBag.PageName = "Yeni Makale Oluştur";

            var categories = _context.Categories
                           .Select(x => new SelectListItem
                           {
                               Text = x.CategoryName,
                               Value = x.CategoryId.ToString()
                           })
                           .ToList();

            ViewBag.Categories = categories;

            var authors = _context.Users
                         .Select(x => new SelectListItem
                         {
                             Text = x.Name + " " + x.Surname,
                             Value = x.Id
                         })
                         .ToList();

            ViewBag.Authors = authors;

            return View();
        }

        [HttpPost]
        public IActionResult CreateArticle(Article article)
        {
            article.CreatedDate = DateTime.Now;
            _context.Articles.Add(article);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }

        [HttpGet]
        public IActionResult UpdateArticle(int id)
        {

            ViewBag.ControllerName = "Makaleler";
            ViewBag.PageName = "Makale Güncelleme Sayfası";

            var categories = _context.Categories
                           .Select(x => new SelectListItem
                           {
                               Text = x.CategoryName,
                               Value = x.CategoryId.ToString()
                           })
                           .ToList();

            ViewBag.Categories = categories;

            var authors = _context.Users
                         .Select(x => new SelectListItem
                         {
                             Text = x.Name + " " + x.Surname,
                             Value = x.Id
                         })
                         .ToList();

            ViewBag.Authors = authors;

            var value = _context.Articles.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateArticle(Article article)
        {
            _context.Articles.Update(article);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }

        [HttpGet]
        public IActionResult DeleteArticle(int id)
        {
            var value = _context.Articles.Find(id);
            _context.Articles.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ArticleList");
        }

        [HttpGet]
        public IActionResult CreateArticleWithOpenAI()
        {
            ViewBag.categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticleWithOpenAI(string prompt, int categoryId)
        {
            var apiKey = _configuration["OpenAIApiKey"];
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestData = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Sen bir sigorta uzmanısın. Türkçe, SEO uyumlu, profesyonel makaleler yazıyorsun. Kullanıcının verdiği özet ve anahtar kelimelere göre sigortacılık sektörüyle ilgili makale üret. En az 1000 karakter olsun. Format: Başlık | İçerik. Sadece bu formatı kullan, başka açıklama ekleme." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7
            };

            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
                var articleText = result?.choices?[0]?.message?.content;

                var parts = articleText?.Split('|');
                if (parts?.Length >= 2)
                {
                    _context.Articles.Add(new Article
                    {
                        Title = parts[0].Trim(),
                        Content = parts[1].Trim(),
                        CreatedDate = DateTime.Now,
                        CoverImageUrl = "default.jpg",
                        MainCoverImageUrl = "default.jpg",
                        CategoryId = categoryId
                    });
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("ArticleList");
        }

        public class OpenAIResponse
        {
            public List<Choice>? choices { get; set; }
        }

        public class Choice
        {
            public Message? message { get; set; }
        }

        public class Message
        {
            public string? role { get; set; }
            public string? content { get; set; }
        }
    }
}
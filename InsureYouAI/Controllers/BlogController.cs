using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class BlogController : Controller
    {
        private readonly InsureContext _context;
        private readonly IConfiguration _configuration;

        public BlogController(InsureContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult BlogList()
        {
            return View();
        }
        public IActionResult GetBlogByCategory(int id)
        {
            ViewBag.c = id;
            return View();
        }

        public IActionResult BlogDetail(int id)
        {
            ViewBag.i = id;
            return View();
        }

        public PartialViewResult GetBlog()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult GetBlog(string keyword)
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult AddComment()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            comment.AppUserId = "10a44613-592e-49fc-b48e-86757fa82ff4";

            using var client = new HttpClient();
            var apiKey = _configuration["HuggingFaceApiKey"];
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
             
                var translateRequestBody = new { inputs = comment.CommentDetail };
                var translateContent = new StringContent(JsonSerializer.Serialize(translateRequestBody), Encoding.UTF8, "application/json");
                var translateResponse = await client.PostAsync("https://router.huggingface.co/hf-inference/models/Helsinki-NLP/opus-mt-tc-big-tr-en", translateContent);
                var translateResponseString = await translateResponse.Content.ReadAsStringAsync();

                string englishText = comment.CommentDetail;

                if (translateResponseString.TrimStart().StartsWith("["))
                {
                    var translateDoc = JsonDocument.Parse(translateResponseString);
                    englishText = translateDoc.RootElement[0].GetProperty("translation_text").GetString();
                }

                var toxicRequestBody = new { inputs = englishText };
                var toxicContent = new StringContent(JsonSerializer.Serialize(toxicRequestBody), Encoding.UTF8, "application/json");
                var toxicResponse = await client.PostAsync("https://router.huggingface.co/hf-inference/models/unitary/toxic-bert", toxicContent);
                var toxicResponseString = await toxicResponse.Content.ReadAsStringAsync();

                if (toxicResponseString.TrimStart().StartsWith("["))
                {
                    var toxicDoc = JsonDocument.Parse(toxicResponseString);
                    foreach (var item in toxicDoc.RootElement[0].EnumerateArray())
                    {
                        string label = item.GetProperty("label").GetString();
                        double score = item.GetProperty("score").GetDouble();
                        if (label == "toxic" && score > 0.5)
                        {
                            comment.CommentStatus = "Toksik Yorum";
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(comment.CommentStatus))
                    comment.CommentStatus = "Yorum Onaylandı";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                comment.CommentStatus = "Onay Bekliyor";
            }

            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("BlogList");
        }
    }
}

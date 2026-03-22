using InsureYouAI.Context;
using InsureYouAI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace InsureYouAI.Controllers
{
    public class AppUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly InsureContext _context;
        private readonly IConfiguration _configuration;

        public AppUserController(UserManager<AppUser> userManager, InsureContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        public IActionResult UserList()
        {
            var values = _userManager.Users.ToList();
            return View(values);
        }

        public async Task<IActionResult> UserProfileWithAI(string id)
        {

            var values = await _userManager.FindByIdAsync(id);
            ViewBag.name = values?.Name;
            ViewBag.surname = values?.Surname;
            ViewBag.imageUrl = values?.ImageUrl;
            ViewBag.description = values?.Description;
            ViewBag.titleValue = values?.Title;
            ViewBag.city = values?.City;

            // Kullanıcı Bilgilerini Çekelim
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            //Kullanıcıya Ait Makaleler
            var articles = await _context.Articles
                                         .Where(x => x.AppUserId == id)
                                         .Select(y => y.Content)
                                         .ToListAsync();

            if(articles.Count == 0)
            {
                ViewBag.AIResult = "Bu kullanıcıya ait analiz yapılacak makale bulunamadı";
            }

            //Makaleleri tek bir metinde toplayın
            var allArticles = string.Join("\n\n", articles);
            var apiKey = _configuration["OpenAIApiKey"];

            // Promptun Yazılması
            var prompt = $@"
                Siz bir sigorta sektöründe uzman bir içerik analistisin.
                Elinizde bir sigorta şirketinin çalışanın yazdığı tüm makaleler var.
                Bu makaleler üzerinden çalışanın içerik üretim tarzını analiz et.

                Analiz Başlıkları:

                1-) Konu Çeşitliliği Ve Odak Alanları(Sağlık , Hayat , Kasko, Tamamlayıcı, DASK)
                2-) Hedef Kitle Tahmini (Bireysel/Kurumsal , Segment , Persona)
                3-) Dil ve Anlatım Tarzı(Teknik seviyesi , Okunabilirlik , İkna gücü)
                4-) Sigorta Terimlerini Kullanma Ve Doğruluk Düzeyi
                5-) Müşteri İhtiyaçlarına ve Risk Yönetimine Odaklanma
                6-) Pazarlama/Satış vurgusu, CTA netliği
                7-) Geliştirilmesi Gereken Alanlar ve Net Aksiyon Maddeleri

                Makaleler :

                {allArticles}

                Lütfen Çıktıyı Profesyonel Rapor Formatında , Madde Madde Ve En Sonda 5 Maddelik Aksiyon Listesi İle Ver.
                ";

            // OpenAI Chat Completions
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var body = new
            {
                model = "gpt-3.5-turbo",
                messages = new object[]
                {
                    new
                    {
                        role = "system", content = "Sen Sigorta Sektöründe İçerik Analizi Yapan Bir Uzmansın",
                    },
                    new
                    {
                        role = "system" , content = prompt
                    }
                },
                max_tokens = 1000,
                temperature = 0.2
            };

            //Json Dönüşümleri
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var httpResponse = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseText = await httpResponse.Content.ReadAsStringAsync();
            if(!httpResponse.IsSuccessStatusCode)
            {
                ViewBag.AIResult = "Open AI Hatası: " + httpResponse.StatusCode;
                return View(user);
            }

            // Json Yapısından Veri Okuma
            try
            {
                using var document = JsonDocument.Parse(responseText);
                var aiText = document.RootElement
                                     .GetProperty("choices")[0]
                                     .GetProperty("message")
                                     .GetProperty("content")
                                     .GetString();
                ViewBag.AIResult = aiText ?? "Boş Yanıt Döndü";
            }
            catch (Exception)
            {

                ViewBag.AIResult = "OpenAI Yanıtı Beklenen Formatta Değil.";
            }
            return View(user);
        }

        public async Task<IActionResult> UserCommentsProfileWithAI(string id)
        {
            var values = await _userManager.FindByIdAsync(id);
            ViewBag.name = values?.Name;
            ViewBag.surname = values?.Surname;
            ViewBag.imageUrl = values?.ImageUrl;
            ViewBag.description = values?.Description;
            ViewBag.titleValue = values?.Title;
            ViewBag.city = values?.City;

            // Kullanıcı Bilgilerini Çekelim
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            //Kullanıcıya Ait Yorumlar
            var comments = await _context.Comments
                                         .Where(x => x.AppUserId == id)
                                         .Select(y => y.CommentDetail)
                                         .ToListAsync();

            if (comments.Count == 0)
            {
                ViewBag.AIResult = "Bu kullanıcıya ait analiz yapılacak yorum bulunamadı";
            }

            //Yorumları tek bir metinde toplayın
            var allComments = string.Join("\n\n", comments);
            var apiKey = _configuration["OpenAIApiKey"];

            // Promptun Yazılması
            var prompt = $@"
                Sen kullanıcı davranış analizi yapan bir yapay zeka uzmanısın.
                Aşşağıdaki yorumlara göre kullanıcıyı değerlendir.

                Analiz Başlıkları:

                1-) Genel Duygu Durumu (Pozitif/Negatif/Nötr)
                2-) Toksik İçerik Var mı ? (Örnekleriyle)
                3-) İlgi Alanları / Konu Başlıkları
                4-) İletişim Tarzı (Samimi/Resmi/Agresif vb.)
                5-) Geliştirilmesi Gereken İletişim Alanları
                6-) 5 Maddelik Kısa Özet
                

                Yorumlar : {allComments} ";

            // OpenAI Chat Completions
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var body = new
            {
                model = "gpt-3.5-turbo",
                messages = new object[]
                {
                    new
                    {
                        role = "system", content = "Sen Kullanıcı Yorum Analizi Yapan Bir Uzmansın",
                    },
                    new
                    {
                        role = "system" , content = prompt
                    }
                },
                max_tokens = 1000,
                temperature = 0.2
            };

            //Json Dönüşümleri
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var httpResponse = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseText = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                ViewBag.AIResult = "Open AI Hatası: " + httpResponse.StatusCode;
                return View(user);
            }

            // Json Yapısından Veri Okuma
            try
            {
                using var document = JsonDocument.Parse(responseText);
                var aiText = document.RootElement
                                     .GetProperty("choices")[0]
                                     .GetProperty("message")
                                     .GetProperty("content")
                                     .GetString();
                ViewBag.AIResult = aiText ?? "Boş Yanıt Döndü";
            }
            catch (Exception)
            {

                ViewBag.AIResult = "OpenAI Yanıtı Beklenen Formatta Değil.";
            }
            return View(user);
        }
    }
}

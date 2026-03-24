using InsureYouAI.Context;
using InsureYouAI.Entities;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InsureYouAI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly InsureContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClient;
        public DefaultController(InsureContext context, IConfiguration configuration, IHttpClientFactory httpClient)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public PartialViewResult SendMessage()
        {
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            message.SendDate = DateTime.Now;
            message.IsRead = false;
            _context.Messages.Add(message);
            _context.SaveChanges();

            var payload = new
            {
                email = message.Email,
                subject = message.Subject,
                nameSurname = message.NameSurname,
                messageDetail = message.MessageDetail
            };

            var client = _httpClient.CreateClient();

            await client.PostAsJsonAsync("http://localhost:5678/webhook-test/send-message", payload);

            #region CloudAI
            //string prompt = @$"Sen InsureYouAI Sigorta'nın dijital müşteri hizmetleri asistanısın.
            //                  Görevin, müşterilerden gelen tüm mesajlara hızlı, doğru ve samimi bir kurumsal dille yanıt vermektir. {message.MessageDetail}";

            //using var client = new HttpClient();
            //client.BaseAddress = new Uri("https://api.anthropic.com/");
            //client.DefaultRequestHeaders.Add("x-api-key", _configuration["AnthropicKey"]);
            //client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //var requestBody = new
            //{
            //    model = "claude-sonnet-4-20250514",
            //    max_tokens = 1000,
            //    temperature = 0.6,
            //    messages = new[]
            //    {
            //        new
            //        {
            //            role = "user",
            //            content = prompt
            //        }
            //    }
            //};

            //var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            //var response = await client.PostAsync("v1/messages", jsonContent);
            //var responseString = await response.Content.ReadAsStringAsync();
            //var json = JsonNode.Parse(responseString);
            //string? textContent = json?["content"]?[0]?["text"]?.ToString();
            //ViewBag.v = textContent;
            //#endregion
            //#region Send_Mail
            //MimeMessage mimeMessage = new MimeMessage();
            //MailboxAddress mailboxAddress = new MailboxAddress("InsureYouAI Admin", "iskitoglu.halitberk@gmail.com");
            //mimeMessage.From.Add(mailboxAddress);
            //MailboxAddress mailboxAddressTo = new MailboxAddress("User", message.Email);
            //mimeMessage.To.Add(mailboxAddressTo);

            //var bodyBuilder = new BodyBuilder();
            //bodyBuilder.TextBody = "Mail İçeriği Cloude dan alınacak";
            //mimeMessage.Body = bodyBuilder.ToMessageBody();
            //mimeMessage.Subject = textContent;

            //SmtpClient smptpClient = new SmtpClient();
            //smptpClient.Connect("smtp.gmail.com", 587, false);
            //smptpClient.Authenticate("iskitoglu.halitberk@gmail.com", _configuration["GoogleApplicationKey"]);
            //smptpClient.Send(mimeMessage);
            //smptpClient.Disconnect(true);
            //#endregion
            //#region CloudeAI_DbRegister
            //ClaudeAIMessage claudeAIMessage = new ClaudeAIMessage()
            //{
            //    MessageDetail = textContent,
            //    ReceiverEmail = message.Email,
            //    ReceiverNameSurname = message.NameSurname,
            //    SendDate = DateTime.Now
            //};
            //_context.ClaudeAIMessages.Add(claudeAIMessage);
            //_context.SaveChanges();
            #endregion

            return RedirectToAction("Index");
        }

        public PartialViewResult SubscribeEmail()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult SubscribeEmail(string email)
        {
            return View();
        }
    }
}

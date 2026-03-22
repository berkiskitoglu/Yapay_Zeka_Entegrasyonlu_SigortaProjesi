using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InsureYouAI.Models
{
    public class ChatHub : Hub
    {
        private readonly IHttpClientFactory _client;
        private readonly string _apiKey;
        private readonly string _model;

        public ChatHub(IConfiguration configuration, IHttpClientFactory client)
        {
            _apiKey = configuration["OpenAIApiKey"] ?? throw new InvalidOperationException("Api Key Bulunamadı");
            _model = "gpt-3.5-turbo";
            _client = client;
        }

        private static readonly Dictionary<string, List<Dictionary<string, string>>> _history = new();


        public override Task OnConnectedAsync()
        {
            _history[Context.ConnectionId] = 
              [
                 new()
                  {

                     ["role"] = "system",
                     ["content"] = "You are a helpful assistant. Keep Answers Concise."

                  }
              ];

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _history.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);

        }

        public async Task SendMessage(string userMessage)
        {
            await Clients.Caller.SendAsync("ReceiveUserEcho", userMessage);

            var history = _history[Context.ConnectionId];
            history.Add(new()
            {
                ["role"] = "user",
                ["content"] = userMessage
            });

            await StreamOpenAI(history, Context.ConnectionAborted);
        }

        private async Task StreamOpenAI(List<Dictionary<string,string>>history,CancellationToken cancellationToken)
        {
            var client = _client.CreateClient("openai");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            var payload = new
            {
                model = _model,
                messages = history,
                stream = true,
                temperature = 0.2
            };
            using var request = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions");
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);
            var stringBuilder = new StringBuilder();
            while(!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!line.StartsWith("data:")) continue;
                var data = line["data:".Length..].Trim();
                if (data == "[DONE]") break;
                try
                {
                    var chunk = System.Text.Json.JsonSerializer.Deserialize<ChatStreamChunk>(data);
                    var delta = chunk?.Choices?.FirstOrDefault()?.Delta?.Content;
                    if(!string.IsNullOrEmpty(delta))
                    {
                        stringBuilder.Append(delta);
                        await Clients.Caller.SendAsync("ReceiveToken", delta, cancellationToken);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            var full = stringBuilder.ToString();
            history.Add(new()
            {
                ["role"] = "assistant",
                ["content"] = full
            });

            await Clients.Caller.SendAsync("CompleteMessage", full, cancellationToken);
        }

        private sealed class ChatStreamChunk
        {
            [JsonPropertyName("choices")] 
            public List<Choice>? Choices { get; set; }
        }

        private sealed class Choice
        {
            [JsonPropertyName("delta")]
            public Delta? Delta { get; set; }

            [JsonPropertyName("finish_reason")]
            public string? FinishReason{ get; set; }
        }

        private sealed class Delta
        {
            [JsonPropertyName("content")]
            public string? Content { get; set; }

            [JsonPropertyName("role")]
            public string? Role { get; set; }
        }
    }
}

using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebApplication1.Controllers
{
    public class ChatController : Controller
    {
        private const string Azure_OpenAi_Endpoint = "https://ai-instance-uksouth.openai.azure.com/";
        private const string Azure_OpenAi_Key = "";
        private const string Azure_OpenAi_Model = "";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetResponse(string value)
        {
            OpenAIClient client = new OpenAIClient(new Uri(Azure_OpenAi_Endpoint), new AzureKeyCredential(Azure_OpenAi_Key));

            var chatCompletionOptions = new ChatCompletionsOptions()
            {
                Messages = {
                    new ChatRequestSystemMessage("You are a helpful assistant"),
                    new ChatRequestUserMessage("Does Azure OpenAI support GPT-4?"),
                    new ChatRequestAssistantMessage("Yes it does"),
                    new ChatRequestUserMessage(value)
                },
                MaxTokens = 400 // Higher quicker and more complete answers (not truncated)
            };

            Response<ChatCompletions> azureAIResponse = await client.GetChatCompletionsAsync(chatCompletionOptions);

            var stringBuilder = new StringBuilder();
            int counter = 1;
            foreach (var choice in azureAIResponse.Value.Choices)
            {
                stringBuilder.AppendLine($"{counter} {choice.Message.Content}");
            }

            return Json(new { Response = stringBuilder.ToString() });
        }
    }
}

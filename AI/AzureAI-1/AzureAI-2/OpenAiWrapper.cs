using Azure;
using Azure.AI.OpenAI;

namespace AzureAI_2
{
    internal class OpenAiWrapper
    {
        internal OpenAiWrapper(string endpoint, string key, string model, string searchEndpoint, string searchApiKey)
        {
            Endpoint = endpoint;
            Key = key;
            Model = model;
            Client = new OpenAIClient(new Uri(Endpoint), new AzureKeyCredential(Key));
            searchConfiguration = new AzureSearchChatExtensionConfiguration()
            {
                SearchEndpoint = new Uri(searchEndpoint),
                Authentication = new OnYourDataApiKeyAuthenticationOptions(searchApiKey),
                IndexName = "help",
            };
        }

        public string Endpoint { get; }
        public string Key { get; }
        public string Model { get; }

        private AzureSearchChatExtensionConfiguration searchConfiguration { get; }

        public OpenAIClient Client { get; }

        internal ChatCompletionsOptions Create(string systemPrompt, string usersQuestion)
        {
            var chatCompletionOptions = new ChatCompletionsOptions()
            {
                DeploymentName = Model,
                Messages = {
                    new ChatRequestSystemMessage(systemPrompt),
                    new ChatRequestUserMessage(usersQuestion)
                },
                Temperature = 0.5F, // 0-Same response everytime, 1-very creative responses
                MaxTokens = 400, // Higher quicker and more complete answers (not truncated)
                AzureExtensionsOptions = new AzureChatExtensionsOptions()
                {
                    Extensions = { searchConfiguration },
                },
            };

            return chatCompletionOptions;
        }

        internal async Task<string> GetResponseAsync(ChatCompletionsOptions chatCompletionsOptions)
        {
            var response = await Client.GetChatCompletionsAsync(chatCompletionsOptions);
            var message = response.Value.Choices[0].Message;
            return message.Content;
        }
    }
}

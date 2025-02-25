namespace AzureAI_2
{
    // Goto Azure Open AI - ai-instance-uksouth - this is used for the whole company
    // Goto Resource Management
    //      Keys and Endpoints is what ends up here.
    //      Model Deployments. This is managed in Azure OpenAI Studio
    //          We're using "gpt-35-turbo-16k". So open this in the playground
    //              In the "Setup" area.
    //                  Enter the prompt in the "System Message" part of the Azure OpenAI Studio playground
    //                  Add You Data - Add Data Source, and chooose "URL/web address (preview)"-> This actually will download the content of pages and store them in blob storage
    //                      WebSite Url: = "https://hlm4.highlighter.net/help/"
    //                      Index Name = "help" (in Azure Search Service = hlsearchserviceopenai)
    //                      StorageAccount = martingaytest1 (where the html documents are stored. One time / snapshot copy from the "WebSite Url")
    //                      Azure Search Service = hlsearchserviceopenai
    //                  
    internal class Program
    {
        private static readonly string Azure_OpenAi_Endpoint = "https://ai-instance-uksouth.openai.azure.com/";
        private static readonly string Azure_OpenAi_Key = "";
        private static readonly string Azure_OpenAi_Model = "gpt-35-turbo-16k";
        private static readonly string Azure_Search_Endpoint = "https://hlsearchserviceopenai.search.windows.net";
        private static readonly string Azure_Search_Key = "";

        public static async Task Main(string[] args)
        {
            // System Message part of the Azure OpenAI Studio playground
            const string systemPrompt = "You are a customer support bot whose primary goal is to help users with questions they may ask about a web based, network monitoring tool called Highlight. You are friendly and concise and do not provide answers that are not in the sources. If there isn't enough information from the source, say you don't know. If asking a clarifying question to the user would help, ask the question";
            const string userQuestion = "What sort of watches can I create in Highlight";
            var openAiWrapper = new OpenAiWrapper(Azure_OpenAi_Endpoint, Azure_OpenAi_Key, Azure_OpenAi_Model, Azure_Search_Endpoint, Azure_Search_Key);
            var chatCompletionOptions = openAiWrapper.Create(systemPrompt, userQuestion);
            var responseMessage = await openAiWrapper.GetResponseAsync(chatCompletionOptions);
            Console.WriteLine(responseMessage);
        }
    }
}

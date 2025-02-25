// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;

try
{
    var subscription = args[0];
    Console.WriteLine($"Topic1 - consuming {subscription}");
    var serviceBusClient = new ServiceBusClient(primaryConnectionString);

    var processorTopic1Consumer1 = serviceBusClient.CreateProcessor("topic1", subscription);
    processorTopic1Consumer1.ProcessMessageAsync += HandleEvent;
    processorTopic1Consumer1.ProcessErrorAsync += ProcessorTopic1Consumer1_ProcessErrorAsync;
    await processorTopic1Consumer1.StartProcessingAsync();

    Console.ReadKey();

    await processorTopic1Consumer1.StopProcessingAsync();
    await serviceBusClient.DisposeAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

async Task ProcessorTopic1Consumer1_ProcessErrorAsync(ProcessErrorEventArgs arg)
{
    var messageBody = arg.Exception.Message;
    Console.WriteLine("PROCESS ERROR ==========");
    Console.WriteLine(messageBody);
    Console.WriteLine("========== PROCESS ERROR");
}

async Task HandleEvent(ProcessMessageEventArgs arg)
{
    var messageBody = arg.Message.Body.ToString();
    Console.WriteLine("PROCESS ==========");
    Console.WriteLine(messageBody);
    Console.WriteLine("========== PROCESS");

}
// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;

Console.WriteLine("Topic1 Send Messages");

// https://mg345test.servicebus.windows.net/topic1

try
{
    var serviceBusClient = new ServiceBusClient(primaryConnectionString);
    var sender = serviceBusClient.CreateSender("topic1");
    var serviceBusMessage = new ServiceBusMessage($"some content: {DateTime.Now}");
    await sender.SendMessageAsync(serviceBusMessage);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

while (true)
{
    Console.WriteLine("Enter the subscription on topic1 to consume");
    var subscription = Console.ReadLine();
    if (string.IsNullOrEmpty(subscription))
    {
        continue;
    }
    SpawnConsumerProcess(subscription);
}

void SpawnConsumerProcess(string subscription)
{
    var process = new Process();
    var cmdLine = $"C:\\git\\TopicProducer\\Topic1Consumer\\bin\\Debug\\net8.0\\Topic1Consumer.exe";
    process.StartInfo = new ProcessStartInfo(cmdLine, subscription);
    process.Start();
}
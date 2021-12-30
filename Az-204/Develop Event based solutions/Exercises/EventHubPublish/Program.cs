using System;
using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace EventHubPublish
{
    class Program
    {
        static Random rand = new Random();
        private const string connectionString = "Endpoint=sb://wired-brain-eh-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Pg7V0INuvcPqA4maWfaNG/Pj4PL7PrUrZXpGNCqnJ+Y=";
        private const string eventHubName = "wired-brain-eh-hub";

        static async Task Main(string[] args)
        {
            await SendToSamePartition();
        }

        static async Task SendToRandomPartition()
        {
            await using var producerClient = 
                new EventHubProducerClient(connectionString, eventHubName);

            using EventDataBatch eventDataBatch = 
                await producerClient.CreateBatchAsync();

            for (int i = 0; i < 100; i++)
            {
                double waterTemp = (rand.NextDouble()) * 100;
                int coffeTypeIndex = rand.Next(2);

                var coffeMachineData = new CoffeeData()
                {
                    WaterTemperature = waterTemp,
                    ReadingTime = DateTime.Now,
                    CoffeeType = CoffeeData.AllCoffeeTypes[coffeTypeIndex]
                };

                var coffeMachineDataBytes = 
                    JsonSerializer.SerializeToUtf8Bytes(coffeMachineData);

                var eventData = new EventData(coffeMachineDataBytes);

                if (!eventDataBatch.TryAdd(eventData))
                {
                    throw new Exception("Cannot add coffee machine data to random batch");
                }
            }

            await producerClient.SendAsync(eventDataBatch);

            Console.WriteLine("Wrote events to random partitions");
        }

        static async Task SendToSamePartition()
        {
            await using var producerClient = 
                new EventHubProducerClient(connectionString, eventHubName);

            List<EventData> eventBatch = new List<EventData>();

            for (int i = 0; i < 100; i++)
            {
                double waterTemp = (rand.NextDouble()) * 100;
                int coffeTypeIndex = rand.Next(2);

                var coffeMachineData = new CoffeeData()
                {
                    WaterTemperature = waterTemp,
                    ReadingTime = DateTime.Now,
                    CoffeeType = CoffeeData.AllCoffeeTypes[coffeTypeIndex]
                };

                var coffeMachineDataBytes = 
                    JsonSerializer.SerializeToUtf8Bytes(coffeMachineData);

                var eventData = new EventData(coffeMachineDataBytes);
                
                eventBatch.Add(eventData);
            }

            var options = new SendEventOptions();
            options.PartitionKey = "device1"; // It doesn't matter what this is as long as it is the same name;

            await producerClient.SendAsync(eventBatch, options);

            Console.WriteLine("Wrote events to single partition");
        }
    }

    class CoffeeData
    {
        public static string[] AllCoffeeTypes = {"a", "b", "c"};
        public double WaterTemperature { get; set; }
        public DateTime ReadingTime { get; set; }
        public string CoffeeType { get; set; }
    }
}
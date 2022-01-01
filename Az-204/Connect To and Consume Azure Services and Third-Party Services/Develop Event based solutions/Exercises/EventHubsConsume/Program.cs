using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using System.Text.Json;

namespace EventHubsConsume
{
    class Program
    {
        private const string connectionString = "Endpoint=sb://wired-brain-eh-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Pg7V0INuvcPqA4maWfaNG/Pj4PL7PrUrZXpGNCqnJ+Y=";
        private const string eventHubName = "wired-brain-eh-hub";

        const string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

        static async Task Main(string[] args)
        {
            // await GetPartitionInfo();
            var readOne = ReadFromPartition("1");
            var readTwo = ReadFromPartition("2");

            await Task.WhenAll(readOne, readTwo);
        }

        static async Task ReadFromPartition(string partitionNumber)
        {
            var cancelationToken = new CancellationTokenSource();
            cancelationToken.CancelAfter(TimeSpan.FromSeconds(120));

            await using( var consumerClient = new EventHubConsumerClient(consumerGroup, connectionString, eventHubName))
            {
                try
                {
                    PartitionProperties props = await consumerClient.GetPartitionPropertiesAsync(partitionNumber);

                    EventPosition startingPosition = 
                        EventPosition.FromSequenceNumber(
                            props.LastEnqueuedSequenceNumber
                            // props.BeginningSequenceNumber
                            );

                    // Read the events comming into the partition until the cancellation token timer runs out and stops it
                    await foreach( PartitionEvent partitionEvent in consumerClient.ReadEventsFromPartitionAsync(partitionNumber, startingPosition, cancelationToken.Token))
                    {
                        Console.WriteLine("**** NEW COFFEE ****")
;
                        string partitionId = partitionEvent.Partition.PartitionId;
                        var sequenceNumber = partitionEvent.Data.SequenceNumber;
                        var key = partitionEvent.Data.PartitionKey;

                        Console.WriteLine($"Paritiotn Id: {partitionId}{Environment.NewLine}" +
                            $"Sequence Number: {sequenceNumber}" +
                            $"Partition Key: {key}");

                        var coffee = JsonSerializer.Deserialize<CoffeeData>(partitionEvent.Data.EventBody);

                        Console.WriteLine($"Type: {coffee.CoffeeType}{Environment.NewLine}" +
                            $"Temp: {coffee.WaterTemperature}{Environment.NewLine}" +
                            $"");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    await consumerClient.CloseAsync();
                }
            }
        }

        static async Task GetPartitionInfo()
        {
            await using var consumerClient =
                new EventHubConsumerClient(consumerGroup, connectionString, eventHubName);

            var partitionIds = await consumerClient.GetPartitionIdsAsync();

            foreach ( var id in partitionIds )
            {
                var partitionInfo = await consumerClient.GetPartitionPropertiesAsync(id);

                Console.WriteLine("**** NEW PARTITION INFO ****");
                Console.WriteLine($"Partition Id: " +
                    $"{partitionInfo.Id}{Environment.NewLine}" +
                    $"Empty? {partitionInfo.IsEmpty}{Environment.NewLine}" +
                    $"Last Sequence: {partitionInfo.LastEnqueuedSequenceNumber}");
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
}
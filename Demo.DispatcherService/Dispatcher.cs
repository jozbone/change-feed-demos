namespace Demo.DispatcherService;

public class Dispatcher
{
    private readonly ILogger _logger;

    public Dispatcher(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Dispatcher>();
    }

    // Other binding examples
    //         StartFromBeginning = true,
    //         FeedPollDelay = 5000,
    //         LeaseContainerPrefix = "myprefix"

    // [ExponentialBackoffRetry(5, "00:00:04", "00:15:00")]

    [Function("Dispatcher")]
    [FixedDelayRetry(2, "00:00:10")]
    public async Task Run([CosmosDBTrigger(
            databaseName: "Demo",
            containerName: "Orders",
            Connection = "CosmosDBConnectionString",
            LeaseContainerName = "leases",
            FeedPollDelay = 5000, //ms
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Order> input
    )
    {
        try
        {
            if (input != null && input.Count > 0) // if there are documents to process
            {
                _logger.LogInformation("Documents modified " + input.Count);
                _logger.LogInformation("First document Id " + input[0].Id);
                _logger.LogInformation($"Retry count: {RetryCounter.CurrentRetryCount}");
                _logger.LogInformation($"Processing Region: { Environment.GetEnvironmentVariable("ProcessingRegion")}");

                // Iterate through the collection of orders that are ready to be processed
                foreach (var o in input)
                {
                    // Simulate an error (for demonstration purposes only)
                    //throw new Exception("Simulated exception");

                    o.ProcessingRegion = Environment.GetEnvironmentVariable("ProcessingRegion");

                    // Sending a message to the Service Bus topic just to demonstrate doing some work. Normally you would move
                    // this code to a separate class for better separation of concerns.

                    // Create a service bus message with the order object
                    var jsonBody = JsonConvert.SerializeObject(o);
                    var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonBody));

                    // Set the message ID and session ID properties to the order ID for convenience for this little demo
                    message.MessageId = o.OrderId.ToString();
                    message.ContentType = "application/json";
                    message.SessionId = message.MessageId;
                    message.ApplicationProperties.Add("MessageType", "OrderCreatedEvent");

                    var fullyQualifiedNamespace = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
                    var client = new ServiceBusClient(fullyQualifiedNamespace);
                    var sender = client.CreateSender(Environment.GetEnvironmentVariable("TopicName"));
                    await sender.SendMessageAsync(message);
                        
                    // log the message sent and include the order ID
                    _logger.LogInformation($"Message sent: {message.MessageId}");

                    RetryCounter.CurrentRetryCount = 0;
                }
            }
        }
        catch (Exception)
        {
            RetryCounter.CurrentRetryCount++;
            _logger.LogInformation($"Current retry count is {RetryCounter.CurrentRetryCount}");
            throw;
        }
    }
}

// For demonstration purposes only
public static class RetryCounter
{
    public static int CurrentRetryCount = 0;
}
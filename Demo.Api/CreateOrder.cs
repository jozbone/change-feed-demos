namespace Demo.Api;

public class CreateOrder
{
    private readonly ILogger<CreateOrder> _logger;
    private readonly CosmosClient _client;
    private readonly Container _container;

    public CreateOrder(CosmosClient cosmosClient, ILogger<CreateOrder> logger)
    {
        _client = cosmosClient;
        _logger = logger;
        var databaseId = Environment.GetEnvironmentVariable("CosmosDBDatabaseId");
        var containerId = Environment.GetEnvironmentVariable("CosmosDBContainerId");
        _container = _client.GetContainer(databaseId, containerId);
    }

    [Function("CreateOrder")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var job = JsonConvert.DeserializeObject<Job>(requestBody);

        for (int i = 0; i < job.Multiples; i++)
        {
            var order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = Guid.NewGuid().ToString(),
                Quantity = new Random().Next(1, 10),
                AccountNumber = 2
            };

            await _container.CreateItemAsync(order, new PartitionKey(order.OrderId));
        }


        return new OkResult();
    }
}
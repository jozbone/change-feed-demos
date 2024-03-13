﻿namespace Demo.Api.Models;

public class Order
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "orderId")]
    public string OrderId { get; set; }

    [JsonProperty(PropertyName = "quantity")]
    public int Quantity { get; set; }

    [JsonProperty(PropertyName = "accountNumber")]
    public int AccountNumber { get; set; }

    [JsonProperty(PropertyName = "processingRegion")]
    public string ProcessingRegion { get; set; }
}
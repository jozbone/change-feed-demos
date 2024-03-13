namespace Demo.Api.Models;

public class Job
{
    [JsonProperty(PropertyName = "multiples")]
    public int Multiples { get; set; }
}
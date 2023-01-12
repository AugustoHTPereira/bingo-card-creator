using System.Text.Json.Serialization;

public class CardRequest
{
    public IEnumerable<CardDate> Dates { get; set; }
    public string PromotedBy { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class CardDate 
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("award_description")]
    public string AwardDescription { get; set; }
}

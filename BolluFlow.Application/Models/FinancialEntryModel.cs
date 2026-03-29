using System.Text.Json.Serialization;

namespace BolluFlow.Application.Models;

public class FinancialEntryModel
{
    public long Id { get; set; }

    [JsonPropertyName("tipo")]
    public string Tipo { get; set; } = null!;

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; } = null!;

    [JsonPropertyName("origem")]
    public string Origem { get; set; } = null!;

    [JsonPropertyName("origem_id")]
    public long OrigemId { get; set; }

    public DateTime CreatedAt { get; set; }
}

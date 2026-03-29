using System.Text.Json.Serialization;
using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;
using MediatR;

namespace BolluFlow.Application.Features.Finance.CreateFinancialEntry;

public class CreateFinancialEntryCommand : ICommand<ResultOf<FinancialEntryModel>>
{
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
}

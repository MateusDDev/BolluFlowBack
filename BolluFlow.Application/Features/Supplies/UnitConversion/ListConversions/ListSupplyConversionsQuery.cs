using BolluFlow.Application.Abstractions;
using BolluFlow.Application.Models;

namespace BolluFlow.Application.Features.Supplies.UnitConversion.ListConversions;

public record ListSupplyConversionsQuery(long SupplyId) : IQuery<ResultOf<List<SupplyConversionModel>>>;
